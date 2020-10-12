using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;

using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader.Analyzer
{

    public class RenderingAnalyzeToFile : AnalyzeToTextbaseFileBase
    {

        const string FrameWholeDataSpecialKey = "CPUTotal";

        private List<int> frameIdxList = new List<int>();
        private List<DrawStats> drawStatsList = new List<DrawStats>();

        public override void CollectData(ProfilerFrameData frameData)
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
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            AppendHeaderToStringBuilder(csvStringGenerator);

            for (int i = 0; i < drawStatsList.Count; ++i)
            {
                DrawStats drawStats = drawStatsList[i];
                csvStringGenerator.AppendColumn(frameIdxList[i]);

                csvStringGenerator.AppendColumn("");
                // Total
                csvStringGenerator.AppendColumn(drawStats.setPassCalls);
                csvStringGenerator.AppendColumn(drawStats.drawCalls);
                csvStringGenerator.AppendColumn(drawStats.batches);
                csvStringGenerator.AppendColumn(drawStats.triangles);
                csvStringGenerator.AppendColumn(drawStats.vertices);
                csvStringGenerator.AppendColumn("");
                // DynamicBatching
                csvStringGenerator.AppendColumn(drawStats.dynamicBatchedDrawCalls);
                csvStringGenerator.AppendColumn(drawStats.dynamicBatchedTriangles);
                csvStringGenerator.AppendColumn(drawStats.dynamicBatchedTriangles);
                csvStringGenerator.AppendColumn(drawStats.dynamicBatchedVertices);
                csvStringGenerator.AppendColumn("");
                // static batching
                csvStringGenerator.AppendColumn(drawStats.staticBatchedDrawCalls);
                csvStringGenerator.AppendColumn(drawStats.staticBatchedTriangles);
                csvStringGenerator.AppendColumn(drawStats.staticBatchedTriangles);
                csvStringGenerator.AppendColumn(drawStats.staticBatchedVertices);
                // instancing    
                csvStringGenerator.AppendColumn("");
                csvStringGenerator.AppendColumn(drawStats.hasInstancing);
                csvStringGenerator.AppendColumn(drawStats.instancedBatchedDrawCalls);
                csvStringGenerator.AppendColumn(drawStats.instancedBatches);
                csvStringGenerator.AppendColumn(drawStats.instancedTriangles);
                csvStringGenerator.AppendColumn(drawStats.instancedVertices);
                //screen Info
                csvStringGenerator.AppendColumn("");
                csvStringGenerator.AppendColumn(drawStats.screenWidth);
                csvStringGenerator.AppendColumn(drawStats.screenHeight);
                csvStringGenerator.AppendColumn(drawStats.screenBytes);
                // RenderTexture Info
                csvStringGenerator.AppendColumn("");
                csvStringGenerator.AppendColumn(drawStats.renderTextureCount);
                csvStringGenerator.AppendColumn(drawStats.renderTextureBytes);
                csvStringGenerator.AppendColumn(drawStats.renderTextureStateChanges);
                // SkinnedMesh
                csvStringGenerator.AppendColumn("");
                csvStringGenerator.AppendColumn(drawStats.visibleSkinnedMeshes);
                // etc...
                csvStringGenerator.AppendColumn("");
                csvStringGenerator.AppendColumn(drawStats.totalAvailableVRamMBytes);
                csvStringGenerator.AppendColumn(drawStats.vboTotal);
                csvStringGenerator.AppendColumn(drawStats.vboUploads);
                csvStringGenerator.AppendColumn(drawStats.ibUploads);
                csvStringGenerator.AppendColumn(drawStats.shadowCasters);
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }
        private void AppendHeaderToStringBuilder(CsvStringGenerator csvStringGenerator)
        {
            csvStringGenerator.AppendColumn("frameIdx");
            // Total
            csvStringGenerator.AppendColumn("Total");
            csvStringGenerator.AppendColumn("setPassCalls");
            csvStringGenerator.AppendColumn("drawCalls");
            csvStringGenerator.AppendColumn("batches");
            csvStringGenerator.AppendColumn("triangles");
            csvStringGenerator.AppendColumn("vertices");
            // DynamicBatching
            csvStringGenerator.AppendColumn("DynamicBatching");
            csvStringGenerator.AppendColumn("dynamicBatchedDrawCalls");
            csvStringGenerator.AppendColumn("dynamicBatchedTriangles");
            csvStringGenerator.AppendColumn("dynamicBatchedTriangles");
            csvStringGenerator.AppendColumn("dynamicBatchedVertices");
            // static batching
            csvStringGenerator.AppendColumn("StaticBatching");
            csvStringGenerator.AppendColumn("staticBatchedDrawCalls");
            csvStringGenerator.AppendColumn("staticBatchedTriangles");
            csvStringGenerator.AppendColumn("staticBatchedTriangles");
            csvStringGenerator.AppendColumn("staticBatchedVertices");
            // instancing    
            csvStringGenerator.AppendColumn("Instancing");
            csvStringGenerator.AppendColumn("hasInstancing");
            csvStringGenerator.AppendColumn("instancedBatchedDrawCalls");
            csvStringGenerator.AppendColumn("instancedBatches");
            csvStringGenerator.AppendColumn("instancedTriangles");
            csvStringGenerator.AppendColumn("instancedVertices");
            //screen Info
            csvStringGenerator.AppendColumn("ScreenInfo");
            csvStringGenerator.AppendColumn("screenWidth");
            csvStringGenerator.AppendColumn("screenHeight");
            csvStringGenerator.AppendColumn("screenBytes");
            // RenderTexture Info
            csvStringGenerator.AppendColumn("RenderTextureInfo");
            csvStringGenerator.AppendColumn("renderTextureCount");
            csvStringGenerator.AppendColumn("renderTextureBytes");
            csvStringGenerator.AppendColumn("renderTextureStateChanges");
            // SkinnedMesh
            csvStringGenerator.AppendColumn("SkinnedMesh");
            csvStringGenerator.AppendColumn("visibleSkinnedMeshes");
            // etc...
            csvStringGenerator.AppendColumn("etc");
            csvStringGenerator.AppendColumn("totalAvailableVRamMBytes");
            csvStringGenerator.AppendColumn("vboTotal");
            csvStringGenerator.AppendColumn("vboUploads");
            csvStringGenerator.AppendColumn("ibUploads");
            csvStringGenerator.AppendColumn("shadowCasters");
            csvStringGenerator.NextRow();

        }

        protected override string FooterName
        {
            get
            {
                return "_rendering.csv";
            }
        }

    }
}
