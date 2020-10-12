using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct AsyncMetadataAnchor
    {
        public uint asyncMetadataId;

        internal void Read(RawBinaryReader reader)
        {
            asyncMetadataId = reader.ReadUint();
        }
    }
}