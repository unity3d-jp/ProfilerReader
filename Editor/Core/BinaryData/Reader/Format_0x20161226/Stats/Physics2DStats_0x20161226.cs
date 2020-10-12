
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class Physics2DStats
    {

        internal void Read_0x20161226(System.IO.Stream stream)
        {
            m_TotalBodyCount = ProfilerLogUtil.ReadInt(stream);
            m_ActiveBodyCount = ProfilerLogUtil.ReadInt(stream);
            m_SleepingBodyCount = ProfilerLogUtil.ReadInt(stream);
            m_DynamicBodyCount = ProfilerLogUtil.ReadInt(stream);
            m_KinematicBodyCount = ProfilerLogUtil.ReadInt(stream);
            m_StaticBodyCount = ProfilerLogUtil.ReadInt(stream);

            m_DiscreteBodyCount = ProfilerLogUtil.ReadInt(stream);
            m_ContinuousBodyCount = ProfilerLogUtil.ReadInt(stream);
            m_JointCount = ProfilerLogUtil.ReadInt(stream);
            m_ContactCount = ProfilerLogUtil.ReadInt(stream);
            m_ActiveColliderShapesCount = ProfilerLogUtil.ReadInt(stream);
            m_SleepingColliderShapesCount = ProfilerLogUtil.ReadInt(stream);

            m_StaticColliderShapesCount = ProfilerLogUtil.ReadInt(stream);

            m_StepTime = ProfilerLogUtil.ReadInt(stream);
            m_CollideTime = ProfilerLogUtil.ReadInt(stream);
            m_SolveTime = ProfilerLogUtil.ReadInt(stream);
            m_SolveInitialization = ProfilerLogUtil.ReadInt(stream);
            m_SolveVelocity = ProfilerLogUtil.ReadInt(stream);
            m_SolvePosition = ProfilerLogUtil.ReadInt(stream);
            m_SolveBroadphase = ProfilerLogUtil.ReadInt(stream);
            m_SolveTimeOfImpact = ProfilerLogUtil.ReadInt(stream);

            for (int i = 0; i < kMaxPlatformDependentStats; ++i)
            {
                this.platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }

        }
    }
}