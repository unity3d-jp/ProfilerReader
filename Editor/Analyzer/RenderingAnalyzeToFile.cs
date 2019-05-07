using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;

using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader.Analyzer
{

    public class RenderingAnalyzeToFile : IAnalyzeFileWriter
    {

        const string FrameWholeDataSpecialKey = "CPUTotal";

        private List<int> frameIdxList = new List<int>();
        private List<DrawStats> drawStatsList = new List<DrawStats>();

        public void CollectData(ProfilerFrameData frameData)
        {
            if( frameData == null || frameData.allStats == null || frameData.allStats.drawStats == null)
            {
                return;
            }
            frameIdxList.Add(frameData.frameIndex);
            drawStatsList.Add(frameData.allStats.drawStats);
        }

        /// <summary>
        /// 結果書き出し
        /// </summary>
        public void WriteResultFile(string path)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024 * 1024);
            AppendHeaderToStringBuilder(sb);

            for (int i = 0; i < drawStatsList.Count; ++i)
            {
                DrawStats drawStats = drawStatsList[i];
                sb.Append(frameIdxList[i]).Append(",");

                sb.Append(",");
                // Total
                sb.Append(drawStats.setPassCalls).Append(",");
                sb.Append(drawStats.drawCalls).Append(",");
                sb.Append(drawStats.batches).Append(",");
                sb.Append(drawStats.triangles).Append(",");
                sb.Append(drawStats.vertices).Append(",");
                sb.Append(",");
                // DynamicBatching
                sb.Append(drawStats.dynamicBatchedDrawCalls).Append(",");
                sb.Append(drawStats.dynamicBatchedTriangles).Append(",");
                sb.Append(drawStats.dynamicBatchedTriangles).Append(",");
                sb.Append(drawStats.dynamicBatchedVertices).Append(",");
                sb.Append(",");
                // static batching
                sb.Append(drawStats.staticBatchedDrawCalls).Append(",");
                sb.Append(drawStats.staticBatchedTriangles).Append(",");
                sb.Append(drawStats.staticBatchedTriangles).Append(",");
                sb.Append(drawStats.staticBatchedVertices).Append(",");
                // instancing    
                sb.Append(",");
                sb.Append(drawStats.hasInstancing).Append(",");
                sb.Append(drawStats.instancedBatchedDrawCalls).Append(",");
                sb.Append(drawStats.instancedBatches).Append(",");
                sb.Append(drawStats.instancedTriangles).Append(",");
                sb.Append(drawStats.instancedVertices).Append(",");
                //screen Info
                sb.Append(",");
                sb.Append(drawStats.screenWidth).Append(",");
                sb.Append(drawStats.screenHeight).Append(",");
                sb.Append(drawStats.screenBytes).Append(",");
                // RenderTexture Info
                sb.Append(",");
                sb.Append(drawStats.renderTextureCount).Append(",");
                sb.Append(drawStats.renderTextureBytes).Append(",");
                sb.Append(drawStats.renderTextureStateChanges).Append(",");
                // SkinnedMesh
                sb.Append(",");
                sb.Append(drawStats.visibleSkinnedMeshes).Append(",");
                // etc...
                sb.Append(",");
                sb.Append(drawStats.totalAvailableVRamMBytes).Append(",");
                sb.Append(drawStats.vboTotal).Append(",");
                sb.Append(drawStats.vboUploads).Append(",");
                sb.Append(drawStats.ibUploads).Append(",");
                sb.Append(drawStats.shadowCasters).Append(",");
                sb.Append("\n");
            }

            System.IO.File.WriteAllText(path, sb.ToString());
        }
        private void AppendHeaderToStringBuilder(System.Text.StringBuilder sb)
        {
            sb.Append("frameIdx").Append(",");
            // Total
            sb.Append("Total,");
            sb.Append("setPassCalls").Append(",");
            sb.Append("drawCalls").Append(",");
            sb.Append("batches").Append(",");
            sb.Append("triangles").Append(",");
            sb.Append("vertices").Append(",");
            // DynamicBatching
            sb.Append("DynamicBatching,");
            sb.Append("dynamicBatchedDrawCalls").Append(",");
            sb.Append("dynamicBatchedTriangles").Append(",");
            sb.Append("dynamicBatchedTriangles").Append(",");
            sb.Append("dynamicBatchedVertices").Append(",");
            // static batching
            sb.Append("StaticBatching,");
            sb.Append("staticBatchedDrawCalls").Append(",");
            sb.Append("staticBatchedTriangles").Append(",");
            sb.Append("staticBatchedTriangles").Append(",");
            sb.Append("staticBatchedVertices").Append(",");
            // instancing    
            sb.Append("Instancing,");
            sb.Append("hasInstancing").Append(",");
            sb.Append("instancedBatchedDrawCalls").Append(",");
            sb.Append("instancedBatches").Append(",");
            sb.Append("instancedTriangles").Append(",");
            sb.Append("instancedVertices").Append(",");
            //screen Info
            sb.Append("ScreenInfo,");
            sb.Append("screenWidth").Append(",");
            sb.Append("screenHeight").Append(",");
            sb.Append("screenBytes").Append(",");
            // RenderTexture Info
            sb.Append("RenderTextureInfo,");
            sb.Append("renderTextureCount").Append(",");
            sb.Append("renderTextureBytes").Append(",");
            sb.Append("renderTextureStateChanges").Append(",");
            // SkinnedMesh
            sb.Append("SkinnedMesh,");
            sb.Append("visibleSkinnedMeshes").Append(",");
            // etc...
            sb.Append("etc,");
            sb.Append("totalAvailableVRamMBytes").Append(",");
            sb.Append("vboTotal").Append(",");
            sb.Append("vboUploads").Append(",");
            sb.Append("ibUploads").Append(",");
            sb.Append("shadowCasters").Append(",");
            sb.Append("\n");

        }

        public string ConvertPath(string originPath)
        {
            var path = originPath.Replace(".", "_") + "_rendering.csv";
            return path;
        }
    }
}
