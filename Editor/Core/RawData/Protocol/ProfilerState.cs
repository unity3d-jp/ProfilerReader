using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct ProfilerState
    {
        public uint flags;
        public ulong time;
        public uint frameIndex;

        internal void Read(RawBinaryReader stream)
        {
            flags = stream.ReadUint();
            time = stream.ReadUlong();
            frameIndex = stream.ReadUint();
        }
    }
}