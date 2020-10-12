
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class Physics2DStats
    {
        public const int kMaxPlatformDependentStats = 8;
        [StatsData(0)]
        public int m_TotalBodyCount;
        [StatsData(1)]
        public int m_ActiveBodyCount;
        [StatsData(2)]
        public int m_SleepingBodyCount;
        [StatsData(3)]
        public int m_DynamicBodyCount;
        [StatsData(4)]
        public int m_KinematicBodyCount;
        //  from 5.5.
        [StatsData(5,ProfilerDataStreamVersion.Unity55)]
        public int m_StaticBodyCount;

        [StatsData(6)]
        public int m_DiscreteBodyCount;
        [StatsData(7)]
        public int m_ContinuousBodyCount;
        [StatsData(8)]
        public int m_JointCount;
        [StatsData(9)]
        public int m_ContactCount;
        [StatsData(10)]
        public int m_ActiveColliderShapesCount;
        [StatsData(11)]
        public int m_SleepingColliderShapesCount;

        // All kFormatTime values are stored and transferred as ints, but are floats!
        [StatsData(20, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity2017_3)]
        public float m_StepTime;
        [StatsData(21, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity2017_3)]
        public float m_CollideTime;
        [StatsData(22, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity2017_3)]
        public float m_SolveTime;
        [StatsData(23, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity2017_3)]
        public float m_SolveInitialization;
        [StatsData(24, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity2017_3)]
        public float m_SolveVelocity;
        [StatsData(25, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity2017_3)]
        public float m_SolvePosition;
        [StatsData(26, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity2017_3)]
        public float m_SolveBroadphase;
        [StatsData(27, ProfilerDataStreamVersion.Unity54, ProfilerDataStreamVersion.Unity2017_3)]
        public float m_SolveTimeOfImpact;

        [StatsData(28, ProfilerDataStreamVersion.Unity54)]
        public int m_StaticColliderShapesCount;

        [StatsData(30, ProfilerDataStreamVersion.Unity2018_1)]
        public int m_DiscreteIslandCount;
        [StatsData(31, ProfilerDataStreamVersion.Unity2018_1)]
        public int m_ContinuousIslandCount;

        [StatsData(100, ProfilerDataStreamVersion.Unity54)]
        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];


    }
}