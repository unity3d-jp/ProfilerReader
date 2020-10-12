
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class PhysicsStats
    {
        public const int kMaxPlatformDependentStats = 8;

        #region BEFORE_UNITY_5_4
        [StatsData(0, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity54)]
        int activeRigidbodies;
        [StatsData(1, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity54)]
        int sleepingRigidbodies;

        [StatsData(2, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity54)]
        int numberOfShapePairs;

        [StatsData(3, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity54)]
        int numberOfStaticColliders;
        [StatsData(4, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity54)]
        int numberOfDynamicColliders;
        #endregion BEFORE_UNITY_5_4

        // after 5.5
        #region AFTER_UNITY_5_5
        [StatsData(0, ProfilerDataStreamVersion.Unity55)]
        public int numActiveDynamicBodies;
        [StatsData(1, ProfilerDataStreamVersion.Unity55)]
        public int numActiveKinematicBodies;

        [StatsData(2, ProfilerDataStreamVersion.Unity55)]
        public int numShapePairs;

        [StatsData(3, ProfilerDataStreamVersion.Unity55)]
        public int numStaticBodies;
        [StatsData(4, ProfilerDataStreamVersion.Unity55)]
        public int numDynamicBodies;

        [StatsData(5, ProfilerDataStreamVersion.Unity55)]
        public int numTriggerOverlaps;
        [StatsData(6, ProfilerDataStreamVersion.Unity55)]
        public int numConstraints;
        #endregion AFTER_UNITY_5_5

        [StatsData(7, ProfilerDataStreamVersion.Unity55)]
        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];

    }
}