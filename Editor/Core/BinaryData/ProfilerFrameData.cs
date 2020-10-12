
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public partial class ProfilerFrameData
        {
            private const uint EndCode = 0xAFAFAFAF;
            // The data from binary log file maybe wrong...
            private int m_FrameID;
            private int frameIndexFromFile;
            private int realFrame;

            // renumbering
            public int frameIndex { get; set; }

            public ProfileTimeFormat m_StartTimeUS { get; set; }
            public int m_TotalCPUTimeInMicroSec { get; set; }
            public int m_TotalGPUTimeInMicroSec { get; set; }

            // from 2019.3
            public int m_GatheredData { get; set; }

            public AllProfilerStats allStats { get; set; }
            public int selectedTime { get; set; }
            public List<ThreadData> m_ThreadData { get; set; }
            public int m_ThreadCount { get; set; }

            // before 5.4
            public List<AudioProfilerInfo> m_AudioInstanceInfo { get; set; }
            // afeter 5.5 
            public List<AudioProfilerGroupInfo> m_AudioInstanceGroupInfo { get; set; }
            public List<AudioProfilerDSPInfo> m_AudioInstanceDSPInfo { get; set; }
            public List<AudioProfilerClipInfo> m_AudioInstanceClipInfo { get; set; }

            private string m_AudioInstanceNames;

            /** Todo UI Profiler form 2017.1 */
            public List<UISystemProfilerInfo> m_UISystemCanvasInfo{get;set;}
            public List<string> m_UISystemCanvasNames{get;set;}
            public List<EventMarker> m_EventMarkers{get;set;}
            public List<string> m_EventNames{get;set;}
            public List<uint> m_UIBatchInstanceIDs { get; set; }

            // from 2019.3
            public List<JitInfo> m_jitInfos;
            private List<JitInfo> m_sortedJitByAddr;




            public ThreadData MainThread
            {
                get
                {
                    if (m_ThreadData == null) { return null; }
                    int length = m_ThreadData.Count;
                    for (int i = 0; i < length; ++i)
                    {
                        if (m_ThreadData[i].IsMainThread)
                        {
                            return m_ThreadData[i];
                        }
                    }
                    return null;
                }
            }

            public JitInfo FindJitInfoFromAddr(ulong addr)
            {
                if (this.m_jitInfos == null || this.m_jitInfos.Count == 0)
                {
                    return null;
                }
                if (m_sortedJitByAddr == null)
                {
                    m_sortedJitByAddr = new List<JitInfo>(this.m_jitInfos);
                    m_sortedJitByAddr.Sort(new JitInfo.CompareByAddr() );
                }
                int minIdx = 0 , maxIdx = m_sortedJitByAddr.Count-1;


                for (int currentIdx = maxIdx / 2; minIdx != maxIdx;) {
                    int lastIdx = currentIdx;
                    var info = m_sortedJitByAddr[currentIdx];

                    if( info.codeAddr <= addr && addr < info.codeAddr + info.size)
                    {
                        return info;
                    }
                    if( addr > info.codeAddr + info.size)
                    {
                        minIdx = currentIdx;
                    }
                    else
                    {
                        maxIdx = currentIdx;
                    }
                    currentIdx = (minIdx + maxIdx) / 2;
                    if( lastIdx == currentIdx)
                    {
                        break;
                    }
                }

                return null;
            }

        }
        

    }
}
