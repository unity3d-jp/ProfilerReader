
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class VideoStats
    {

        internal void Read_0x20170130(System.IO.Stream stream)
        {
            totalVideoSourceCount = ProfilerLogUtil.ReadInt(stream);
            playingSources = ProfilerLogUtil.ReadInt(stream);
            swPlayingSources = ProfilerLogUtil.ReadInt(stream);
            pausedSources = ProfilerLogUtil.ReadInt(stream);
            videoClipCount = ProfilerLogUtil.ReadInt(stream);

            for (int i = 0; i < kMaxPlatformDependentStats; ++i)
            {
                this.platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }

        }
    }
}