using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Stats;
using UTJ.ProfilerReader.RawData.Protocol;

namespace UTJ.ProfilerReader.RawData.Converter
{
    
    public class RawDataDictionaryData
    {
        public const uint kInvalidMarkerID = 0xFFFFFFFF;
        public static readonly string kGCAllocMarkerName = "GC.Alloc";
        public static readonly string kFrameMetadataMarkerName = "Profiler.FrameMetadata";
        public static readonly string kProfilerDefaultMarkerName = "Profiler.Default";


        private Dictionary<ulong, ThreadInfo> threadDictionary;
        private Dictionary<uint, SamplerInfo> sampleDictionary;

        private Dictionary<ulong, MethodJitInfo> methodJitDictionary;
        private List<JitInfo> methodJitList;


        private uint m_GCAllocMakerID = kInvalidMarkerID;

        public uint GCAllocSampleID
        {
            get
            {
                return m_GCAllocMakerID;
            }
        }

        public RawDataDictionaryData()
        {
            threadDictionary = new Dictionary<ulong, ThreadInfo>(32);
            sampleDictionary = new Dictionary<uint, SamplerInfo>(1024 * 16);
            methodJitDictionary = new Dictionary<ulong, MethodJitInfo>(1024 * 16);
            methodJitList = new List<JitInfo>(1024 * 16);
        }

        public void AddThreadInfo(ref ThreadInfo info)
        {
            ulong key = info.threadID;
            if (!threadDictionary.ContainsKey(key))
            {
                threadDictionary.Add(key, info);
            }
            else
            {
                threadDictionary[key] = info;
            }
        }
        public void AddSampleInfo(ref SamplerInfo info)
        {
            uint key = info.samplerId;
            if (!sampleDictionary.ContainsKey(key))
            {
                sampleDictionary.Add(key, info);
            }
            else
            {
                sampleDictionary[key] = info;
            }         
            // append special Maker ID
            if (m_GCAllocMakerID == kInvalidMarkerID && info.name == kGCAllocMarkerName)
            {
                m_GCAllocMakerID = info.samplerId;
            }
        }
        public void AddMethodJitInfo(ref MethodJitInfo info)
        {
            if (!methodJitDictionary.ContainsKey(info.codeAddr))
            {
                methodJitDictionary.Add(info.codeAddr, info);

                JitInfo jitInfo = GenerateJitInfo(ref info);
                methodJitList.Add(jitInfo);
            }
        }
        private JitInfo GenerateJitInfo(ref MethodJitInfo info)
        {
            JitInfo jitInfo = new JitInfo();
            jitInfo.codeAddr = info.codeAddr;
            jitInfo.size = info.codeSize;
            jitInfo.name = info.name;
            jitInfo.sourceFileName = info.sourceFileName;
            jitInfo.sourceFileLine = info.sourceFileLine;
            return jitInfo;
        }

        public ThreadInfo GetThreadInfo(ulong threadId)
        {
            ThreadInfo info = new ThreadInfo();
            if( threadDictionary.TryGetValue(threadId,out info))
            {
                return info;
            }
            return info;
        }

        public SamplerInfo GetSamplerInfo(uint sampleId)
        {
            SamplerInfo info = new SamplerInfo();
            if (sampleDictionary.TryGetValue(sampleId, out info))
            {
                return info;
            }
            return info;
        }
        

        public List<JitInfo> GetMethodJitInfos()
        {
            return this.methodJitList;
        }
    }

}