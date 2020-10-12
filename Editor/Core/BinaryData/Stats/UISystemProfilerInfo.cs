using System.IO;
namespace UTJ.ProfilerReader.BinaryData.Stats
{
    // from 2017.1
    public class UISystemProfilerInfo
    {

        public enum BatchBreakingReason
        {
            NoBreaking,
            NotCoplanarWithCanvas = 1,
            CanvasInjectionIndex = 2,
            DifferentMaterialInstance = 4,
            DifferentRectClipping = 8,
            DifferentTexture = 16,
            DifferentA8TextureUsage = 32,
            DifferentClipRect = 64,
            Unknown = 128,
        }
        
        public int   objectInstanceId{get;private set;}
        public int objectNameOffset { get; set; }
        public int parentId { get; set; }
        public int batchCount { get; set; }
        public int totalBatchCount { get; set; }
        public int vertexCount { get; set; }
        public int totalVertexCount { get; set; }
        public bool isBatch { get; set; }
        public BatchBreakingReason batchBreakingReason { get; set; }
        public int instanceIDsIndex { get; set; }
        public int instanceIDsCount { get; set; }
        public int renderDataIndex { get; set; }
        public int renderDataCount { get; set; }

        public void Read(Stream stream)
        {
            objectInstanceId = ProfilerLogUtil.ReadInt(stream);
            objectNameOffset = ProfilerLogUtil.ReadInt(stream);
            parentId = ProfilerLogUtil.ReadInt(stream);
            batchCount = ProfilerLogUtil.ReadInt(stream);
            totalBatchCount = ProfilerLogUtil.ReadInt(stream);
            vertexCount = ProfilerLogUtil.ReadInt(stream);
            totalVertexCount = ProfilerLogUtil.ReadInt(stream);

            isBatch = (ProfilerLogUtil.ReadInt(stream) == 0);
            batchBreakingReason = (BatchBreakingReason)ProfilerLogUtil.ReadInt(stream);
            instanceIDsIndex = ProfilerLogUtil.ReadInt(stream);
            instanceIDsCount = ProfilerLogUtil.ReadInt(stream);
            renderDataIndex = ProfilerLogUtil.ReadInt(stream);
            renderDataCount = ProfilerLogUtil.ReadInt(stream);
        }
    }
}