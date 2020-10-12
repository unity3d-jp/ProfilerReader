
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class DebugStats
    {


        internal void Read_0x20170130(System.IO.Stream stream)
        {
            m_ProfilerMemoryUsage = ProfilerLogUtil.ReadInt(stream);
            m_ProfilerMemoryUsageOthers = ProfilerLogUtil.ReadInt(stream);
            m_AllocatedProfileSamples = ProfilerLogUtil.ReadInt(stream);
        }
    }
}