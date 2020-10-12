
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class NetworkOperationStats
    {
        public const int kMaxPlatformDependentStats = 8;

        [StatsData(0)]
        public int m_SystemUserMessage;
        [StatsData(1)]
        public int m_SystemObjectDestroy;
        [StatsData(2)]
        public int m_SystemRpc;
        [StatsData(3)]
        public int m_SystemObjectSpawn;
        [StatsData(4)]
        public int m_SystemOwner;
        [StatsData(5)]
        public int m_SystemCommand;
        [StatsData(6)]
        public int m_SystemLocalPlayerTransform;
        [StatsData(7)]
        public int m_SystemSyncEvent;
        [StatsData(8)]
        public int m_SystemUpdateVars;
        [StatsData(9)]
        public int m_SystemSyncList;
        [StatsData(10)]
        public int m_SystemObjectSpawnScene;

        [StatsData(100)]
        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];

    }
}