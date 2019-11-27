using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;

namespace UTJ.ProfilerReader.Analyzer
{



    public abstract class AnalyzeToTextbaseFileBase : IAnalyzeFileWriter
    {
        protected ProfilerLogFormat logFormat
        {
            get; private set;
        }
        protected uint logVersion{
            get;private set;
        }
        protected ushort logPlatform
        {
            get;private set;
        }

        protected abstract string FooterName
        {
            get;
        }

        public void SetInfo(ProfilerLogFormat format, uint dataversion, ushort platform) {
            this.logFormat = format;
            this.logVersion = dataversion;
            this.logPlatform = platform;
        }

        public abstract void CollectData(ProfilerFrameData frameData);

        protected abstract string GetResultText();

        public void WriteResultFile(string logfile , string outputpath) {
            try
            {
                var path = System.IO.Path.Combine(outputpath, logfile.Replace(".", "_") + this.FooterName);
                string result = GetResultText();
                System.IO.File.WriteAllText(path, result);
            }
            catch (System.Exception e)
            {
                ProfilerLogUtil.logErrorException(e);
            }
        }


    }
}