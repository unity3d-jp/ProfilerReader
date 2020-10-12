using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct Frame
    {
        public const uint kInvalidFrame = 0xffffffff;
        public uint index;
        public ulong time;


        internal void Read(RawBinaryReader reader)
        {
            index = reader.ReadUint();
            time = reader.ReadUlong();
        }
    }
}