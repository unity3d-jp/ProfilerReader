
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AllProfilerStats
    {
        public MemoryStats memoryStats { get; set; }
        public DrawStats drawStats { get; set; }
        public PhysicsStats physicsStats { get; set; }
        public Physics2DStats physics2DStats { get; set; }
        public NetworkOperationStats networkOperationStats { get; set; }
        public NetworkMessageStats networkMessageStats { get; set; }
        public DebugStats debugStats { get; set; }
        public AudioStats audioStats { get; set; }
        public VideoStats videoStats { get; set; } // from 5.6
        public ChartSample chartSample;
        public ChartSample chartSampleSelected;
        public UIStats uiStats { get; set; } // from 2017.1
        public GlobalIlluminationStats globalIlluminationStats { get; set; } // from 2017.2



        MemoryStats ReadMemoryStats(StatsDataReader reader, uint version)
        {
            MemoryStats memoryStats = StatsDeserializeManager<MemoryStats>.Instance.GetDeserializer(version).ReadObject(reader);
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