using System.Collections.Generic;

using UTJ.ProfilerReader.BinaryData;


namespace UTJ.ProfilerReader.Analyzer
{
    public class MainThreadAnalyzeToFile : AnalyzeToTextbaseFileBase
    {
        private class SampleData
        {
            public string fullName;
            public string sampleName;

            public float totalSelfMsec = 0.0f;
            public float selfMinMSec = float.MaxValue;
            public float selfMaxMsec = 0.0f;

            public float totalExecMsec = 0.0f;
            public float execMinMSec = float.MaxValue;
            public float execMaxMsec = 0.0f;

            public int callNum = 0;

            public string categoryName;

            public SampleData(string fname, string name,string category)
            {
                this.sampleName = name;
                this.fullName = fname;
                this.categoryName = category;
            }

            public void Called(float selfMsec, float execMsec)
            {

                selfMinMSec = ProfilerLogUtil.Min(selfMinMSec, selfMsec);
                selfMaxMsec = ProfilerLogUtil.Max(selfMaxMsec, selfMsec);
                totalSelfMsec += selfMsec;

                execMinMSec = ProfilerLogUtil.Min(execMinMSec, execMsec);
                execMaxMsec = ProfilerLogUtil.Max(execMaxMsec, execMsec);
                totalExecMsec += execMsec;
                ++callNum;
            }
        }
        private Dictionary<string, SampleData> samples = new Dictionary<string, SampleData>();
        private int frameNum = 0;


        public override void CollectData(ProfilerFrameData frameData)
        {
            // 特別枠で frameDataのＣＰＵ時間を追加
            // 同一フレーム内に同じスレッド名が複数できるので…
            Dictionary<string, int> threadNameCounter = new Dictionary<string, int>(8);
            foreach (var thread in frameData.m_ThreadData)
            {
                if (thread.IsMainThread)
                {
                    CollectThread(frameData,thread);
                }
            }
            ++frameNum;
        }

        private void CollectThread(ProfilerFrameData frameData,ThreadData thread)
        {
            if (thread == null || thread.m_AllSamples == null) { return; }
            foreach (var sample in thread.m_AllSamples)
            {
                if (sample.parent == null)
                {
                    CollectFromNamedChildren(frameData,sample);
                }
            }
        }

        private void CollectFromNamedChildren(ProfilerFrameData frameData,ProfilerSample sample)
        {
            if (!string.IsNullOrEmpty(sample.sampleName))
            {
                string category = ProtocolData.GetCategory(frameData,unityVersion, sample.group);
                AddSampleData(sample.fullSampleName, sample.sampleName, category, sample.selfTimeUs / 1000.0f, sample.timeUS / 1000.0f);
            }
            if (sample.children != null)
            {
                foreach (var child in sample.children)
                {
                    CollectFromNamedChildren(frameData,child);
                }
            }
            return;
        }

        private void AddSampleData(string fullName, string sampleName,string categoryName, float selfMsec, float execMsec)
        {

            SampleData sampleData = null;
            if (selfMsec < 0.0f)
            {
                ProfilerLogUtil.logErrorString("minus Param " + sampleName + ":" + selfMsec + ":" + execMsec);
                return;
            }
            if (selfMsec > 1000.0f * 50.0f)
            {
                ProfilerLogUtil.logErrorString("minus Param " + sampleName + ":" + selfMsec + ":" + execMsec);
                return;
            }

            if (!this.samples.TryGetValue(fullName, out sampleData))
            {
                sampleData = new SampleData(fullName, sampleName,categoryName);
                this.samples.Add(fullName, sampleData);
            }
            sampleData.Called(selfMsec, execMsec);
        }

        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            csvStringGenerator.AppendColumn("name").AppendColumn("fullname").AppendColumn("category").AppendColumn("callNum").
                AppendColumn("self").AppendColumn("sum(msec)").AppendColumn("perFrame(msec)").AppendColumn("min(msec)").AppendColumn("max(msec)").
                AppendColumn("total").AppendColumn("sum(msec)").AppendColumn("perFrame(msec)").AppendColumn("min(msec)").AppendColumn("max(msec)").
                NextRow();
            var sampleDataList = new List<SampleData>(samples.Values);
            sampleDataList.Sort((a, b) =>
            {
                if (a.totalSelfMsec > b.totalSelfMsec)
                {
                    return -1;
                }else if (a.totalSelfMsec < b.totalSelfMsec)
                {
                    return 1;
                }
                return 0;
            });
            foreach (var sampleData in sampleDataList)
            {
                csvStringGenerator.AppendColumn(sampleData.sampleName).
                    AppendColumn(sampleData.fullName).
                    AppendColumn(sampleData.categoryName).
                    AppendColumn(sampleData.callNum).AppendColumn("");

                csvStringGenerator.AppendColumn(sampleData.totalSelfMsec).
                    AppendColumn(sampleData.totalSelfMsec / frameNum).
                    AppendColumn(sampleData.selfMinMSec).
                    AppendColumn(sampleData.selfMaxMsec).AppendColumn("");


                csvStringGenerator.AppendColumn(sampleData.totalExecMsec).
                    AppendColumn(sampleData.totalExecMsec / frameNum).
                    AppendColumn(sampleData.execMinMSec).
                    AppendColumn(sampleData.execMaxMsec);
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }
        protected override string FooterName
        {
            get
            {
                return "_main_self.csv";
            }
        }

    }
}