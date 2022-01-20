using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;

namespace UTJ.ProfilerReader.Analyzer
{

    public enum ProfilerLogFormat
    {
        TypeData,
        TypeRaw,
    }

    public interface IAnalyzeFileWriter
    {

        void SetFileInfo(string logfilename, string outputpath);
        void SetInfo(ProfilerLogFormat logformat,string unityVersion, uint dataversion, ushort platform);

        void CollectData(ProfilerFrameData frameData);

        void WriteResultFile(string logfilaneme,string outputpath);


    }
}