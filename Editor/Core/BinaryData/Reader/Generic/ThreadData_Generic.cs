
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Thread;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {


        public partial class ThreadData 
        {
            internal void Read_Generic(System.IO.Stream stream,uint version)
            {
                m_GroupName = ProfilerLogUtil.ReadString(stream);
                m_ThreadName = ProfilerLogUtil.ReadString(stream);
                // all samples
                int allSampleNum = ProfilerLogUtil.ReadInt(stream);
                m_AllSamples = new List<ProfilerSample>(allSampleNum);
                for (int i = 0; i < allSampleNum; ++i)
                {
                    ProfilerSample profilerSampe = new ProfilerSample();
                    profilerSampe.Read_Generic(stream,version);
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


                // profiler Information
                for (int i = 0; i < allSampleNum; ++i)
                {
                    m_AllSamples[i].ReadInformation_Generic(stream,version);
                }

                // warningSample
                int warningSampleNum = ProfilerLogUtil.ReadInt(stream);
                this.m_WarningSamples = new List<uint>(warningSampleNum);
                for (int i = 0; i < warningSampleNum; ++i)
                {
                    uint warning = ProfilerLogUtil.ReadUint(stream);
                    m_WarningSamples.Add(warning);
                }

                // Metadatas
                // todo currently skip metadata.

                int metadataSize = ProfilerLogUtil.ReadInt(stream);
                m_MetaDatas = new List<MetaData>(metadataSize);
                for (int i = 0; i < metadataSize; ++i)
                {
                    MetaData metadata = new MetaData();
                    metadata.Read(stream);
                    this.m_MetaDatas.Add(metadata);
                    /*
                    uint relatedSampleIndex = ProfilerLogUtil.ReadUint(stream);

                    int metadataValueSize = ProfilerLogUtil.ReadInt(stream);
                    for (int j = 0; j < metadataValueSize; ++j)
                    {
                        int type = ProfilerLogUtil.ReadInt(stream);

                        int tmpArrSize = ProfilerLogUtil.ReadInt(stream);
                        for (int k = 0; k < tmpArrSize; ++k)
                        {
                            byte tmpValue = ProfilerLogUtil.ReadUInt8Value(stream);
                        }
                        ProfilerLogUtil.AlignSkip(stream, tmpArrSize, 4);
                    }
                    */
                }
                if( version >=ProfilerDataStreamVersion.Unity2019_1)
                {
                    int frameStatSize  = ProfilerLogUtil.ReadInt(stream);
                    m_FrameStatSamples = new List<uint>(frameStatSize);
                    for ( int i = 0;i < frameStatSize; ++i)
                    {
                        m_FrameStatSamples.Add( ProfilerLogUtil.ReadUint(stream) );
                    }
                }
                if( version >= ProfilerDataStreamVersion.Unity2018_3)
                {
                    m_MaxDepth = ProfilerLogUtil.ReadInt(stream);
                }


                // Callstack Samples
                if (version >= ProfilerDataStreamVersion.Unity2019_3)
                {
                    int callStackSampleNum = ProfilerLogUtil.ReadInt(stream);
                    this.m_CallstackSamples = new List<ProfileCallstack>(callStackSampleNum);
                    for(int i = 0; i < callStackSampleNum; ++i)
                    {
                        ProfileCallstack profileCallstack = new ProfileCallstack();
                        profileCallstack.Read(stream);
                        this.m_CallstackSamples.Add(profileCallstack);
                    }
                }


                // resole 
                ProfilerSample.ResolveCallGraph(m_AllSamples);
                ProfilerSample.ResolveHierarychyLevel(m_AllSamples);
                ProfilerSample.ResolveGcAllocate(m_AllSamples, m_AllocatedGCMemorySamples);
                ProfilerSample.ResolveSampleMetadata(m_AllSamples, m_MetaDatas);
                ProfilerSample.ResolveCallstackInfo(m_AllSamples, m_CallstackSamples);
            }
        }
    }
}