
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public partial class ProfilerFrameData
        {
            internal bool ReadGeneric(System.IO.Stream stream, int frameSize, int threadCount, int index,uint version)
            {
                var reader = new StatsDataReader(stream);

                this.frameIndex = index;

                long before = stream.Position;

                if (stream.Length - before < frameSize || frameSize <= 0)
                {
                    ProfilerLogUtil.LogError("frame data maybe broken. " + frameSize + "/" + stream.Length );
                    return false;
                }
               

                //
                this.frameIndexFromFile = ProfilerLogUtil.ReadInt(stream);
                this.realFrame = ProfilerLogUtil.ReadInt(stream);

                if (version >= ProfilerDataStreamVersion.Unity2019_3)
                {
                    m_StartTimeUS.ReadAsUlong(stream);
                }
                else
                {
                    m_StartTimeUS.ReadAsInt(stream);
                }

                this.m_TotalCPUTimeInMicroSec = ProfilerLogUtil.ReadInt(stream);
                this.m_TotalGPUTimeInMicroSec = ProfilerLogUtil.ReadInt(stream);

                // 

                // GatheredData
                if (version >= ProfilerDataStreamVersion.Unity2019_3)
                {
                    this.m_GatheredData = ProfilerLogUtil.ReadInt(stream);
                }
                
                this.allStats = new AllProfilerStats();
                allStats.ReadGeneric(stream,version);

                // AudioInstance
                int audioIncentanceGroupInfoNum = ProfilerLogUtil.ReadInt(stream);
                m_AudioInstanceGroupInfo = new List<AudioProfilerGroupInfo>(audioIncentanceGroupInfoNum);
                for( int i = 0 ; i < audioIncentanceGroupInfoNum; ++ i ){
                    AudioProfilerGroupInfo audioGroupInfo = StatsDeserializeManager<AudioProfilerGroupInfo>.Instance.GetDeserializer(version).ReadObject(reader);
                    m_AudioInstanceGroupInfo.Add( audioGroupInfo);
                }
                int audioDspInfoNum =  ProfilerLogUtil.ReadInt(stream);
                m_AudioInstanceDSPInfo = new List<AudioProfilerDSPInfo>(audioDspInfoNum);
                for( int i = 0 ; i < audioDspInfoNum; ++ i){
                    AudioProfilerDSPInfo dspInfo = StatsDeserializeManager<AudioProfilerDSPInfo>.Instance.GetDeserializer(version).ReadObject(reader);
                    m_AudioInstanceDSPInfo.Add(dspInfo);
                }

                int nameSize = ProfilerLogUtil.ReadInt(stream);
                byte[] names = new byte[nameSize];
                stream.Read(names, 0, nameSize);
                m_AudioInstanceNames = "";
                for (int i = 0; i < names.Length; ++i)
                {
                    if (names[i] == 0)
                    {
                        m_AudioInstanceNames += "\n";
                        continue;
                    }
                    m_AudioInstanceNames += (char)names[i];
                }
                #region UIINFO
                // ui info
                int uiInfoNum = ProfilerLogUtil.ReadInt(stream);
                m_UISystemCanvasInfo = new List<UISystemProfilerInfo>(uiInfoNum);
                for (int i = 0; i < uiInfoNum; ++i)
                {
                    UISystemProfilerInfo  uiInfo = new UISystemProfilerInfo();
                    uiInfo.Read(stream);
                    m_UISystemCanvasInfo.Add(uiInfo);
                }
                // canvas names
                m_UISystemCanvasNames = ProfilerLogUtil.ReadNamesString(stream);
                // eventMaker
                int eventMakerNum = ProfilerLogUtil.ReadInt(stream);
                m_EventMarkers = new List<EventMarker>(eventMakerNum);
                for (int i = 0; i < eventMakerNum; ++i)
                {
                    EventMarker evtMaker = new EventMarker();
                    evtMaker.Read(stream);
                    m_EventMarkers.Add(evtMaker);
                }
                // event Names
                m_EventNames = ProfilerLogUtil.ReadNamesString(stream);
                // batchInstances
                int batchInstanceNum = ProfilerLogUtil.ReadInt(stream);
                m_UIBatchInstanceIDs = new List<uint>(batchInstanceNum);
                for (int i = 0; i < batchInstanceNum; ++i)
                {
                    m_UIBatchInstanceIDs.Add(ProfilerLogUtil.ReadUint(stream));
                }
                #endregion

                // thread Data
                this.m_ThreadCount = ProfilerLogUtil.ReadInt(stream);
                m_ThreadData = new List<ThreadData>(m_ThreadCount);
                for (int i = 0; i < m_ThreadCount; ++i)
                {
                    ThreadData threadData = new ThreadData();
                    threadData.Read_Generic(stream,version);
                    m_ThreadData.Add(threadData);
                }


                // JIT INFO
                if (version >= ProfilerDataStreamVersion.Unity2019_3)
                {
                    int jitInfoCout = ProfilerLogUtil.ReadInt(stream);
                    m_jitInfos = new List<JitInfo>(jitInfoCout);
                    for (int i = 0; i < jitInfoCout; ++i)
                    {
                        var jitInfo = new JitInfo();
                        jitInfo.Read(stream);
                        m_jitInfos.Add(jitInfo);
                    }
                }
                uint end = ProfilerLogUtil.ReadUint(stream);
                if ((end != EndCode))
                {
                    ProfilerLogUtil.LogError("EndCode was wrong " + end);
                    ProfilerLogUtil.LogError("frameSize : " + frameSize + " , readSize:" + (stream.Position - before));
                }
                return true;
            }

        }
    }
}
