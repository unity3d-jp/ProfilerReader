
using System.Collections;
namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class DrawStats
    {
        const int kMaxPlatformDependentStats = 16;
        [StatsData(0)]
        public int setPassCalls;
        [StatsData(1)]
        public int batches;
        [StatsData(2)]
        public int drawCalls;
        [StatsData(3)]
        public int triangles;
        [StatsData(4)]
        public int vertices;

        [StatsData(5)]
        public int dynamicBatchedDrawCalls;
        [StatsData(6)]
        public int dynamicBatches;
        [StatsData(7)]
        public int dynamicBatchedTriangles;
        [StatsData(8)]
        public int dynamicBatchedVertices;

        [StatsData(9)]
        public int staticBatchedDrawCalls;
        [StatsData(10)]
        public int staticBatches;
        [StatsData(11)]
        public int staticBatchedTriangles;
        [StatsData(12)]
        public int staticBatchedVertices;

        [StatsData(13)]
        public bool hasInstancing;
        [StatsData(14)]
        public int instancedBatchedDrawCalls;
        [StatsData(15)]
        public int instancedBatches;
        [StatsData(16)]
        public int instancedTriangles;
        [StatsData(17)]
        public int instancedVertices;

        [StatsData(18)]
        public int shadowCasters;

        [StatsData(19)]
        public int usedTextureCount;
        [StatsData(20)]
        public int usedTextureBytes;

        [StatsData(21)]
        public int renderTextureCount;
        [StatsData(22)]
        public int renderTextureBytes;
        [StatsData(23)]
        public int renderTextureStateChanges;

        [StatsData(24)]
        public int screenWidth;
        [StatsData(25)]
        public int screenHeight;
        [StatsData(26,0,ProfilerDataStreamVersion.Unity55)]
        public int screenFSAA;
        [StatsData(27)]
        public int screenBytes;

        [StatsData(28)]
        public int vboTotal;
        [StatsData(29)]
        public int vboTotalBytes;
        [StatsData(30)]
        public int vboUploads;
        [StatsData(31)]
        public int vboUploadBytes;
        [StatsData(32)]
        public int ibUploads;
        [StatsData(33)]
        public int ibUploadBytes;

        [StatsData(34)]
        public int visibleSkinnedMeshes;

        [StatsData(35)]
        public int totalAvailableVRamMBytes;

        [StatsData(36)]
        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];

    }
}