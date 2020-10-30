using System.Collections;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public struct ProfileTimeFormat
        {
            public ulong timeValue{get;private set;}

            public void Read(System.IO.Stream stream,uint version)
            {
                if (version >= ProfilerDataStreamVersion.Unity2020_2)
                {
                    this.timeValue = ProfilerLogUtil.ReadULong(stream);
                    this.timeValue /= 1000;
                }
                else if (version >= ProfilerDataStreamVersion.Unity2019_3)
                {
                    this.timeValue = ProfilerLogUtil.ReadULong(stream);
                }
                else
                {
                    this.timeValue = ProfilerLogUtil.ReadUint(stream);
                }
            }

            public void ReadAsInt(System.IO.Stream stream)
            {
                this.timeValue = ProfilerLogUtil.ReadUint(stream);
            }

            public void ReadAsUlong(System.IO.Stream stream)
            {
                this.timeValue = ProfilerLogUtil.ReadULong(stream);
            }
        }
    }
}