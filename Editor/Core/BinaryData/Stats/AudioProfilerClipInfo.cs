
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioProfilerClipInfo
    {

        /*InstanceID*/

        [StatsData(0)]
        public int assetInstanceId;
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