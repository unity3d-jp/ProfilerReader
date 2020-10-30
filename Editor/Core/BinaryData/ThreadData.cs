
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Thread;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public partial class ThreadData 
        {
            public List<ProfilerSample> m_AllSamples { get; set; }
            public List<GPUTime> m_GPUTimeSamples { get; set; }
            public List<AllocatedGCMemory> m_AllocatedGCMemorySamples { get; set; }
            public List<uint> m_WarningSamples { get; set; }

            // from 5.5
            List<ProfileCallstack> m_CallstackSamples;

            // from 5.6 to 2017.2
            List<MetaDataOffset> m_MetaDataOffsets;
            List<byte> m_MetaData;
            // sampleMetadatas( from 2017.3 )
            List<MetaData> m_MetaDatas;



            // from 2018.3
            int m_MaxDepth;
            // from 2019.1
            List<uint> m_FrameStatSamples;
            // from 2020.1
            List<FlowEvent> m_FlowEvents;


            public string FullName
            {
                get
                {
                    if(string.IsNullOrEmpty( m_GroupName ))
                    {
                        return m_ThreadName;
                    }
                    return m_GroupName + "/" + m_ThreadName;
                }
            }


            public string m_GroupName { get; set; }
            public string m_ThreadName { get; set; }

            public bool IsMainThread
            {
                get
                {
                    return (m_ThreadName == "Main Thread");
                }
            }
            public void AddGPUTime(GPUTime time)
            {
                if(m_GPUTimeSamples == null) { m_GPUTimeSamples = new List<GPUTime>(); }
                m_GPUTimeSamples.Add(time);
            }


        }
    }
}