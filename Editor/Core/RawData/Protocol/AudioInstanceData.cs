using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.RawData.Protocol
{

    public struct AudioInstanceData
    {
        public int[] audioGroup;
        public int[] audioDSPInfo;
        public int[] audioClipInfo;
        public string audioNames;

        internal void Read(RawBinaryReader reader)
        {
            audioGroup = reader.ReadIntArray();
            audioDSPInfo = reader.ReadIntArray();
            audioClipInfo = reader.ReadIntArray();
            audioNames = reader.ReadStringAsTransfer();
            reader.Align();
        }
    }
}
