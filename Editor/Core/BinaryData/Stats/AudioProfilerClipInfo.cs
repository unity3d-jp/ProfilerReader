
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioProfilerClipInfo
    {

        /*InstanceID*/

        [StatsData(0,0, ProfilerDataStreamVersion.Unity6000_5 - 1)]
        public int assetInstanceId;

        // 64bit from 6.5

        [StatsData(0, ProfilerDataStreamVersion.Unity6000_5 )]
        public ulong assetInstanceId64Bit;

        [StatsData(1)]
        public int assetNameOffset;
        [StatsData(2)]
        public int loadState;
        [StatsData(3)]
        public int internalLoadState;
        [StatsData(4)]
        public int age;
        [StatsData(5)]
        public int disposed;
        [StatsData(6)]
        public int numChannelInstances;
    }
}