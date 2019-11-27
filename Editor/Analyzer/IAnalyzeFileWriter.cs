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
        void SetInfo(ProfilerLogFormat logformat, uint dataversion, ushort platform);

        void CollectData(ProfilerFrameData frameData);

        void WriteResultFile(string logfilaneme,string outputpath);

    }
}