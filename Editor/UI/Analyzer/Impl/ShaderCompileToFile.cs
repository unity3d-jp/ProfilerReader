using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;
using System.Text;
using UTJ.ProfilerReader.BinaryData.Thread;
using UTJ.ProfilerReader.RawData.Protocol;

namespace UTJ.ProfilerReader.Analyzer
{
    public class ShaderCompileToFile : AnalyzeToTextbaseFileBase
    {
        private class ShaderCompileInfo
        {
            public int frameIdx = 0;
            public string shader;
            public float msec = 0.0f;
            public string pass;
            public string stage;
            public string keywords;
            public bool callFromWarmup = false;

            public ShaderCompileInfo()
            {
                this.shader = "";
                this.pass = "";
                this.stage = "";
                this.keywords = "";
            }
        }

        private List<ShaderCompileInfo> compileInfos = new List<ShaderCompileInfo>();
        private bool hasPassStageKeywordsInfo = false;

        public override void CollectData(ProfilerFrameData frameData)
        {

            foreach ( var threadData in frameData.m_ThreadData){
                CollectThreadData(frameData.frameIndex,threadData);
            }
        }

        private void CollectThreadData(int frameIdx,ThreadData thread)
        {
            if( thread == null) { return; }
            if( thread.m_AllSamples == null) { return; }

            foreach( var sample in thread.m_AllSamples)
            {
                if (sample.sampleName == "Shader.CreateGPUProgram")
                {
                    AddShaderCompileSample(frameIdx, sample);
                }
            }
        }


        private void AddShaderCompileSample(int frameIdx,ProfilerSample sampleData)
        {
            var compileInfo = new ShaderCompileInfo();
            compileInfo.frameIdx = frameIdx;
            compileInfo.msec = sampleData.timeUS / 1000.0f;
            compileInfo.callFromWarmup = IsCalledWarmup(sampleData.parent);
            if( sampleData.metaDatas != null )
            {
                var metadatas = sampleData.metaDatas.metadatas;
                if( metadatas != null)
                {
                    if (metadatas.Count > 0)
                    {
                        compileInfo.shader = metadatas[0].convertedObject as string;
                    }
                    if (metadatas.Count > 1)
                    {
                        compileInfo.pass = metadatas[1].convertedObject as string;
                        this.hasPassStageKeywordsInfo = true;
                    }

                    if (metadatas.Count > 2)
                    {
                        compileInfo.stage = metadatas[2].convertedObject as string;
                    }

                    if (metadatas.Count > 3)
                    {
                        compileInfo.keywords = metadatas[3].convertedObject as string;
                    }
                }
            }
            this.compileInfos.Add(compileInfo);
        }
        //
        private bool IsCalledWarmup(ProfilerSample sampleData)
        {
            for (var current = sampleData; current != null; current = current.parent)
            {
                if( current.sampleName == "ShaderVariantCollection.WarmupShaders" ||
                    current.sampleName == "Shader.WarmupAllShaders")
                {
                    return true;
                }
            }
            //            "ShaderVariantCollection.WarmupShaders"
            return false;
        }


        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            csvStringGenerator.AppendColumn("frameIdx");
            csvStringGenerator.AppendColumn("Shader")
                .AppendColumn("exec(ms)").AppendColumn("isWarmupCall");
            if (this.hasPassStageKeywordsInfo)
            {
                csvStringGenerator.AppendColumn("pass")
                    .AppendColumn("stage")
                    .AppendColumn("keyword");
            }
            csvStringGenerator.NextRow();
            foreach (var compileInfo in this.compileInfos)
            {
                if( compileInfo == null) { continue; }
                    
                csvStringGenerator.AppendColumn(compileInfo.frameIdx)
                    .AppendColumn(compileInfo.shader)
                    .AppendColumn(compileInfo.msec)
                    .AppendColumn(compileInfo.callFromWarmup);
                if (this.hasPassStageKeywordsInfo) {
                    csvStringGenerator.AppendColumn(compileInfo.pass).
                            AppendColumn(compileInfo.stage).
                            AppendColumn(compileInfo.keywords);
                }
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }
        

        protected override string FooterName
        {
            get
            {
                return "_shader_compile.csv";
            }
        }


    }

}