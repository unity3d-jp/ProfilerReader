using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct GCAlloc
    {
        public uint size;

        internal void Read(RawBinaryReader reader)
        {
            size = reader.ReadUint();
        }
    }
}