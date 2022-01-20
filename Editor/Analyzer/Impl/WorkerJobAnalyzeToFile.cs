using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;


namespace UTJ.ProfilerReader.Analyzer
{
    public class WorkerJobAnalyzeToFile : AnalyzeToTextbaseFileBase
    {
        private class WorkerThreadSample
        {
            public string sampleName;
            public float minMSec = float.MaxValue;
            public float maxMsec = 0.0f;
            public float sumMsec = 0.0f;
            public int callNum = 0;

            public WorkerThreadSample(string name)
            {
                this.sampleName = name;
            }

            public void Called(float msec)
            {
                minMSec = ProfilerLogUtil.Min(minMSec, msec);
                maxMsec = ProfilerLogUtil.Max(maxMsec, msec);
                sumMsec += msec;
                ++callNum;
            }
        }
        private void AddSampleData(string sampleName, float msec)
        {
            WorkerThreadSample sampleData = null;
            if (!this.samples.TryGetValue(sampleName, out sampleData))
            {
                sampleData = new WorkerThreadSample(sampleName);
                this.samples.Add(sampleName, sampleData);
            }
            sampleData.Called(msec);
        }

        private Dictionary<string, WorkerThreadSample> samples = new Dictionary<string, WorkerThreadSample>();


        private void CollectThread(ThreadData thread)
        {
            if (thread.m_AllSamples == null) { return; }
            foreach (var sample in thread.m_AllSamples)
            {
                if (sample.parent == null)
                {
                    CollectFromNamedChildren(sample);
                }
            }
        }

        private void CollectFromNamedChildren(ProfilerSample sample)
        {
            if (!string.IsNullOrEmpty(sample.sampleName))
            {
                AddSampleData(sample.sampleName, sample.timeUS / 1000.0f);
                return;
            }
            if (sample.children == null)
            {
                return;
            }
            foreach (var child in sample.children)
            {
                CollectFromNamedChildren(child);
            }
            return;
        }

        public override void CollectData(ProfilerFrameData frameData)
        {
            // 特別枠で frameDataのＣＰＵ時間を追加
            // 同一フレーム内に同じスレッド名が複数できるので…
            Dictionary<string, int> threadNameCounter = new Dictionary<string, int>(8);
            foreach (var thread in frameData.m_ThreadData)
            {
                if (thread.m_ThreadName == "Worker Thread" || thread.m_GroupName  == "Job" )
                {
                    CollectThread(thread);
                }
            }
        }
        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            csvStringGenerator.AppendColumn("name").AppendColumn("sum(msec)").AppendColumn("call").AppendColumn("min(msec)").AppendColumn("max(msec)").NextRow();

            var sampleDataList = new List<WorkerThreadSample>(samples.Values);
            sampleDataList.Sort((a, b) =>
            {
                if (a.sumMsec > b.sumMsec)
                {
                    return -1;
                }
                else if (a.sumMsec < b.sumMsec)
                {
                    return 1;
                }
                return 0;
            });
            foreach (var sampleData in sampleDataList)
            {
                csvStringGenerator.AppendColumn(sampleData.sampleName).
                    AppendColumn(sampleData.sumMsec).
                    AppendColumn(sampleData.callNum).
                    AppendColumn(sampleData.minMSec).
                    AppendColumn(sampleData.maxMsec);
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }
        
        protected override string FooterName
        {
            get
            {
                return "_worker.csv";
            }
        }

    }

}