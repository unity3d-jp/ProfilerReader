
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class NetworkMessageStats
    {

        public const int kMaxPlatformDependentStats = 8;

        [StatsData(0)]
        public int bytesIn;
        [StatsData(1)]
        public int bytesOut;

        [StatsData(2)]
        public int packetsIn;
        [StatsData(3)]
        public int HLAPIMsgsIn;
        [StatsData(4)]
        public int LLAPIMsgsIn;

        [StatsData(5)]
        public int packetsOut;
        [StatsData(6)]
        public int HLAPIMsgsOut;
        [StatsData(7)]
        public int LLAPIMsgsOut;

        [StatsData(8)]
        public int numberConnections;
        [StatsData(9)]
        public int rtt;
        [StatsData(10)]
        public int HLAPIResends;
        [StatsData(11)]
        public int HLAPIPending;

        [StatsData(100)]
        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];

    }
}