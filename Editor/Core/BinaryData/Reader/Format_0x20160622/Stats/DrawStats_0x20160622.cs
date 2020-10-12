
using System.Collections;
namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class DrawStats
    {

        internal void Read_0x20160622(System.IO.Stream stream)
        {
            this.setPassCalls = ProfilerLogUtil.ReadInt(stream);
            this.batches = ProfilerLogUtil.ReadInt(stream);
            this.drawCalls = ProfilerLogUtil.ReadInt(stream);
            this.triangles = ProfilerLogUtil.ReadInt(stream);
            this.vertices = ProfilerLogUtil.ReadInt(stream);

            this.dynamicBatchedDrawCalls = ProfilerLogUtil.ReadInt(stream);
            this.dynamicBatches = ProfilerLogUtil.ReadInt(stream);
            this.dynamicBatchedTriangles = ProfilerLogUtil.ReadInt(stream);
            this.dynamicBatchedVertices = ProfilerLogUtil.ReadInt(stream);

            this.staticBatchedDrawCalls = ProfilerLogUtil.ReadInt(stream);
            this.staticBatches = ProfilerLogUtil.ReadInt(stream);
            this.staticBatchedTriangles = ProfilerLogUtil.ReadInt(stream);
            this.staticBatchedVertices = ProfilerLogUtil.ReadInt(stream);

            this.hasInstancing = (ProfilerLogUtil.ReadInt(stream) != 0);
            this.instancedBatchedDrawCalls = ProfilerLogUtil.ReadInt(stream);
            this.instancedBatches = ProfilerLogUtil.ReadInt(stream);
            this.instancedTriangles = ProfilerLogUtil.ReadInt(stream);
            this.instancedVertices = ProfilerLogUtil.ReadInt(stream);

            this.shadowCasters = ProfilerLogUtil.ReadInt(stream);

            this.usedTextureCount = ProfilerLogUtil.ReadInt(stream);
            this.usedTextureBytes = ProfilerLogUtil.ReadInt(stream);

            this.renderTextureCount = ProfilerLogUtil.ReadInt(stream);
            this.renderTextureBytes = ProfilerLogUtil.ReadInt(stream);
            this.renderTextureStateChanges = ProfilerLogUtil.ReadInt(stream);

            this.screenWidth = ProfilerLogUtil.ReadInt(stream);
            this.screenHeight = ProfilerLogUtil.ReadInt(stream);
            this.screenFSAA = ProfilerLogUtil.ReadInt(stream);
            this.screenBytes = ProfilerLogUtil.ReadInt(stream);

            this.vboTotal = ProfilerLogUtil.ReadInt(stream);
            this.vboTotalBytes = ProfilerLogUtil.ReadInt(stream);
            this.vboUploads = ProfilerLogUtil.ReadInt(stream);
            this.vboUploadBytes = ProfilerLogUtil.ReadInt(stream);
            this.ibUploads = ProfilerLogUtil.ReadInt(stream);
            this.ibUploadBytes = ProfilerLogUtil.ReadInt(stream);

            this.visibleSkinnedMeshes = ProfilerLogUtil.ReadInt(stream);

            this.totalAvailableVRamMBytes = ProfilerLogUtil.ReadInt(stream);


            for (int i = 0; i < kMaxPlatformDependentStats; ++i) {
                platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }

        }
    }
}