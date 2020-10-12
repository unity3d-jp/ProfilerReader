using UTJ.ProfilerReader.RawData;
using UTJ.ProfilerReader.BinaryData;

namespace UTJ.ProfilerReader
{
    public interface ILogReaderPerFrameData
    {
        uint GetLogFileVersion();
        ushort GetLogFilePlatform();

        void ForceExit();

        float Progress { get; }
        bool IsComplete { get; }
        void SetFile(string file);
        UTJ.ProfilerReader.BinaryData.ProfilerFrameData ReadFrameData();

        int SkipFrameData(int frameCount);

        void SetUnityVersion(string version);
    }
    
}
