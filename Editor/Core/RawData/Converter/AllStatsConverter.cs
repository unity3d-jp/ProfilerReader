using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.RawData.Protocol;
using UTJ.ProfilerReader.BinaryData.Stats;

using RawAllProfilerStats = UTJ.ProfilerReader.RawData.Protocol.AllProfilerStats;
using BinAllProfilerStats = UTJ.ProfilerReader.BinaryData.Stats.AllProfilerStats;

namespace UTJ.ProfilerReader.RawData.Converter
{
    public class AllStatsConverter
    {

        public BinAllProfilerStats ConvertFromRaw(RawAllProfilerStats stats,uint version)
        {
            if( version == UTJ.ProfilerReader.BinaryData.ProfilerDataStreamVersion. Actual_Unity2017_3)
            {
                version = UTJ.ProfilerReader.BinaryData.ProfilerDataStreamVersion.Unity2017_3;
            }

            BinAllProfilerStats binStats = new BinAllProfilerStats();

            StatsReader reader = new StatsReader(stats.data);
            binStats.memoryStats = ReadMemoryStats(reader ,version);
            binStats.drawStats = StatsDeserializeManager<DrawStats>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.physicsStats = StatsDeserializeManager<PhysicsStats>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.physics2DStats = StatsDeserializeManager<Physics2DStats>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.networkOperationStats = StatsDeserializeManager<NetworkOperationStats>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.networkMessageStats = StatsDeserializeManager<NetworkMessageStats>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.debugStats = StatsDeserializeManager<DebugStats>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.audioStats = StatsDeserializeManager<AudioStats>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.videoStats = StatsDeserializeManager<VideoStats>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.chartSample = StatsDeserializeManager<ChartSample>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.chartSampleSelected = StatsDeserializeManager<ChartSample>.Instance.GetDeserializer(version).ReadObject(reader);
            binStats.globalIlluminationStats = StatsDeserializeManager<GlobalIlluminationStats>.Instance.GetDeserializer(version).ReadObject(reader);
            return binStats;
        }

        private MemoryStats ReadMemoryStats(StatsReader reader,uint version)
        {
            MemoryStats memoryStats =  StatsDeserializeManager<MemoryStats>.Instance.GetDeserializer(version).ReadObject(reader);
            memoryStats.Scale();
            int count = reader.ReadInt();
            int persistentTypeID = reader.ReadInt();

            while (persistentTypeID != -1)
            {
                count = reader.ReadInt();
                persistentTypeID = reader.ReadInt();
            }

            for (int i = 0; i < MemoryStats.kMaxPlatformDependentStats; i++)
            {
                memoryStats.platformDependentStats[i] = reader.ReadInt();
            }
            return memoryStats;
        }
        
    }

}
 