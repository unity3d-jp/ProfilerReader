using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class PhysicsStats
    {

        internal void Read_0x20140736(System.IO.Stream stream)
        {
            this.activeRigidbodies = ProfilerLogUtil.ReadInt(stream);
            this.sleepingRigidbodies = ProfilerLogUtil.ReadInt(stream);
            this.numberOfShapePairs = ProfilerLogUtil.ReadInt(stream);
            this.numberOfStaticColliders = ProfilerLogUtil.ReadInt(stream);
            this.numberOfDynamicColliders = ProfilerLogUtil.ReadInt(stream);

            for (int i = 0; i < kMaxPlatformDependentStats; ++i)
            {
                this.platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }
        }
    }
}