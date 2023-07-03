using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct UnityObjectInfo
    {
        public int instanceId;
        // exclude editor only
        public int relatedGameObjectInstanceId;
        public string name;
        public uint runtimeTypeID;
        public int serializedFileIndex;
        public long localIdentifierInFile;
        // exclude editor only
        public ulong rootId;


        internal void Read(RawBinaryReader reader,bool v1)
        {
            instanceId = reader.ReadInt();
            if (!v1)
            {
                relatedGameObjectInstanceId = reader.ReadInt();
            }
            name = reader.ReadStringAsTransfer();
            reader.Align();

            runtimeTypeID = reader.ReadUint();
            serializedFileIndex = reader.ReadInt();
            localIdentifierInFile = reader.ReadLong();
            if (!v1)
            {
                rootId = reader.ReadUlong();
            }

        }
    }
}