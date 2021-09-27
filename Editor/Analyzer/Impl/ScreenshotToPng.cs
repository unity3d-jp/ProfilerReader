using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;
using UTJ.ProfilerReader.RawData.Protocol;
using System.Text;
using System.IO;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace UTJ.ProfilerReader.Analyzer
{
    public class ScreenShotToProfiler : IAnalyzeFileWriter
    {

        public static readonly System.Guid MetadataGuid = new System.Guid("4389DCEB-F9B3-4D49-940B-E98482F3A3F8");
        public static readonly int InfoTag = -1;

        private string outputPath;
        private string logFile;
        private bool createDir = false;
        private StringBuilder stringBuilder = new StringBuilder();

        public enum TextureCompress : byte
        {
            None = 0,
            RGB_565 = 1,
            PNG = 2,
            JPG_BufferRGB565 = 3,
            JPG_BufferRGBA = 4,
        }
        private class CaptureData
        {
            public int profilerFrameIndex;
            public int idx;
            public int width;
            public int height;
            public int originWidth;
            public int originHeight;
            public TextureCompress compress;

            public CaptureData(int frameIdx ,byte[] data)
            {
                this.profilerFrameIndex = frameIdx;
                this.idx = GetIntValue(data, 0);
                this.width = GetShortValue(data, 4);
                this.height = GetShortValue(data, 6);
                this.originWidth = GetShortValue(data, 8);
                this.originHeight = GetShortValue(data, 10);

                if (data.Length > 12)
                {
                    this.compress = (ScreenShotToProfiler.TextureCompress)data[12];
                }
                else
                {
                    this.compress = ScreenShotToProfiler.TextureCompress.None;
                }

            }

            public static int GetIntValue(byte[] bin, int offset)
            {
                return (bin[offset + 0] << 0) +
                    (bin[offset + 1] << 8) +
                    (bin[offset + 2] << 16) +
                    (bin[offset + 3] << 24);
            }
            public static int GetShortValue(byte[] bin, int offset)
            {
                return (bin[offset + 0] << 0) +
                    (bin[offset + 1] << 8);
            }
        }
        private Dictionary<int, CaptureData> captureFrameData = new Dictionary<int, CaptureData>();


        public void CollectData(ProfilerFrameData frameData)
        {
            foreach( var thread in frameData.m_ThreadData)
            {
                if (thread.IsMainThread)
                {
                    ExecuteThreadData(frameData.frameIndex,thread);
                }
            }
        }
        private void ExecuteThreadData(int frameIdx,ThreadData thread)
        {
            if(thread == null || thread.m_AllSamples == null) { return; }
            foreach( var sample in thread.m_AllSamples)
            {
                if( sample == null || sample.sampleName != RawDataDefines.EmitFramemetataSample)
                {
                    continue;
                }
                if( sample.metaDatas == null || 
                    sample.metaDatas.metadatas == null ) { 
                    continue; 
                }
                ExecuteFrameMetadata(frameIdx,sample);
            }
        }

        private void ExecuteFrameMetadata(int frameIdx,ProfilerSample sample)
        {
            var metadatas = sample.metaDatas.metadatas;
            if (metadatas.Count < 2)
            {
                return;
            }
            var guidBin = metadatas[0].convertedObject as byte[];
            var tagId = (int)metadatas[1].convertedObject;
            var valueBin = metadatas[2].convertedObject as byte[];
            if (guidBin == null || valueBin == null)
            {
                return;
            }
            System.Guid guid = new System.Guid(guidBin);

            CaptureData captureData = null;
            if (guid != MetadataGuid)
            {
                return;
            }
            if (tagId == InfoTag)
            {
                captureData = new CaptureData(frameIdx,valueBin);
                this.captureFrameData.Add(captureData.idx, captureData);
                return;
            }
            if( this.captureFrameData.TryGetValue(tagId,out captureData)){
                ExecuteBinData(captureData, valueBin);
            }
        }

        private void InitDirectory()
        {
            if (!createDir)
            {
                if (!Directory.Exists(this.outputPath))
                {
                    Directory.CreateDirectory(this.outputPath);
                }
                createDir = true;
            }
        }

        // execute data
        private void ExecuteBinData(CaptureData captureData,byte[] binData)
        {
            this.InitDirectory();

            string file = GetFilePath(captureData);
            byte[] pngBin = GetImageBin(captureData,binData);
            if ( pngBin != null)
            {
                File.WriteAllBytes(file, pngBin);
            }
        }
        private byte[] GetImageBin(CaptureData captureData,byte[] binData)
        {
            if(binData == null) { return null; }
            switch (captureData.compress)
            {
#if UNITY_EDITOR
                case TextureCompress.None:
                    return ImageConversion.EncodeArrayToPNG(binData,
                        UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB,
                        (uint)captureData.width, (uint)captureData.height);
                case TextureCompress.RGB_565:
                    return ImageConversion.EncodeArrayToPNG(binData,
                        UnityEngine.Experimental.Rendering.GraphicsFormat.R5G6B5_UNormPack16,
                        (uint)captureData.width, (uint)captureData.height);
#endif
                case TextureCompress.PNG:
                case TextureCompress.JPG_BufferRGB565:
                case TextureCompress.JPG_BufferRGBA:
                    return binData;
            }
            return null;
        }
        private string GetFilePath(CaptureData captureData)
        {
            stringBuilder.Length = 0;
            stringBuilder.Append(this.outputPath).Append("/ss-");
            stringBuilder.Append(string.Format("{0:D5}", captureData.idx));
            switch (captureData.compress)
            {
                case TextureCompress.None:
                case TextureCompress.RGB_565:
                case TextureCompress.PNG:
                    stringBuilder.Append(".png");
                    break;
                case TextureCompress.JPG_BufferRGB565:
                case TextureCompress.JPG_BufferRGBA:
                    stringBuilder.Append(".jpg");
                    break;
            }
            return stringBuilder.ToString();
        }


        public void SetFileInfo(string logfilename, string outputpath)
        {
            this.outputPath = Path.Combine(outputpath, "screenshots");
            this.logFile = logfilename;
        }

        public void WriteResultFile(string logfilaneme, string outputpath)
        {
        }

        // nothing todo...
        public void SetInfo(ProfilerLogFormat logformat, string unityVersion, uint dataversion, ushort platform)
        {
        }

    }

}