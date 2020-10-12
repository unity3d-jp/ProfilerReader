using System.IO;


namespace UTJ.ProfilerReader.BinaryData
{
    public class ProfilerBinaryLogReader : ILogReaderPerFrameData
    {
        // Memo:There are some impacts to opening and reading file for a long time.
        private FileStream fileStream;
        public int frameIndex { get; set; }
        private long currentPosition;
        private long fileLength;
        private bool isComplete = true;
        private string targetFile;
        private string unityVersion;

        private uint logfileVersion;

        public uint GetLogFileVersion()
        {
            return logfileVersion;
        }
        public ushort GetLogFilePlatform()
        {
            return 0xffff;
        }

        public float Progress
        {
            get
            {
                if (fileLength == 0) { return 0.0f; }
                if (this.isComplete) { return 1.0f; }
                return ((float)currentPosition / (float)fileLength);
            }
        }


        public bool IsComplete
        {
            get
            {
                return isComplete;
            }
        }

        public void SetFile(string path)
        {
            this.frameIndex = 0;
            this.currentPosition = 0;
            this.isComplete = false;
            this.targetFile = path;
            this.fileLength = 0;
        }

        public void ForceExit()
        {
            this.isComplete = true;
        }
        
        public UTJ.ProfilerReader.BinaryData.ProfilerFrameData ReadFrameData()
        {
            if (this.isComplete) { return null; }
            var data = new UTJ.ProfilerReader.BinaryData.ProfilerFrameBlockData();
            using (FileStream fileStream = File.OpenRead(this.targetFile))
            {
                this.fileLength = fileStream.Length;
                fileStream.Position = currentPosition;
                if (fileStream == null) { return null; }
                if (fileStream.Position >= fileStream.Length) { return null; }
                bool isRead = data.Read(fileStream,frameIndex,unityVersion);
                this.isComplete |= !isRead;
                if (isRead)
                {
                    ++frameIndex;
                }
                currentPosition = fileStream.Position;
                fileStream.Close();
            }
            if (isComplete) { return null; }
            this.logfileVersion = (uint)data.dataVersion;
            return data.frameData;
        }

        public int SkipFrameData(int frameCount)
        {
            if (this.isComplete) { return 0; }
            var data = new UTJ.ProfilerReader.BinaryData.ProfilerFrameBlockData();
            int skipCnt = 0;
            using (FileStream fileStream = File.OpenRead(this.targetFile))
            {
                this.fileLength = fileStream.Length;
                fileStream.Position = currentPosition;
                for (int i = 0; i < frameCount; ++i)
                {
                    data.ReadHeader(fileStream);

                    if (currentPosition + data.frameDataSize < fileStream.Length)
                    {
                        currentPosition += (data.frameDataSize) + UTJ.ProfilerReader.BinaryData.ProfilerFrameBlockData.HeaderSize;
                        ++frameIndex;
                        ++skipCnt;
                    }
                    else
                    {
                        isComplete = true;
                        break;
                    }
                }
                fileStream.Close();
            }
            return skipCnt;
        }


        public void SetUnityVersion(string ver) {
            this.unityVersion = ver;
        }


    }
}