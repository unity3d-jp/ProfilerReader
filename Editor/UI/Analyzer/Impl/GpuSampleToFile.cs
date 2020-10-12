using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;
using System.Text;
using UTJ.ProfilerReader.BinaryData.Thread;
using UTJ.ProfilerReader.RawData.Protocol;

namespace UTJ.ProfilerReader.Analyzer
{
    public class GPUSampleToFile : AnalyzeToTextbaseFileBase
    {
        private struct GpuTimeInfo
        {
            public int time;
            public int count;
        }
        private class FrameGpuTime
        {
            public int frameIdx;
            public Dictionary<int, GpuTimeInfo> gpuTimeByCategory;

            public void AddGpuSample(GPUTime gpuTime)
            {
                if(gpuTimeByCategory == null)
                {
                    gpuTimeByCategory = new Dictionary<int, GpuTimeInfo>();
                }
                GpuTimeInfo time;
                if( gpuTimeByCategory.TryGetValue( gpuTime.gpuSection,out time))
                {
                    time.time += gpuTime.gpuTimeInMicroSec;
                    time.count += 1;
                    gpuTimeByCategory[gpuTime.gpuSection] = time;
                }
                else
                {
                    time = new GpuTimeInfo { time = gpuTime.gpuTimeInMicroSec, count = 1 };
                    gpuTimeByCategory.Add(gpuTime.gpuSection, time);
                }
            }
        }

        private List<FrameGpuTime> frameGpuTimes = new List<FrameGpuTime>();

        public override void CollectData(ProfilerFrameData frameData)
        {
            FrameGpuTime frameGpuTime = new FrameGpuTime();
            frameGpuTime.frameIdx = frameData.frameIndex;

            foreach ( var threadData in frameData.m_ThreadData){
                AddGpuSampleByThread(threadData, frameGpuTime);
            }
            this.frameGpuTimes.Add(frameGpuTime);
        }

        private void AddGpuSampleByThread(ThreadData thread, FrameGpuTime frameGpuTime)
        {
            if( thread == null) { return; }
            if( thread.m_GPUTimeSamples == null) { return; }
            foreach( var gpuSample in thread.m_GPUTimeSamples)
            {
                frameGpuTime.AddGpuSample(gpuSample);
            }
        }


        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            csvStringGenerator.AppendColumn("frameIdx");

            for (int i = 0; i < GPUTime.SECTION_NUM; ++i)
            {
                csvStringGenerator.AppendColumn(((GPUTime.GpuSection)i).ToString() + "(ms)");
            }
            csvStringGenerator.AppendColumn("callNum");
            for (int i = 0; i < GPUTime.SECTION_NUM; ++i)
            {
                csvStringGenerator.AppendColumn(((GPUTime.GpuSection)i).ToString() + "(calls)");
            }

            csvStringGenerator.NextRow();
            foreach( var gpuFrame in frameGpuTimes)
            {
                if (gpuFrame.gpuTimeByCategory == null) { continue; }
                csvStringGenerator.AppendColumn(gpuFrame.frameIdx);

                for( int i = 0; i < GPUTime.SECTION_NUM; ++i)
                {
                    GpuTimeInfo val ;
                    if(gpuFrame.gpuTimeByCategory.TryGetValue(i,out val))
                    {
                        csvStringGenerator.AppendColumn( (float)val.time / 1000.0f);
                    }
                    else
                    {
                        csvStringGenerator.AppendColumn(0);
                    }
                }
                csvStringGenerator.AppendColumn("");

                for (int i = 0; i < GPUTime.SECTION_NUM; ++i)
                {
                    GpuTimeInfo val;
                    if (gpuFrame.gpuTimeByCategory.TryGetValue(i, out val))
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
                return "_gpu_sample.csv";
            }
        }


    }

}