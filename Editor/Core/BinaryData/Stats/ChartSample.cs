
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class ChartSample
    {
        // All kFormatTime values are stored and transferred as ints, but are floats!
        [StatsData(0)]
        public float rendering;
        [StatsData(1)]
        public float scripts;
        [StatsData(2)]
        public float physics;

        [StatsData(3, ProfilerDataStreamVersion.Unity2018_2)]
        public float animation;

        [StatsData(4)]
        public float gc;
        [StatsData(5)]
        public float vsync;
        [StatsData(6)]
        public float gi;
        [StatsData(7,ProfilerDataStreamVersion.Unity55,ProfilerDataStreamVersion.Unity56)]
        public float nativeMem; // available 5.5 - 5.6
        [StatsData(8, ProfilerDataStreamVersion.Unity2017_1)]
        public float UI;// from 2017.1

        [StatsData(9)]
        public float others;

        [StatsData(10)]
        public float gpuOpaque;
        [StatsData(11)]
        public float gpuTransparent;
        [StatsData(12)]
        public float gpuShadows;
        [StatsData(13)]
        public float gpuPostProcess;
        [StatsData(14)]
        public float gpuDeferredPrePass;
        [StatsData(15)]
        public float gpuDeferredLighting;
        [StatsData(16)]
        public float gpuOther;

        [StatsData(17)]
        public int hasGPUProfiler;

        [StatsData(18,ProfilerDataStreamVersion.Unity2017_1)]
        public float uisystemLayouting;// from 2017.1
        [StatsData(19, ProfilerDataStreamVersion.Unity2017_1)]
        public float uisystemRendering;// from 2017.1
    }
}