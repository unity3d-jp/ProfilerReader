using System.Collections;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public struct ProfileTimeFormat
        {
            public ulong timeValue{get;private set;}

            public void Read(System.IO.Stream stream)
            {
                this.ReadAsUlong(stream);
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