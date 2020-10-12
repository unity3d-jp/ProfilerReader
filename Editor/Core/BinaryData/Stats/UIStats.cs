
using System.Collections;


namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class UIStats
    {
        const int kMaxPlatformDependentStats = 8;
        [StatsData(0,ProfilerDataStreamVersion.Unity2018_3)]
        public int batchCount;
        [StatsData(1, ProfilerDataStreamVersion.Unity2018_3)]
        public int vertexCount;
        [StatsData(2, ProfilerDataStreamVersion.Unity2018_3)]
        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];
    }
}