using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.RawData.Protocol
{

    public struct AllProfilerStats
    {
        uint gatheredData;
        public int[] data;

        internal void Read(RawBinaryReader reader,uint version)
        {
            version = ProfilerReader.BinaryData.ProfilerDataStreamVersion.ConvertVersionForExecute(version,reader.UnityVersion);
            if ( version >= ProfilerReader.BinaryData.ProfilerDataStreamVersion.Unity2018_3)
            {
                gatheredData = reader.ReadUint();
            }
            data = reader.ReadIntArray();
        }
    }
}
