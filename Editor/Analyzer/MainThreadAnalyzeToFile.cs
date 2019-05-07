using System.Collections.Generic;

using UTJ.ProfilerReader.BinaryData;


namespace UTJ.ProfilerReader.Analyzer
{
    public class MainThreadAnalyzeToFile : IAnalyzeFileWriter
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

            public SampleData(string fname, string name)
            {
                this.sampleName = name;
                this.fullName = fname;
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


        public void CollectData(ProfilerFrameData frameData)
        {
            // 特別枠で frameDataのＣＰＵ時間を追加
            // 同一フレーム内に同じスレッド名が複数できるので…
            Dictionary<string, int> threadNameCounter = new Dictionary<string, int>(8);
            foreach (var thread in frameData.m_ThreadData)
            {
                if (thread.IsMainThread)
                {
                    CollectThread(thread);
                }
            }
            ++frameNum;
        }

        private void CollectThread(ThreadData thread)
        {
            if (thread == null || thread.m_AllSamples == null) { return; }
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
                AddSampleData(sample.fullSampleName, sample.sampleName, sample.selfTimeUs / 1000.0f, sample.timeUS / 1000.0f);
            }
            if (sample.children != null)
            {
                foreach (var child in sample.children)
                {
                    CollectFromNamedChildren(child);
                }
            }
            return;
        }

        private void AddSampleData(string fullName, string sampleName, float selfMsec, float execMsec)
        {

            SampleData sampleData = null;
            if (!this.samples.TryGetValue(fullName, out sampleData))
            {
                sampleData = new SampleData(fullName, sampleName);
                this.samples.Add(fullName, sampleData);
            }
            if (selfMsec < 0.0f)
            {
                ProfilerLogUtil.logErrorString("minus Param " + sampleName + ":" + selfMsec + ":" + execMsec );
            }
            sampleData.Called(selfMsec, execMsec);
        }

                /// <summary>
        /// 結果書き出し
        /// </summary>
        public void WriteResultFile(string path)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024 * 1024);
            sb.Append("name,fullname,callNum,self,sum(msec),perFrame(msec),min(msec),max(msec),total,sum(msec),perFrame(msec),min(msec),max(msec),\n");

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
                sb.Append(sampleData.sampleName.Replace("\n", "")).Append(",").
                    Append(sampleData.fullName.Replace("\n", "")).Append(",").
                    Append(sampleData.callNum).Append(",").Append(",");

                sb.Append(sampleData.totalSelfMsec).Append(",").
                    Append(sampleData.totalSelfMsec / frameNum).Append(",").
                    Append(sampleData.selfMinMSec).Append(",").
                    Append(sampleData.selfMaxMsec).Append(",").Append(",");


                sb.Append(sampleData.totalExecMsec).Append(",").
                    Append(sampleData.totalExecMsec / frameNum).Append(",").
                    Append(sampleData.execMinMSec).Append(",").
                    Append(sampleData.execMaxMsec).Append(",");
                
                sb.Append("\n");
            }

            System.IO.File.WriteAllText(path, sb.ToString());

        }
        public string ConvertPath(string originPath)
        {
            var path = originPath.Replace(".", "_") + "_main_self.csv";
            return path;
        }
        public MainThreadAnalyzeToFile()
        {
        }
    }
}