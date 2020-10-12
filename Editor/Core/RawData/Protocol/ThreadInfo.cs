using System.Collections;
using System.Collections.Generic;

namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct ThreadInfo
    {
        public ulong threadID;
        public ulong startTime;// from 2018.2
        public uint flags;
        public string group;
        public string name;

        internal void Read(RawBinaryReader reader,uint version)
        {
            threadID = reader.ReadUlong();
            version = UTJ.ProfilerReader.BinaryData.ProfilerDataStreamVersion.ConvertVersionForExecute(version,reader.UnityVersion);
            if (version >= UTJ.ProfilerReader.BinaryData.ProfilerDataStreamVersion.Unity2018_2)
            {
                startTime = reader.ReadUlong();
            }
            flags = reader.ReadUint();
            group = reader.ReadStringAsTransfer();
            reader.Align();
            name = reader.ReadStringAsTransfer();
            reader.Align();
        }
    }
}