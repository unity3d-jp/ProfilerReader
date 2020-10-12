using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.RawData.Protocol
{

    public struct CleanupThread
    {
        public ulong endTime;

        internal void Read(RawBinaryReader reader)
        {
            endTime = reader.ReadUlong();
        }
    }
}
