using System;
using System.IO;
namespace UTJ.ProfilerReader.BinaryData.Stats
{
    // 2022.3
    public class MetadataDescription
    {
        public ushort paramAndUnitType;
        public string paramName;

        public void Read(Stream stream)
        {
            paramAndUnitType = (ushort)ProfilerLogUtil.ReadInt(stream);
            paramName = ProfilerLogUtil.ReadString(stream);
        }
    }
}