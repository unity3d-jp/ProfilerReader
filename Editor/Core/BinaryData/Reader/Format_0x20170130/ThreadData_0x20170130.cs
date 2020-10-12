
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Thread;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {


        public partial class ThreadData 
        {
            internal void Read_0x20170130(System.IO.Stream stream)
            {
                m_GroupName = ProfilerLogUtil.ReadString(stream);
                m_ThreadName = ProfilerLogUtil.ReadString(stream);
                // all samples
                int allSampleNum = ProfilerLogUtil.ReadInt(stream);

                m_AllSamples = new List<ProfilerSample>(allSampleNum);
                for (int i = 0; i < allSampleNum; ++i)
                {
                    ProfilerSample profilerSampe = new ProfilerSample();
                    profilerSampe.Read_0x20170130(stream);
                    m_AllSamples.Add(profilerSampe);
                }
                // gpu samples
                int gpuSampleNum = ProfilerLogUtil.ReadInt(stream);
                m_GPUTimeSamples = new List<GPUTime>(gpuSampleNum);
                for (int i = 0; i < gpuSampleNum; ++i)
                {
                    GPUTime gpuTm = new GPUTime();
                    gpuTm.Read(stream);
                    m_GPUTimeSamples.Add(gpuTm);
                }
                //gc memorySamples
                int gcSampleNum = ProfilerLogUtil.ReadInt(stream);
                m_AllocatedGCMemorySamples = new List<AllocatedGCMemory>(gcSampleNum);

                for (int i = 0; i < gcSampleNum; ++i)
                {
                    AllocatedGCMemory allocGc = new AllocatedGCMemory();
                    allocGc.Read(stream);
                    m_AllocatedGCMemorySamples.Add(allocGc);
                }

                // ここまではOK

                // profiler Information
                for (int i = 0; i < allSampleNum; ++i)
                {
                    m_AllSamples[i].ReadInformation_0x20170130(stream);
                }

                // warningSample
                int warningSampleNum = ProfilerLogUtil.ReadInt(stream);
                this.m_WarningSamples = new List<uint>(warningSampleNum);
                for (int i = 0; i < warningSampleNum; ++i)
                {
                    uint warning = ProfilerLogUtil.ReadUint(stream);
                    m_WarningSamples.Add(warning);
                }

                // Offset
                int metaDataOffsetNum = ProfilerLogUtil.ReadInt(stream);
                this.m_MetaDataOffsets = new List<MetaDataOffset>(metaDataOffsetNum);
                for (int i = 0; i < metaDataOffsetNum; ++i)
                {
                    MetaDataOffset metaDataOffset = new MetaDataOffset();
                    metaDataOffset.Read(stream);
                    m_MetaDataOffsets.Add(metaDataOffset);
                }


                int metaDataNum = ProfilerLogUtil.ReadInt(stream);
                this.m_MetaData = new List<byte>(metaDataNum);
                for (int i = 0; i < metaDataNum; ++i)
                {
                    byte metaData = ProfilerLogUtil.ReadUInt8Value(stream);
                    m_MetaData.Add(metaData);
                }
                ProfilerLogUtil.AlignSkip(stream, metaDataNum, 4);

                // resole 
                ProfilerSample.ResolveCallGraph(m_AllSamples);
                ProfilerSample.ResolveHierarychyLevel(m_AllSamples);
                ProfilerSample.ResolveGcAllocate(m_AllSamples, m_AllocatedGCMemorySamples);
            }
        }
    }
}