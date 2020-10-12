using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.BinaryData.Stats
{
    // from 5.6
    public partial class VideoStats
    {
        public const int kMaxPlatformDependentStats = 8;

        [StatsData(0, ProfilerDataStreamVersion.Unity56)]
        public int totalVideoSourceCount;
        [StatsData(1, ProfilerDataStreamVersion.Unity56)]
        public int playingSources;
        [StatsData(2, ProfilerDataStreamVersion.Unity56)]
        public int swPlayingSources;

        [StatsData(3, ProfilerDataStreamVersion.Unity2018_1)]
        public int preBufferedFrames;
        [StatsData(4, ProfilerDataStreamVersion.Unity2018_1)]
        public int preBufferedFrameLimit;
        [StatsData(5, ProfilerDataStreamVersion.Unity2018_1)]
        public int framesDropped;

        [StatsData(6, ProfilerDataStreamVersion.Unity56)]
        public int pausedSources;
        [StatsData(7, ProfilerDataStreamVersion.Unity56)]
        public int videoClipCount;

        [StatsData(100, ProfilerDataStreamVersion.Unity56)]
        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];
    }
}