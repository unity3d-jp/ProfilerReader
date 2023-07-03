using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct GfxResourceInfo
    {
        public ulong gfxResourceId;
        public ulong size;
        public ulong relatedRootId;


        internal void Read(RawBinaryReader reader)
        {
            gfxResourceId = reader.ReadUlong();
            size = reader.ReadUlong();
            relatedRootId = reader.ReadUlong();
        }
    }
}