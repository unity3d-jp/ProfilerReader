using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{

    public struct UIEvents
    {
        public byte[] eventMarker;
        public byte[] eventName;
        public byte[] batchInstanceId;

        internal void Read(RawBinaryReader reader)
        {
            eventMarker = reader.ReadByteArray();
            reader.Align();
            eventName = reader.ReadByteArray();
            reader.Align();
            batchInstanceId = reader.ReadByteArray();
            reader.Align();
        }
    }
}
