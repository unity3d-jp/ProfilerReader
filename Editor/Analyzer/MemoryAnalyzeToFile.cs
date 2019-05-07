using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;

using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader.Analyzer
{

    public class MemoryAnalyzeToFile : IAnalyzeFileWriter
    {
        private List<int> frameIdxList = new List<int>();
        private List<MemoryStats> memoryStatsList = new List<MemoryStats>();

        public void CollectData(ProfilerFrameData frameData)
        {
            if( frameData == null || frameData.allStats == null || frameData.allStats.memoryStats == null)
            {
                return;
            }
            frameIdxList.Add(frameData.frameIndex);
            memoryStatsList.Add( frameData.allStats.memoryStats );
        }

        /// <summary>
        /// 結果書き出し
        /// </summary>
        public void WriteResultFile(string path)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024 * 1024);

            AppendHeaderToStringBuilder(sb);

            for ( int i = 0; i < memoryStatsList.Count; ++i)
            {
                MemoryStats memoryStats = memoryStatsList[i];
                sb.Append(frameIdxList[i]).Append(",");
                sb.Append(",");
                // Used Memory
                sb.Append(memoryStats.bytesUsedTotal).Append(",");
                sb.Append(memoryStats.bytesUsedUnity).Append(",");
                sb.Append(memoryStats.bytesUsedMono).Append(",");
                sb.Append(memoryStats.bytesUsedGFX).Append(",");
                sb.Append(memoryStats.bytesUsedFMOD).Append(",");
                sb.Append(memoryStats.bytesUsedVideo).Append(",");
                sb.Append(memoryStats.bytesUsedProfiler).Append(",");
                sb.Append(",");

                // Reserved Memory
                sb.Append(memoryStats.bytesReservedTotal).Append(",");
                sb.Append(memoryStats.bytesReservedUnity).Append(",");
                sb.Append(memoryStats.bytesReservedMono).Append(",");
                sb.Append(memoryStats.bytesReservedFMOD).Append(",");
                sb.Append(memoryStats.bytesReservedVideo).Append(",");
                sb.Append(memoryStats.bytesReservedProfiler).Append(",");
                sb.Append(",");

                // by Assets
                sb.Append(memoryStats.textureCount).Append(",");
                sb.Append(memoryStats.textureBytes).Append(",");
                sb.Append(memoryStats.meshCount).Append(",");
                sb.Append(memoryStats.meshBytes).Append(",");
                sb.Append(memoryStats.materialCount).Append(",");
                sb.Append(memoryStats.materialBytes).Append(",");
                sb.Append(memoryStats.audioCount).Append(",");
                sb.Append(memoryStats.audioBytes).Append(",");
                sb.Append(memoryStats.assetCount).Append(",");
                sb.Append(memoryStats.gameObjectCount).Append(",");
                sb.Append(memoryStats.sceneObjectCount).Append(",");
                sb.Append(memoryStats.totalObjectsCount).Append(",");
                sb.Append(",");

                // GC
                sb.Append(memoryStats.frameGCAllocCount).Append(",");
                sb.Append(memoryStats.frameGCAllocBytes).Append(",");
                sb.Append("\n");
            }

            System.IO.File.WriteAllText(path, sb.ToString());
        }
        private void AppendHeaderToStringBuilder( System.Text.StringBuilder sb)
        {
            sb.Append("frameIdx").Append(",");
            sb.Append("UsedMemory,");
            // Used Memory
            sb.Append("bytesUsedTotal").Append(",");
            sb.Append("bytesUsedUnity").Append(",");
            sb.Append("bytesUsedMono").Append(",");
            sb.Append("bytesUsedGFX").Append(",");
            sb.Append("bytesUsedFMOD").Append(",");
            sb.Append("bytesUsedVideo").Append(",");
            sb.Append("bytesUsedProfiler").Append(",");

            sb.Append("ReservedMemory,");
            // Reserved Memory
            sb.Append("bytesReservedTotal").Append(",");
            sb.Append("bytesReservedUnity").Append(",");
            sb.Append("bytesReservedMono").Append(",");
            sb.Append("bytesReservedFMOD").Append(",");
            sb.Append("bytesReservedVideo").Append(",");
            sb.Append("bytesReservedProfiler").Append(",");

            sb.Append("AssetUsage,");
            // by Assets
            sb.Append("textureCount").Append(",");
            sb.Append("textureBytes").Append(",");
            sb.Append("meshCount").Append(",");
            sb.Append("meshBytes").Append(",");
            sb.Append("meshCount").Append(",");
            sb.Append("meshBytes").Append(",");
            sb.Append("materialCount").Append(",");
            sb.Append("materialBytes").Append(",");
            sb.Append("audioCount").Append(",");
            sb.Append("audioBytes").Append(",");
            sb.Append("assetCount").Append(",");
            sb.Append("gameObjectCount").Append(",");
            sb.Append("sceneObjectCount").Append(",");
            sb.Append("totalObjectsCount").Append(",");
            sb.Append("GC,");

            // GC
            sb.Append("frameGCAllocCount").Append(",");
            sb.Append("frameGCAllocBytes").Append(",");
            sb.Append("\n");

        }

        public string ConvertPath(string originPath)
        {
            var path = originPath.Replace(".", "_") + "_memory.csv";
            return path;
        }
    }
}
