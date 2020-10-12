using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.BinaryData.Stats
{

    public interface IStatsStream
    {
        int ReadInt();
        float ReadFloat();
        uint ReadUint();
    }
}