using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct Callstack
    {
        const int kFramesCount = 32;
        public uint hash;
        public byte framesCount;
        public ulong[] frames; //

        internal void Read(RawBinaryReader reader)
        {
            hash = reader.ReadUint();
            framesCount = reader.ReadByte();
        }
        internal void ReadFrames(RawBinaryReader reader,int frameCount)
        {
            frames = new ulong[frameCount];
            for(int i = 0; i < frameCount; ++i)
            {
                frames[i] = reader.ReadUlong();
            }
//            frames = reader.ReadUlongArray();
        }
    }
}