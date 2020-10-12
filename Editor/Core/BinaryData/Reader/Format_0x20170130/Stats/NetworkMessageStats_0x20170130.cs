
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class NetworkMessageStats
    {
        internal void Read_0x20170130(System.IO.Stream stream)
        {
            bytesIn = ProfilerLogUtil.ReadInt(stream);
            bytesOut = ProfilerLogUtil.ReadInt(stream);

            packetsIn = ProfilerLogUtil.ReadInt(stream);
            HLAPIMsgsIn = ProfilerLogUtil.ReadInt(stream);
            LLAPIMsgsIn = ProfilerLogUtil.ReadInt(stream);

            packetsOut = ProfilerLogUtil.ReadInt(stream);
            HLAPIMsgsOut = ProfilerLogUtil.ReadInt(stream);
            LLAPIMsgsOut = ProfilerLogUtil.ReadInt(stream);

            numberConnections = ProfilerLogUtil.ReadInt(stream);
            rtt = ProfilerLogUtil.ReadInt(stream);
            HLAPIResends = ProfilerLogUtil.ReadInt(stream);
            HLAPIPending = ProfilerLogUtil.ReadInt(stream);

            for (int i = 0; i < kMaxPlatformDependentStats; ++i)
            {
                this.platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }
        }
    }
}