using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;

using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader.Analyzer
{

    public class JitInfoAnalyzeToFile : AnalyzeToTextbaseFileBase
    {
        private Dictionary<ulong, JitInfo> jitInfoDict = new Dictionary<ulong, JitInfo>();


        public override void CollectData(ProfilerFrameData frameData)
        {
            if( frameData == null )
            {
                return;
            }
            var jitInfos = frameData.m_jitInfos;
            if( jitInfos == null)
            {
                return;
            }
            foreach( var jitInfo in jitInfos)
            {
                if(jitInfo == null) { continue; }
                if(!jitInfoDict.ContainsKey(jitInfo.codeAddr))
                {
                    jitInfoDict.Add(jitInfo.codeAddr, jitInfo);
                }
            }

        }



        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            AppendHeaderToStringBuilder(csvStringGenerator);
            List<JitInfo> sortedJitInfo = new List<JitInfo>(this.jitInfoDict.Values);
            sortedJitInfo.Sort(new JitInfo.CompareByAddr() );

            foreach( var jitInfo in sortedJitInfo)
            {
                string name = jitInfo.name;
                string sourceFileName = jitInfo.sourceFileName;
                csvStringGenerator.AppendColumnAsAddr(jitInfo.codeAddr);
                csvStringGenerator.AppendColumn(jitInfo.size);
                csvStringGenerator.AppendColumn(name);
                csvStringGenerator.AppendColumn(sourceFileName);
                csvStringGenerator.AppendColumn(jitInfo.sourceFileLine);
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }


        private void AppendHeaderToStringBuilder(CsvStringGenerator csvStringGenerator)
        {
            csvStringGenerator.AppendColumn("Address");
            // Total
            csvStringGenerator.AppendColumn("Code Size");
            csvStringGenerator.AppendColumn("FunctionName");
            csvStringGenerator.AppendColumn("SourceFile");
            csvStringGenerator.AppendColumn("SourceLine");
            csvStringGenerator.NextRow();
        }


        protected override string FooterName
        {
            get
            {
                return "_jitInfos.csv";
            }
        }

    }
}
