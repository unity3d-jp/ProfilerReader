using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;
using System.Text;
using UTJ.ProfilerReader.BinaryData.Thread;
using UTJ.ProfilerReader.RawData.Protocol;
using System.Runtime.Remoting.Channels;
using System;

namespace UTJ.ProfilerReader.Analyzer
{
    public class UrpGPUSampleToFile : AnalyzeToTextbaseFileBase
    {
        public enum Category : int
        {
            Opaque = 0,
            Transparent = 1,
            Shadowmap = 2,
            PostProcess = 3,
            Other = 4
        };

        private struct GpuTimeInfo
        {
            public int time;
            public int count;
        }
        private class FrameGpuTime
        {
            public int frameIdx;
            public Dictionary<int, GpuTimeInfo> gpuTimeByCategory;

            public void AddGpuSample(GPUTime gpuTime, int category)
            {
                if (gpuTimeByCategory == null)
                {
                    gpuTimeByCategory = new Dictionary<int, GpuTimeInfo>();
                }
                GpuTimeInfo time;
                if (gpuTimeByCategory.TryGetValue(category, out time))
                {
                    time.time += gpuTime.gpuTimeInMicroSec;
                    time.count += 1;
                    gpuTimeByCategory[category] = time;
                }
                else
                {
                    time = new GpuTimeInfo { time = gpuTime.gpuTimeInMicroSec, count = 1 };
                    gpuTimeByCategory.Add(category, time);
                }
            }
        }

        private List<FrameGpuTime> frameGpuTimes = new List<FrameGpuTime>();

        public override void CollectData(ProfilerFrameData frameData)
        {
            FrameGpuTime frameGpuTime = new FrameGpuTime();
            frameGpuTime.frameIdx = frameData.frameIndex;

            foreach (var threadData in frameData.m_ThreadData) {
                AddGpuSampleByThread(threadData, frameGpuTime);
            }
            this.frameGpuTimes.Add(frameGpuTime);
        }

        private void AddGpuSampleByThread(ThreadData thread, FrameGpuTime frameGpuTime)
        {
            if (thread == null) { return; }
            if (thread.m_GPUTimeSamples == null) { return; }
            foreach (var gpuSample in thread.m_GPUTimeSamples)
            {
                frameGpuTime.AddGpuSample(gpuSample, GetGpuCategoryByCpuSample(thread, gpuSample));
            }
        }
        private int GetGpuCategoryByCpuSample(ThreadData thread, GPUTime gpuSample)
        {
            int category = (int)Category.Other;
            var cpuSample = GetCpuSample(thread, gpuSample);
            for (var current = cpuSample; current != null;current = current.parent)
            {
                if(current.sampleName == null) { continue; }

                if (current.sampleName.EndsWith("Opaques"))
                {
                    return (int)Category.Opaque;
                }
                else if (current.sampleName.EndsWith("Transparents"))
                {
                    return (int)Category.Transparent;
                }
                else if (current.sampleName.EndsWith("ShadowMap"))
                {
                    return (int)Category.Shadowmap;
                }
                else if (current.sampleName.Contains("PostProcessing"))
                {
                    return (int)Category.PostProcess;
                }
            }
            return category;
        }

        private ProfilerSample GetCpuSample(ThreadData thread, GPUTime gpuSample)
        {
            int idx = (int)gpuSample.relatedSampleIndex;
            if(thread.m_AllSamples == null) { return null; }
            if( idx < 0 || idx >= thread.m_AllSamples.Count)
            {
                return null;
            }
            ProfilerSample cpuSample = thread.m_AllSamples[idx];
            return cpuSample;

        }


        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            csvStringGenerator.AppendColumn("frameIdx");


            foreach (var category in System.Enum.GetValues(typeof(Category))) 
            {
                csvStringGenerator.AppendColumn(category.ToString() + "(ms)");
            }
            csvStringGenerator.AppendColumn("callNum");
            foreach (var category in System.Enum.GetValues(typeof(Category)))
            {
                csvStringGenerator.AppendColumn(category.ToString() + "(calls)");
            }

            csvStringGenerator.NextRow();
            foreach( var gpuFrame in frameGpuTimes)
            {
                if (gpuFrame.gpuTimeByCategory == null) { continue; }
                csvStringGenerator.AppendColumn(gpuFrame.frameIdx);

                foreach (var category in System.Enum.GetValues(typeof(Category)))
                {
                    GpuTimeInfo val ;
                    if(gpuFrame.gpuTimeByCategory.TryGetValue((int)category,out val))
                    {
                        csvStringGenerator.AppendColumn( (float)val.time / 1000.0f);
                    }
                    else
                    {
                        csvStringGenerator.AppendColumn(0);
                    }
                }
                csvStringGenerator.AppendColumn("");

                foreach (var category in System.Enum.GetValues(typeof(Category)))
                {
                    GpuTimeInfo val;
                    if (gpuFrame.gpuTimeByCategory.TryGetValue((int)category, out val))
                    {
                        csvStringGenerator.AppendColumn(val.count);
                    }
                    else
                    {
                        csvStringGenerator.AppendColumn(0);
                    }
                }

                csvStringGenerator.NextRow();
            }
            return csvStringGenerator.ToString();
        }
        

        protected override string FooterName
        {
            get
            {
                return "_urp_gpu_sample.csv";
            }
        }


    }

}