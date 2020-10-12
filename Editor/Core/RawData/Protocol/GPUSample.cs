using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.RawData.Protocol
{

    public struct GPUSample
    {
        public uint asyncMetadataId;
        public uint elapsedGpuTimeInMicroSec;
        public byte gpuSection;

        internal void Read(RawBinaryReader reader)
        {
            asyncMetadataId = reader.ReadUint();
            elapsedGpuTimeInMicroSec = reader.ReadUint();
            gpuSection = reader.ReadByte();
            reader.Align();
        }
    }
}
