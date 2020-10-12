
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioProfilerDSPInfo
    {
        [StatsData(0)]
        public uint id;
        [StatsData(1)]
        public uint target;
        [StatsData(2)]
        public uint targetPort;
        [StatsData(3)]
        public uint numChannels;
        [StatsData(4)]
        public uint nameOffset;
        [StatsData(5)]
        public float weight;
        [StatsData(6)]
        public float cpuLoad;
        [StatsData(7)]
        public float level1;
        [StatsData(8)]
        public float level2;
        [StatsData(9)]
        public uint numLevels;
        [StatsData(10)]
        public uint flags;
    }
}