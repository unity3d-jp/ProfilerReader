
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AllProfilerStats
    {

        internal void Read_0x20170929(System.IO.Stream stream)
        {
            var reader = new StatsDataReader(stream);
            uint version = ProfilerDataStreamVersion.Unity2017_2;
            
            this.memoryStats = ReadMemoryStats(reader, version);

            this.drawStats = StatsDeserializeManager<DrawStats>.Instance.GetDeserializer(version).ReadObject(reader);

            this.physicsStats = StatsDeserializeManager<PhysicsStats>.Instance.GetDeserializer(version).ReadObject(reader);
            this.physics2DStats = StatsDeserializeManager<Physics2DStats>.Instance.GetDeserializer(version).ReadObject(reader);

            this.networkOperationStats = StatsDeserializeManager<NetworkOperationStats>.Instance.GetDeserializer(version).ReadObject(reader);
            this.networkMessageStats = StatsDeserializeManager<NetworkMessageStats>.Instance.GetDeserializer(version).ReadObject(reader);

            this.debugStats = StatsDeserializeManager<DebugStats>.Instance.GetDeserializer(version).ReadObject(reader);
            this.audioStats = StatsDeserializeManager<AudioStats>.Instance.GetDeserializer(version).ReadObject(reader);

            this.videoStats = StatsDeserializeManager<VideoStats>.Instance.GetDeserializer(version).ReadObject(reader);
            this.chartSample = StatsDeserializeManager<ChartSample>.Instance.GetDeserializer(version).ReadObject(reader);
            this.chartSampleSelected = StatsDeserializeManager<ChartSample>.Instance.GetDeserializer(version).ReadObject(reader);


            this.globalIlluminationStats = StatsDeserializeManager<GlobalIlluminationStats>.Instance.GetDeserializer(version).ReadObject(reader);
            
        }
    }
}