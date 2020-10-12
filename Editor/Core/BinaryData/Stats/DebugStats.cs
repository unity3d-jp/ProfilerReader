
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class DebugStats
    {
        const int kMaxPlatformDependentStats = 8;
        [StatsData(0)]
        public int m_ProfilerMemoryUsage;
        [StatsData(1)]
        public int m_ProfilerMemoryUsageOthers;
        [StatsData(2)]
        public int m_AllocatedProfileSamples;


        [StatsData(3, ProfilerDataStreamVersion.Unity2019_3)]
        public int m_GpuProfilingStatisticsAvailabilityStates;
        [StatsData(4, ProfilerDataStreamVersion.Unity2019_3)]
        public int m_RuntimePlatform;
        [StatsData(5, ProfilerDataStreamVersion.Unity2019_3)]
        public int m_UnityVersionMajor;
        [StatsData(6, ProfilerDataStreamVersion.Unity2019_3)]
        public int m_UnityVersionMinor;
        [StatsData(7, ProfilerDataStreamVersion.Unity2019_3)]
        public int m_UnityVersionRevision;
        [StatsData(8, ProfilerDataStreamVersion.Unity2019_3)]
        public int m_UnityVersionReleaseType;
        [StatsData(9, ProfilerDataStreamVersion.Unity2019_3)]
        public int m_UnityVersionIncrementalVersion;

        public int[]platformDependentStats = new int[kMaxPlatformDependentStats];

    }
}