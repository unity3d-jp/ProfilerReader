using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public static class ProfilerDataStreamVersion
        {

            public const int Unity2020_2 = 0x20200828;
            public const int Unity2020_1 = 0x20200312;

            public const int Unity2019_3 = 0x20191122;//0x20190514;
            public const int Actual_Unity2019_2 = 0x20180306;
            public const int Actual_Unity2019_1 = 0x20180306;
            public const int Unity2019_2 = Unity2019_1 + 1;
            public const int Unity2019_1 = Unity2018_3 + 1;

            public const int Unity2018_3 = 0x20181101;
            public const int Unity2018_2 = 0x20180123;
            public const int Unity2018_1 = 0x20171110;
            // 0x20171110
            public const int Actual_Unity2017_3 = 0x20170327;
            public const int Unity2017_3 = Unity2017_2 + 1;
            public const int Unity2017_2 = 0x20170929;
            public const int Unity2017_1 = 0x20170130;
            public const int Unity56 = 0x20161226;
            public const int Unity55 = 0x20160622;
            public const int Unity54 = 0x20140736;

            public static uint ConvertVersionForExecute(uint originVersion, string unityVersion="")
            {
                return (uint)ConvertVersionForExecute((int)originVersion, unityVersion);
            }

            public static int ConvertVersionForExecute(int originVersion,string unityVersion = "")
            {
                // avoid the version bug
                if ( !string.IsNullOrEmpty(unityVersion)  && originVersion == Unity2018_3)
                {
                    if (unityVersion.StartsWith("2019.1"))
                    {
                        return Unity2019_1;
                    }else if (unityVersion.StartsWith("2019.2"))
                    {
                        return Unity2019_2;
                    }
                }
                switch (originVersion)
                {
                    case Actual_Unity2017_3:
                        return Unity2017_3;
                    case Actual_Unity2019_1:
                        return Unity2019_1;
                }
                return originVersion;
            }

        }

        public partial class ProfilerFrameBlockData
        {

            public bool ReadHeader(System.IO.Stream stream)
            {
                const int endSig =  unchecked((int)0xDEADFEED);
                int firstParam = ProfilerLogUtil.ReadInt(stream);
                if (firstParam == endSig)
                {
                    return false;
                }
                if (firstParam == 0 || firstParam == 1)
                {
                    this.littleEndian = firstParam;
                    this.dataVersion = ProfilerLogUtil.ReadInt(stream);
                    this.frameDataSize = ProfilerLogUtil.ReadInt(stream);
                    this.threadCount = ProfilerLogUtil.ReadInt(stream);
                }
                // 2017.3 or later
                else
                {
                    this.littleEndian = 1;
                    this.dataVersion = firstParam;
                    this.frameDataSize = ProfilerLogUtil.ReadInt(stream);
                    this.threadCount = ProfilerLogUtil.ReadInt(stream);
                }
                return true;
            }

            public bool Read(System.IO.Stream stream, int frameIndex,string unityVersion)
            {
                bool headerRead = this.ReadHeader(stream);
                if (!headerRead)
                {
                    return false;
                }

                this.frameData = new ProfilerFrameData();
                bool flag = false;

                switch (this.dataVersion)
                {
                    case ProfilerDataStreamVersion.Unity54:
                        flag = this.frameData.Read_0x20140736(stream, frameDataSize, threadCount, frameIndex);
                        break;
                    case ProfilerDataStreamVersion.Unity55:
                        flag = this.frameData.Read_0x20160622(stream, frameDataSize, threadCount, frameIndex);
                        break;
                    case ProfilerDataStreamVersion.Unity56:
                        flag = this.frameData.Read_0x20161226(stream, frameDataSize, threadCount, frameIndex);
                        break;
                    case ProfilerDataStreamVersion.Unity2017_1:
                        flag = this.frameData.Read_0x20170130(stream, frameDataSize, threadCount, frameIndex);
                        break;
                    case ProfilerDataStreamVersion.Unity2017_2:
                        flag = this.frameData.Read_0x20170929(stream, frameDataSize, threadCount, frameIndex);
                        break;
                    case ProfilerDataStreamVersion.Actual_Unity2017_3:
                    case ProfilerDataStreamVersion.Unity2018_1:
                    case ProfilerDataStreamVersion.Unity2018_2:
                    case ProfilerDataStreamVersion.Unity2018_3:
                    case ProfilerDataStreamVersion.Actual_Unity2019_1:
                    case ProfilerDataStreamVersion.Unity2019_3:
                    case ProfilerDataStreamVersion.Unity2020_1:
                    case ProfilerDataStreamVersion.Unity2020_2:
                        flag = this.frameData.ReadGeneric(stream, frameDataSize, threadCount, frameIndex, 
                            (uint)ProfilerDataStreamVersion.ConvertVersionForExecute(this.dataVersion,unityVersion) );
                        break;
                    default:
                        ProfilerLogUtil.LogError("This log file isn't supported. 0x" + dataVersion.ToString("x"));
                        throw new System.Exception("This log file isn't supported. 0x" + dataVersion.ToString("x"));
                }
                return flag;
            }


        }
    }
}