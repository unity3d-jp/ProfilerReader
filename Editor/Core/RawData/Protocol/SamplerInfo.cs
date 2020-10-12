using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct SamplerInfo
    {
        // ProfilerInformation.m_ID
        public uint samplerId;
        // ProfilerInformation.m_Flags
        public ushort flags;
        // ProfilerInformation.m_Group
        public ushort group;
        // ProfilerInformation.m_Name
        public string name;
        // ProfilerInformation.m_MetadataDescription
        public byte metadataDescriptionsCount;
        public List<MetadataDescription> metadataDescriptions;

        public struct MetadataDescription
        {
            public ushort paramType;
            public string paramName;
        };

        internal void Read(RawBinaryReader reader)
        {
            samplerId = reader.ReadUint();
            flags = reader.ReadUshort();
            group = reader.ReadUshort();
            name = reader.ReadStringAsTransfer();
            reader.Align();
            metadataDescriptionsCount = (byte)reader.ReadByte();
            reader.Align();
            if (metadataDescriptionsCount > 0)
            {
                metadataDescriptions = new List<MetadataDescription>(metadataDescriptionsCount);
                for (int i = 0; i < metadataDescriptionsCount; ++i)
                {
                    var metadataDesc = new MetadataDescription();
                    metadataDesc.paramType = reader.ReadUshort();
                    reader.Align();
                    metadataDesc.paramName = reader.ReadStringAsTransfer();
                    reader.Align();
                    metadataDescriptions.Add(metadataDesc);
                }
            }
        }
    }

}