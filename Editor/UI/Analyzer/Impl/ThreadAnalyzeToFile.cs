using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;


namespace UTJ.ProfilerReader.Analyzer
{

    public class ThreadAnalyzeToFile : AnalyzeToTextbaseFileBase
    {
        private class ThreadViewData
        {
            public int maxFrame = 0;
            public string threadName;
            public Dictionary<int, float> totalMsecList;
            public Dictionary<int, float> idleMsecList;

            public Dictionary<int, int> totalCount;
            public Dictionary<int, int> idleCount;

            public ThreadViewData(string name)
            {
                this.threadName = name;
                this.totalMsecList = new Dictionary<int, float>(512);
                this.idleMsecList = new Dictionary<int, float>(512);
                idleCount = new Dictionary<int, int>(512);
                totalCount = new Dictionary<int, int>(512);
            }

            public void AddMsec(int frame, float total, float idle, int totalCnt, int idleCnt)
            {
                maxFrame = ProfilerLogUtil.Max(frame, maxFrame);
                if (!this.totalMsecList.ContainsKey(frame))
                {

                    this.totalMsecList.Add(frame, total);
                    this.idleMsecList.Add(frame, idle);
                    this.totalCount.Add(frame, totalCnt);
                    this.idleCount.Add(frame, idleCnt);
                }
                else
                {
                    ProfilerLogUtil.logErrorString("same frame " + threadName +"::" + frame);
                }
            }

            public void GetMSecData(int frame, out float total, out float idle, out int totalCnt, out int idleCnt)
            {
                total = 0.0f;
                idle = 0.0f;
                totalMsecList.TryGetValue(frame, out total);
                idleMsecList.TryGetValue(frame, out idle);
                totalCount.TryGetValue(frame, out totalCnt);
                idleCount.TryGetValue(frame, out idleCnt);
            }
        }
        private Dictionary<string, ThreadViewData> viewData = new Dictionary<string, ThreadViewData>();

        const string FrameWholeDataSpecialKey = "CPUTotal";

        public override void CollectData(ProfilerFrameData frameData)
        {
            // 特別枠で frameDataのＣＰＵ時間を追加
            ThreadViewData frameViewData = null;
            if (!this.viewData.TryGetValue(FrameWholeDataSpecialKey, out frameViewData))
            {
                frameViewData = new ThreadViewData(FrameWholeDataSpecialKey);
                viewData.Add(FrameWholeDataSpecialKey, frameViewData);
            }
            frameViewData.AddMsec(frameData.frameIndex, frameData.m_TotalCPUTimeInMicroSec / 1000.0f, 0.0f, 0, 0);

            // 同一フレーム内に同じスレッド名が複数できるので…
            Dictionary<string, int> threadNameCounter = new Dictionary<string, int>(8);
            foreach (var thread in frameData.m_ThreadData)
            {
                string threadName = thread.FullName;
                if (threadName == null) { continue; }
                int cnt = 0;
                if (threadNameCounter.TryGetValue(threadName, out cnt))
                {
                    ++cnt;
                    threadName = threadName + cnt;
                }
                threadNameCounter[threadName] = cnt;
                this.AddDataTo(frameData.frameIndex, threadName, thread);
            }
        }

        private void AddDataTo(int frameIdx, string threadName, ThreadData data)
        {
            ThreadViewData threadViewData = null;
            if (!this.viewData.TryGetValue(threadName, out threadViewData))
            {
                threadViewData = new ThreadViewData(threadName);
                viewData.Add(threadName, threadViewData);
            }
            float totalMsec = 0.0f;
            float idleMsec = 0.0f;
            int totalCount = 0;
            int idleCount = 0;
            if (data.m_AllSamples != null)
            {
                foreach (var sample in data.m_AllSamples)
                {
                    if (sample.parent == null)
                    {
                        idleMsec += GetSumOfTimeInSampleChildren(sample, "Idle", ref idleCount);
                        totalMsec += GetSumeOfTimeWithNamedSampleInChildren(sample, ref totalCount);// sample.timeUS / 1000.0f;
                    }
                }
            }
            threadViewData.AddMsec(frameIdx, totalMsec, idleMsec, totalCount, idleCount);
        }

        private float GetSumeOfTimeWithNamedSampleInChildren(ProfilerSample sample, ref int count)
        {
            float sum = 0.0f;
            if (!string.IsNullOrEmpty(sample.sampleName))
            {
                count += 1;
                return sample.timeUS / 1000.0f;
            }
            if (sample.children == null)
            {
                return sum;
            }
            foreach (var child in sample.children)
            {
                sum += GetSumeOfTimeWithNamedSampleInChildren(child, ref count);
            }
            return sum;
        }

        private float GetSumOfTimeInSampleChildren(ProfilerSample sample, string matchStr, ref int count)
        {
            float sum = 0.0f;
            if (sample == null) { return sum; }
            if (sample.sampleName == matchStr)
            {
                count += 1;
                return sample.timeUS / 1000.0f;
            }
            if (sample.children == null)
            {
                return sum;
            }

            foreach (var child in sample.children)
            {
                sum += GetSumOfTimeInSampleChildren(child, matchStr, ref count);
            }
            return sum;
        }

        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            int frameNum = 0;
            var threadViewDataList = new List<ThreadViewData>(viewData.Values);

            foreach (var data in threadViewDataList)
            {
                if (data.threadName == FrameWholeDataSpecialKey)
                {
                    csvStringGenerator.AppendColumn(data.threadName).AppendColumn("");
                }
                else
                {
                    csvStringGenerator.AppendColumn(data.threadName + "(msec)");
                    csvStringGenerator.AppendColumn("idle(msec)");
                    csvStringGenerator.AppendColumn("working(msec)");
                    csvStringGenerator.AppendColumn("rootBlock");
                    csvStringGenerator.AppendColumn("idleBlock").AppendColumn("");
                }
                frameNum = ProfilerLogUtil.Max(data.maxFrame, frameNum);
            }
            csvStringGenerator.NextRow();

            for (int i = 0; i < frameNum; ++i)
            {
                foreach (var data in threadViewDataList)
                {
                    int totalCnt, idleCnt;
                    float total, idle;
                    data.GetMSecData(i, out total, out idle, out totalCnt, out idleCnt);
                    if (data.threadName == FrameWholeDataSpecialKey)
                    {
                        csvStringGenerator.AppendColumn(total).AppendColumn("");
                    }
                    else
                    {
                        csvStringGenerator.AppendColumn(total).AppendColumn(idle).AppendColumn(total - idle);
                        csvStringGenerator.AppendColumn(totalCnt).AppendColumn(idleCnt).AppendColumn("");
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
                return "_result.csv";
            }
        }

    }
}
