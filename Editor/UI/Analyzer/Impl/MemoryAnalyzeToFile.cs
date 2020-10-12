using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;

using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader.Analyzer
{

    public class MemoryAnalyzeToFile : AnalyzeToTextbaseFileBase
    {
        private List<int> frameIdxList = new List<int>();
        private List<MemoryStats> memoryStatsList = new List<MemoryStats>();

        public override void CollectData(ProfilerFrameData frameData)
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
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();

            AppendHeaderToStringBuilder(csvStringGenerator);

            for ( int i = 0; i < memoryStatsList.Count; ++i)
            {
                MemoryStats memoryStats = memoryStatsList[i];
                csvStringGenerator.AppendColumn(frameIdxList[i]);
                csvStringGenerator.AppendColumn("");
                // Used Memory
                csvStringGenerator.AppendColumn(memoryStats.bytesUsedTotal);
                csvStringGenerator.AppendColumn(memoryStats.bytesUsedUnity);
                csvStringGenerator.AppendColumn(memoryStats.bytesUsedMono);
                csvStringGenerator.AppendColumn(memoryStats.bytesUsedGFX);
                csvStringGenerator.AppendColumn(memoryStats.bytesUsedFMOD);
                csvStringGenerator.AppendColumn(memoryStats.bytesUsedVideo);
                csvStringGenerator.AppendColumn(memoryStats.bytesUsedProfiler);
                csvStringGenerator.AppendColumn("");

                // Reserved Memory
                csvStringGenerator.AppendColumn(memoryStats.bytesReservedTotal);
                csvStringGenerator.AppendColumn(memoryStats.bytesReservedUnity);
                csvStringGenerator.AppendColumn(memoryStats.bytesReservedMono);
                csvStringGenerator.AppendColumn(memoryStats.bytesReservedFMOD);
                csvStringGenerator.AppendColumn(memoryStats.bytesReservedVideo);
                csvStringGenerator.AppendColumn(memoryStats.bytesReservedProfiler);
                csvStringGenerator.AppendColumn("");

                // by Assets
                csvStringGenerator.AppendColumn(memoryStats.textureCount);
                csvStringGenerator.AppendColumn(memoryStats.textureBytes);
                csvStringGenerator.AppendColumn(memoryStats.meshCount);
                csvStringGenerator.AppendColumn(memoryStats.meshBytes);
                csvStringGenerator.AppendColumn(memoryStats.materialCount);
                csvStringGenerator.AppendColumn(memoryStats.materialBytes);
                csvStringGenerator.AppendColumn(memoryStats.audioCount);
                csvStringGenerator.AppendColumn(memoryStats.audioBytes);
                csvStringGenerator.AppendColumn(memoryStats.assetCount);
                csvStringGenerator.AppendColumn(memoryStats.gameObjectCount);
                csvStringGenerator.AppendColumn(memoryStats.sceneObjectCount);
                csvStringGenerator.AppendColumn(memoryStats.totalObjectsCount);
                csvStringGenerator.AppendColumn("");

                // GC
                csvStringGenerator.AppendColumn(memoryStats.frameGCAllocCount);
                csvStringGenerator.AppendColumn(memoryStats.frameGCAllocBytes);
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }
        private void AppendHeaderToStringBuilder(CsvStringGenerator csvStringGenerator)
        {
            csvStringGenerator.AppendColumn("frameIdx");
            csvStringGenerator.AppendColumn("UsedMemory");
            // Used Memory
            csvStringGenerator.AppendColumn("bytesUsedTotal");
            csvStringGenerator.AppendColumn("bytesUsedUnity");
            csvStringGenerator.AppendColumn("bytesUsedMono");
            csvStringGenerator.AppendColumn("bytesUsedGFX");
            csvStringGenerator.AppendColumn("bytesUsedFMOD");
            csvStringGenerator.AppendColumn("bytesUsedVideo");
            csvStringGenerator.AppendColumn("bytesUsedProfiler");

            csvStringGenerator.AppendColumn("ReservedMemory");
            // Reserved Memory
            csvStringGenerator.AppendColumn("bytesReservedTotal");
            csvStringGenerator.AppendColumn("bytesReservedUnity");
            csvStringGenerator.AppendColumn("bytesReservedMono");
            csvStringGenerator.AppendColumn("bytesReservedFMOD");
            csvStringGenerator.AppendColumn("bytesReservedVideo");
            csvStringGenerator.AppendColumn("bytesReservedProfiler");

            csvStringGenerator.AppendColumn("AssetUsage");
            // by Assets
            csvStringGenerator.AppendColumn("textureCount");
            csvStringGenerator.AppendColumn("textureBytes");
            csvStringGenerator.AppendColumn("meshCount");
            csvStringGenerator.AppendColumn("meshBytes");
            csvStringGenerator.AppendColumn("meshCount");
            csvStringGenerator.AppendColumn("meshBytes");
            csvStringGenerator.AppendColumn("materialCount");
            csvStringGenerator.AppendColumn("materialBytes");
            csvStringGenerator.AppendColumn("audioCount");
            csvStringGenerator.AppendColumn("audioBytes");
            csvStringGenerator.AppendColumn("assetCount");
            csvStringGenerator.AppendColumn("gameObjectCount");
            csvStringGenerator.AppendColumn("sceneObjectCount");
            csvStringGenerator.AppendColumn("totalObjectsCount");
            csvStringGenerator.AppendColumn("GC");

            // GC
            csvStringGenerator.AppendColumn("frameGCAllocCount");
            csvStringGenerator.AppendColumn("frameGCAllocBytes");
            csvStringGenerator.NextRow();

        }
        
        protected override string FooterName
        {
            get
            {
                return "_memory.csv";
            }
        }

    }
}
