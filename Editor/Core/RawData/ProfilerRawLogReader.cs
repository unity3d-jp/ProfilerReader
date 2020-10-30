using System.Collections;
using System.Collections.Generic;

using UTJ.ProfilerReader.RawData.Converter;
using UTJ.ProfilerReader.RawData.Protocol;

namespace UTJ.ProfilerReader.RawData
{
    public class ProfilerRawLogReader : ILogReaderPerFrameData
    {
        const int BufferedFrame = 10;

        private RawDataReader reader;
        private RawToBinConvertBehaviour converter;
        private string unityVersion;


        private bool isComplete = true;
        private uint currentFrame = Frame.kInvalidFrame;

        public float Progress {
            get {
                if( reader == null)
                {
                    if(isComplete) { return 1.0f; }
                    else { return 0.0f; }
                }
                return reader.Progress;
            }
        }
        public bool IsComplete {
            get
            {
                return isComplete;
            }
        }
        public void SetFile(string file)
        {
            reader = new RawDataReader();
            reader.UnityVersion = this.unityVersion;
            converter = new RawToBinConvertBehaviour();
            converter.SetUnityVersion(this.unityVersion);
            reader.StartReading(file, converter);
            isComplete = false;
            currentFrame = Frame.kInvalidFrame;
        }
        public UTJ.ProfilerReader.BinaryData.ProfilerFrameData ReadFrameData()
        {
            return ReadFrameData(false);
        }

        public void ForceExit()
        {
            this.isComplete = true;
        }

        public uint GetLogFileVersion()
        {
            return converter.header.version;
        }
        public ushort GetLogFilePlatform()
        {
            return converter.header.platform;
        }

        public UTJ.ProfilerReader.BinaryData.ProfilerFrameData ReadFrameData(bool isSkip)
        {
            BinaryData.ProfilerFrameData result = null;
            if(this.IsComplete) { return null; }
            if( reader == null)
            {
                result = converter.GetFrameData(currentFrame);
                converter.ReleaseFrameData(currentFrame);
                ++currentFrame;
                if( result == null) { 
                    this.isComplete = true;
                }
                if (result != null)
                {
                    result.ApplyCountersToDeplicatedStats();
                }

                return result;
            }
            
            ulong fileFrame = currentFrame;

            while (currentFrame + BufferedFrame > fileFrame || currentFrame == Frame.kInvalidFrame)
            {
                bool flag = !converter.isFrameDataRead;
                reader.ReadBlock(reader.ExecuteData);

                if (reader.LastReadFrame.index != Protocol.Frame.kInvalidFrame)
                {
                    if( currentFrame == Frame.kInvalidFrame)
                    {
                        currentFrame = reader.LastReadFrame.index;
                    }
                    fileFrame = reader.LastReadFrame.index;
                }

                if (reader.IsComplete)
                {
                    reader = null;
                    break;
                }
            }
            result = converter.GetFrameData(currentFrame);
            converter.ReleaseFrameData(currentFrame);
            ++currentFrame;

            if (result != null)
            {
                result.ApplyCountersToDeplicatedStats();
            }
            return result;
        }

        public int SkipFrameData(int frameCount)
        {
            int value = 0;
            for( int i = 0; i < frameCount; ++i)
            {
                var frameData = ReadFrameData(true);
                if( frameData == null)
                {
                    break;
                }
                ++value;
            }
            return value;
        }
        public void SetUnityVersion(string version)
        {
            if (reader != null)
            {
                reader.UnityVersion = version;
            }
            if (converter != null)
            {
                converter.SetUnityVersion(this.unityVersion);
            }
            this.unityVersion = version;
        }

    }

}