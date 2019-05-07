using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;

namespace UTJ.ProfilerReader.Analyzer
{


    public interface IAnalyzeFileWriter
    {
        void CollectData(ProfilerFrameData frameData);

        void WriteResultFile(string path);

        string ConvertPath(string originPath);
    }
}