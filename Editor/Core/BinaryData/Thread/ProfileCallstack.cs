
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Thread
{

    // after 5.5
    public class ProfileCallstack
    {
        public uint relatedSampleIndex;
        public uint hash;
        public ulong[] stack = new ulong[32];

        public void Read(System.IO.Stream stream,uint version)
        {
            relatedSampleIndex = ProfilerLogUtil.ReadUint(stream);

            for (int i = 0; i < 32; ++i)
            {
                if (version >= ProfilerDataStreamVersion.Unity2020_2)
                {
                    stack[i] = ProfilerLogUtil.ReadUint(stream);
                    stack[i] |= ((ulong)ProfilerLogUtil.ReadUint(stream)) << 32;
                }
                else
                {
                    // ulong but 4byte only...
                    stack[i] = ProfilerLogUtil.ReadUint(stream);
                }
            }
        }
    }
}
