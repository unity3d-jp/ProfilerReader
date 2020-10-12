
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class PhysicsStats
    {

        internal void Read_0x20160622(System.IO.Stream stream)
        {
            numActiveDynamicBodies = ProfilerLogUtil.ReadInt(stream);
            numActiveKinematicBodies = ProfilerLogUtil.ReadInt(stream);
            numShapePairs = ProfilerLogUtil.ReadInt(stream);
            numStaticBodies = ProfilerLogUtil.ReadInt(stream);
            numDynamicBodies = ProfilerLogUtil.ReadInt(stream);
            numTriggerOverlaps = ProfilerLogUtil.ReadInt(stream);
            numConstraints = ProfilerLogUtil.ReadInt(stream);

            for (int i = 0; i < kMaxPlatformDependentStats; ++i)
            {
                this.platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }
        }
    }
}