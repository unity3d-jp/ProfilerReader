using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.BinaryData.Stats
{
    /// <summary>
    /// GI stas from 2017.2
    /// </summary>
    public partial class GlobalIlluminationStats
    {
        [StatsData(0, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_TotalCPUTime;
        [StatsData(1, ProfilerDataStreamVersion.Unity2017_2)]
        public int m_TotalSystemCount;
        [StatsData(2, ProfilerDataStreamVersion.Unity2017_2)]
        public int m_TotalProbeSetCount;
        [StatsData(3, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_ProbeTime;
        [StatsData(4, ProfilerDataStreamVersion.Unity2017_2)]
        public int m_TotalProbesCount;
        [StatsData(5, ProfilerDataStreamVersion.Unity2017_2)]
        public int m_SolvedProbesCount;
        [StatsData(6, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_SetupTime;
        [StatsData(7, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_EnvironmentTime;
        [StatsData(8, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_InputLightingTime;
        [StatsData(9, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_SystemsTime;
        [StatsData(10, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_SolveTasksTime;
        [StatsData(11, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_DynamicObjectsTime;
        [StatsData(12, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_TimeBetweenUpdates;
        [StatsData(13, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_OtherCommandsTime;
        [StatsData(14, ProfilerDataStreamVersion.Unity2017_2)]
        public int m_BlockedBufferWritesCount;
        [StatsData(15, ProfilerDataStreamVersion.Unity2017_2)]
        public float m_BlockedCommandWriteTime;

        [StatsData(16, ProfilerDataStreamVersion.Unity2018_3)]
        public int m_PendingMaterialUpdateCount;
        [StatsData(17, ProfilerDataStreamVersion.Unity2018_3)]
        public int m_PendingAlbedoUpdateCount;
    }
}
