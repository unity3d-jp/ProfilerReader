using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using UTJ.ProfilerReader.BinaryData;
using UnityEngine.Profiling;

using UTJ.ProfilerReader.BinaryData.Stats;
using UTJ.ProfilerReader.RawData.Protocol;
using UnityEngine.Playables;

namespace UTJ.ProfilerReader.Analyzer
{

    public class GcCallStackInfoAnalyzeToFile : AnalyzeToTextbaseFileBase
    {
        struct SampleKey : System.IEquatable<SampleKey>
        {
            public string threadName;
            public string methodName;
            public string fullMethodName;
            public string callStackName;
            private int hashCodeCache;

            public override int GetHashCode()
            {
                return this.hashCodeCache;
            }
            public bool Equals(SampleKey other)
            {
                return (
                    (other.callStackName == this.callStackName) &&
                    (other.fullMethodName == this.fullMethodName) &&
                    (other.threadName == this.threadName) );
            }

            public SampleKey(string th,string method,string fullMethod,string callStack)
            {
                threadName = th;
                methodName = method;
                fullMethodName = fullMethod;
                callStackName = callStack;
                this.hashCodeCache = threadName.GetHashCode() + fullMethodName.GetHashCode();
            }
        }
        
        class GcInfo
        {
            public uint allocNum;
            public ulong allocAll;
            public uint allocMax = uint.MinValue;
            public uint allocMin = uint.MaxValue;
        }


#if UTJ_CHECK
        private static readonly CustomSampler _collectDataSampler = CustomSampler.Create("GcCallStackInfoAnalyzeToFile.CollectData");
        private static readonly CustomSampler _addDataSampler = CustomSampler.Create("GcCallStackInfoAnalyzeToFile.AddData");
        private static readonly CustomSampler _callStackSampler = CustomSampler.Create("GcCallStackInfoAnalyzeToFile.GetCallStack");
        private static readonly CustomSampler _gcAllocGetSampler = CustomSampler.Create("GcCallStackInfoAnalyzeToFile.GetSelfChildGcAlloc");
#endif


        private Dictionary<SampleKey, GcInfo> gcDitionary = new Dictionary<SampleKey, GcInfo>();
        private StringBuilder stringBuilder = new StringBuilder(1024);

        private void AddData(string threadName,ProfilerSample sample,string callstack,uint gcAlloc)
        {
#if UTJ_CHECK
            using (new ProfilingScope(_addDataSampler))
#endif
            {
                var key = new SampleKey(threadName, sample.sampleName, sample.fullSampleName, callstack);
                GcInfo data;
                if (!gcDitionary.TryGetValue(key, out data))
                {
                    data = new GcInfo();
                    gcDitionary.Add(key, data);
                }
                data.allocAll += gcAlloc;
                data.allocNum++;
                data.allocMin = ProfilerLogUtil.Min(data.allocMin, gcAlloc);
                data.allocMax = ProfilerLogUtil.Max(data.allocMax, gcAlloc);
            }
        }


        public override void CollectData(ProfilerFrameData frameData)
        {
#if UTJ_CHECK
            using (new ProfilingScope(_collectDataSampler))
#endif
            {
                if (frameData == null)
                {
                    return;
                }

                FindGCAllocMarkerId(frameData);

                if (useMakerId)
                {
                    foreach (var thread in frameData.m_ThreadData)
                    {
                        if (thread.m_AllSamples == null) { continue; }
                        foreach (var sample in thread.m_AllSamples)
                        {
                            if (sample != null && sample.parent != null &&
                                sample.makerId == this.gcAllocMakerId)
                            {
                                uint selfGcAlloc = 0;
                                var parent = sample.parent;
#if UTJ_CHECK
                                using (new ProfilingScope(_gcAllocGetSampler))
#endif
                                {
                                    selfGcAlloc = sample.currenGcAlloc;
                                }
                                AddData(thread.FullName, parent, GetCallStackInfo(frameData, sample),
                                    selfGcAlloc);
                            }
                        }

                    }
                }
                else
                {
                    foreach (var thread in frameData.m_ThreadData)
                    {
                        if (thread.m_AllSamples == null) { continue; }
                        foreach (var sample in thread.m_AllSamples)
                        {
                            if (sample != null && sample.parent != null &&
                                sample.sampleName == "GC.Alloc")
                            {
                                var parent = sample.parent;
                                AddData(thread.FullName, parent, GetCallStackInfo(frameData, sample),
                                    sample.currenGcAlloc);
                            }
                        }
                    }
                }
            }
        }

        private bool useMakerId = false;
        private bool foundMakerId = false;
        private uint gcAllocMakerId = 0xFFFFFFFF;

        private void FindGCAllocMarkerId(ProfilerFrameData frameData)
        {
            if (foundMakerId) { return; }
            foreach (var thread in frameData.m_ThreadData)
            {
                if (thread.m_AllSamples == null) { continue; }
                foreach (var sample in thread.m_AllSamples)
                {
                    if (sample != null && sample.parent != null && 
                        sample.sampleName == "GC.Alloc")
                    {
                        gcAllocMakerId = sample.makerId;
                        foundMakerId = true;
                        if(gcAllocMakerId != 0 && gcAllocMakerId != 0xFFFFFFFF)
                        {
                            useMakerId = true;
                        }
                        break;
                    }
                }
            }
        }


        private string GetCallStackInfo( ProfilerFrameData frameData,ProfilerSample profilerSample)
        {
#if UTJ_CHECK
            using (new ProfilingScope(_callStackSampler))
#endif
            {

                if (profilerSample == null) { return ""; }
                var callStackInfo = profilerSample.callStackInfo;
                if (callStackInfo == null)
                {
                    return "";
                }
                stringBuilder.Length = 0;

                int length = callStackInfo.stack.Length;
                bool isAlreadyAdd = false;
                for (int i = length - 1; i >= 0; --i)
                {
                    var info = frameData.FindJitInfoFromAddr(callStackInfo.stack[i]);
                    if (info == null) { continue; }
                    if (isAlreadyAdd)
                    {
                        stringBuilder.Append("->");
                    }
                    stringBuilder.Append("[");
                    CsvStringGenerator.AppendAddrStr(stringBuilder, info.codeAddr, 16).Append("]");
                    stringBuilder.Append(info.name);
                    isAlreadyAdd = true;
                }
                return stringBuilder.ToString();
            }
        }




        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            AppendHeaderToStringBuilder(csvStringGenerator);
            foreach( var kvs in gcDitionary)
            {
                csvStringGenerator.AppendColumn(kvs.Key.threadName);
                csvStringGenerator.AppendColumn(kvs.Key.methodName);
                csvStringGenerator.AppendColumn(kvs.Key.fullMethodName);
                csvStringGenerator.AppendColumn(kvs.Key.callStackName);
                csvStringGenerator.AppendColumn(kvs.Value.allocNum);
                csvStringGenerator.AppendColumn(kvs.Value.allocAll);
                csvStringGenerator.AppendColumn(kvs.Value.allocAll / kvs.Value.allocNum);
                csvStringGenerator.AppendColumn(kvs.Value.allocMin);
                csvStringGenerator.AppendColumn(kvs.Value.allocMax);
                csvStringGenerator.NextRow();
            }
            return csvStringGenerator.ToString();
        }
        private void AppendHeaderToStringBuilder(CsvStringGenerator csvStringGenerator)
        {
            csvStringGenerator.AppendColumn("thread");
            // Total
            csvStringGenerator.AppendColumn("SampleName");
            csvStringGenerator.AppendColumn("FullName");
            csvStringGenerator.AppendColumn("CallStack");
            csvStringGenerator.AppendColumn("calls");
            csvStringGenerator.AppendColumn("all(byte)");
            csvStringGenerator.AppendColumn("average(byte)");
            csvStringGenerator.AppendColumn("min(byte)");
            csvStringGenerator.AppendColumn("max(byte)");
            csvStringGenerator.NextRow();

        }


        protected override string FooterName
        {
            get
            {
                return "_gc_detail.csv";
            }
        }

    }
}
