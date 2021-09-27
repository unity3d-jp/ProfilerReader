using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;


namespace UTJ.ProfilerReader.Analyzer
{
    public class MainThreadCategoryAnalyzeToFile : AnalyzeToTextbaseFileBase
    {
        private class FrameByCategory
        {
            public Dictionary<string, float> frameData;
            public int frameIdx;

            public void AddData(string category,float msec)
            {
                if(frameData == null)
                {
                    frameData = new Dictionary<string, float>();
                }
                float val;
                if( frameData.TryGetValue(category,out val) ){
                    frameData[category] = val + msec;
                }
                else
                {
                    frameData.Add( category , msec );
                }
            }

            public void AppendCsv(CsvStringGenerator csvStringGenerator, List<string> categoriesStr)
            {
                foreach (var category in categoriesStr)
                {
                    float val;

                    if (!frameData.TryGetValue(category, out val))
                    {
                        val = 0.0f;
                    }
                    csvStringGenerator.AppendColumn(val);
                }
            }
        }

        private Dictionary<int,string> categoryDictionary;
        private List<string> categoriesStr;
        private List<FrameByCategory> frames = new List<FrameByCategory>();

        private void CollectThread(ThreadData thread,FrameByCategory frameByCategory)
        {
            if (thread.m_AllSamples == null) { return; }
            foreach (var sample in thread.m_AllSamples)
            {
                string category = null;
                if(categoryDictionary.TryGetValue(sample.group,out category) ){
                    frameByCategory.AddData(categoriesStr[sample.group], sample.selfTimeUs * 0.001f);
                }
            }
        }
        public override void CollectData(ProfilerFrameData frameData)
        {
            // Categoryのセットアップ
            SetupCategories(frameData);
            FrameByCategory frameByCategory = new FrameByCategory();
            frameByCategory.frameIdx = frameData.frameIndex;
            // 特別枠で frameDataのＣＰＵ時間を追加
            // 同一フレーム内に同じスレッド名が複数できるので…
            Dictionary<string, int> threadNameCounter = new Dictionary<string, int>(8);
            foreach (var thread in frameData.m_ThreadData)
            {
                if (thread.IsMainThread)
                {
                    CollectThread(thread, frameByCategory);
                }
            }
            this.frames.Add(frameByCategory);
        }
        private void SetupCategories(ProfilerFrameData frameData)
        {
            if(this.categoryDictionary != null)
            {
                return;
            }
            this.categoriesStr = new List<string>();
            this.categoryDictionary = new Dictionary<int,string>();
            var categories = ProtocolData.GetCategories(frameData, this.unityVersion);
            foreach( var item in categories)
            {
                string name = item.Value.name;
                int idx = (int)item.Value.categoryId;
                if (!categoryDictionary.ContainsKey(idx))
                {
                    categoriesStr.Add(name);
                    categoryDictionary.Add(idx,name);
                }
            }
        }
        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            csvStringGenerator.AppendColumn("frameIdx");
            foreach( var str in categoriesStr)
            {
                csvStringGenerator.AppendColumn(str+"(msec)");
            }            
            csvStringGenerator.NextRow();

            foreach( var frame in frames)
            {
                csvStringGenerator.AppendColumn(frame.frameIdx);
                frame.AppendCsv(csvStringGenerator,this.categoriesStr);
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }
        
        protected override string FooterName
        {
            get
            {
                return "_category_mainThread_frame.csv";
            }
        }

    }

}
