using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class NetworkOperationStats
    {

        internal void Read_0x20140736(System.IO.Stream stream)
        {
            m_SystemUserMessage = ProfilerLogUtil.ReadInt(stream);
            m_SystemObjectDestroy = ProfilerLogUtil.ReadInt(stream);
            m_SystemRpc = ProfilerLogUtil.ReadInt(stream);
            m_SystemObjectSpawn = ProfilerLogUtil.ReadInt(stream);
            m_SystemOwner = ProfilerLogUtil.ReadInt(stream);
            m_SystemCommand = ProfilerLogUtil.ReadInt(stream);
            m_SystemLocalPlayerTransform = ProfilerLogUtil.ReadInt(stream);
            m_SystemSyncEvent = ProfilerLogUtil.ReadInt(stream);
            m_SystemUpdateVars = ProfilerLogUtil.ReadInt(stream);
            m_SystemSyncList = ProfilerLogUtil.ReadInt(stream);
            m_SystemObjectSpawnScene = ProfilerLogUtil.ReadInt(stream);

            for (int i = 0; i < kMaxPlatformDependentStats; ++i)
            {
                this.platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }
        }
    }
}