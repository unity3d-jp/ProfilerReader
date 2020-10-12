
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class MemoryStats
    {
        public const int kMaxPlatformDependentStats = 16;

        // used bytes: Total, unity(-profiler), mono, DX/OGL, Profiler, FMOD??, Executable??
        // reserved bytes: Total, unity, mono, DX/OGL, Profiler, FMOD??, Executable??
        [StatsData(0)]
        public int bytesUsedTotal;
        [StatsData(1)]
        public int bytesUsedUnity;
        [StatsData(2)]
        public int bytesUsedMono;
        [StatsData(3)]
        public int bytesUsedGFX;
        [StatsData(4)]
        public int bytesUsedFMOD;
        [StatsData(5,ProfilerDataStreamVersion.Unity56)]
        public int bytesUsedVideo; // from 5.6
        [StatsData(6)]
        public int bytesUsedProfiler;

        [StatsData(7)]
        public int bytesReservedTotal;
        [StatsData(8)]
        public int bytesReservedUnity;
        [StatsData(9)]
        public int bytesReservedMono;
        [StatsData(10)]
        public int bytesReservedGFX;
        [StatsData(11)]
        public int bytesReservedFMOD;
        [StatsData(12,ProfilerDataStreamVersion.Unity56)]
        public int bytesReservedVideo; // from 5.6
        [StatsData(13)]
        public int bytesReservedProfiler;
        [StatsData(14)]
        public int bytesVirtual;

        [StatsData(15)]
        public int textureCount;
        [StatsData(16)]
        public int textureBytes;

        [StatsData(17)]
        public int meshCount;
        [StatsData(18)]
        public int meshBytes;

        [StatsData(19)]
        public int materialCount;
        [StatsData(20)]
        public int materialBytes;

        [StatsData(21)]
        public int animationClipCount;
        [StatsData(22)]
        public int animationClipBytes;

        [StatsData(23)]
        public int audioCount;
        [StatsData(24)]
        public int audioBytes;

        [StatsData(25)]
        public int assetCount;
        [StatsData(26)]
        public int sceneObjectCount;
        [StatsData(27)]
        public int gameObjectCount;

        [StatsData(28)]
        public int totalObjectsCount;
        [StatsData(29)]
        public int profilerMemUsed;
        [StatsData(30)]
        public int profilerNumAllocations;

        // Number of GC allocations for the specific frame
        [StatsData(31)]
        public int frameGCAllocCount;
        [StatsData(32)]
        public int frameGCAllocBytes;

        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];


        public void Scale()
        {
            bytesUsedTotal *= 1024;
            bytesUsedUnity *= 1024;
            bytesUsedMono *= 1024;
            bytesUsedGFX *= 1024;
            bytesUsedFMOD *= 1024;
            bytesUsedVideo *= 1024;
            bytesUsedProfiler *= 1024;

            bytesReservedTotal *= 1024;
            bytesReservedUnity *= 1024;
            bytesReservedMono *= 1024;
            bytesReservedGFX *= 1024;
            bytesReservedFMOD *= 1024;
            bytesReservedVideo *= 1024;
            bytesReservedProfiler *= 1024;

            bytesVirtual *= 1024;

            textureBytes *= 1024;
            meshBytes *= 1024;
            materialBytes *= 1024;
            animationClipBytes *= 1024;
            audioBytes *= 1024;

            profilerMemUsed *= 1024;

            frameGCAllocBytes *= 1024;
        }

    }
}