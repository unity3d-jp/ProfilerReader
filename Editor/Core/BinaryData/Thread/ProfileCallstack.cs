
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Thread
{

    // after 5.5
    public class ProfileCallstack
    {
        public uint relatedSampleIndex;
        public uint hash;
        public ulong[] stack = new ulong[32];

        public void Read(System.IO.Stream stream)
        {
            relatedSampleIndex = ProfilerLogUtil.ReadUint(stream);

            for (int i = 0; i < 32; ++i)
            {
                // ulong but 4byte only...
                stack[i] = ProfilerLogUtil.ReadUint(stream);
            }
        }
    }
}
