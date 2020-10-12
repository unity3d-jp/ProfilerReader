
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AllProfilerStats
    {

        internal void Read_0x20161226(System.IO.Stream stream)
        {
            this.memoryStats = new MemoryStats();
            this.drawStats = new DrawStats();
            this.physicsStats = new PhysicsStats();
            this.physics2DStats = new Physics2DStats();
            this.networkOperationStats = new NetworkOperationStats();
            this.networkMessageStats = new NetworkMessageStats();
            this.debugStats = new DebugStats();
            this.audioStats = new AudioStats();
            this.videoStats = new VideoStats();
            this.chartSample = new ChartSample();
            this.chartSampleSelected = new ChartSample();

            memoryStats.Read_0x20161226(stream);
            drawStats.Read_0x20161226(stream);
            physicsStats.Read_0x20161226(stream);
            physics2DStats.Read_0x20161226(stream);
            
            networkOperationStats.Read_0x20161226(stream);
            networkMessageStats.Read_0x20161226(stream);

            debugStats.Read_0x20161226(stream);
            audioStats.Read_0x20161226(stream);

            videoStats.Read_0x20161226(stream);
            chartSample.Read_0x20161226(stream);
            chartSampleSelected.Read_0x20161226(stream);
        }
    }
}