
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

                m_StartTimeUS.Read(stream, version);

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


                // from Unity 2022
                if (version >= ProfilerDataStreamVersion.Unity2022_2)
                {
                    this.makerSymbolDictionary = new MakerSymbolDictionary();
                    this.makerSymbolDictionary.Clear();
                    int markerCount = ProfilerLogUtil.ReadInt(stream);
                    for (int i = 0; i < markerCount; ++i)
                    {
                        uint id = ProfilerLogUtil.ReadUint(stream);
                        string name = ProfilerLogUtil.ReadString(stream);
                        uint groupFlag = ProfilerLogUtil.ReadUint(stream);
                        int paramCount = ProfilerLogUtil.ReadInt(stream);
                        var makerInfo = new MakerSymbolDictionary.MakerInfo(id, name, groupFlag, paramCount);
                        for (int j = 0; j < paramCount; ++j)
                        {
                            MetadataDescription metadataDescription = new MetadataDescription();
                            metadataDescription.Read(stream);
                            makerInfo.AddDesctiption(metadataDescription);
                        }
                        this.makerSymbolDictionary.AddMakerInfo(makerInfo);
                    }
                }

                // thread Data
                this.m_ThreadCount = ProfilerLogUtil.ReadInt(stream);
                m_ThreadData = new List<ThreadData>(m_ThreadCount);
                for (int i = 0; i < m_ThreadCount; ++i)
                {
                    ThreadData threadData = new ThreadData();
                    threadData.Read_Generic(stream,version);
                    // collect counter value
                    if (version >= ProfilerDataStreamVersion.Unity2020_2)
                    {
                        CollectCounterValueSamples(threadData.m_AllSamples);
                    }

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
                        jitInfo.Read(stream,version);
                        m_jitInfos.Add(jitInfo);
                    }
                }
                // Category
                if(version >= ProfilerDataStreamVersion.Unity2021_2)
                {
                    int categoryNum = ProfilerLogUtil.ReadInt(stream);
                    m_categories = new List<Category>(categoryNum);
                    for (int i = 0; i < categoryNum; ++i)
                    {
                        var category = new Category();
                        category.Read(stream, version);
                        m_categories.Add(category);
                    }
                }


                // ObjectNames
                if(version >= ProfilerDataStreamVersion.Unity2022_1)
                {
                    // Native Types
                    int nativeTypesCount = ProfilerLogUtil.ReadInt(stream);
                    for(int i = 0;i < nativeTypesCount; ++i)
                    {

                        uint baseTypeId = ProfilerLogUtil.ReadUint(stream);
                        string name = ProfilerLogUtil.ReadString(stream);
                    }
                    // Unity Objects
                    int unityObjectCount = ProfilerLogUtil.ReadInt(stream);
                    for(int i = 0;i< unityObjectCount; ++i)
                    {

                        int instanceId = ProfilerLogUtil.ReadInt(stream);
                        int relatedGameObjectInstanceId = 0;
                        if (version >= ProfilerDataStreamVersion.Unity2022_2)
                        {
                            relatedGameObjectInstanceId = ProfilerLogUtil.ReadInt(stream);
                        }
                        uint typeId = ProfilerLogUtil.ReadUint(stream);
                        string name = ProfilerLogUtil.ReadString(stream);

                        // Extra rootid for objects
                        ulong rootId = 0;
                        if (version >= ProfilerDataStreamVersion.Unity2022_2)
                        {
                            rootId = ProfilerLogUtil.ReadULong(stream);
                        }
                    }

                }

                // GfxResourceInfo
                if(version >= ProfilerDataStreamVersion.Unity2022_2)
                {

                    // Read GfxResource to root mapping
                    int gfxResourceCount = ProfilerLogUtil.ReadInt(stream);
                    for (int i = 0; i < gfxResourceCount; ++i)
                    {
                        ulong gfxResourceId = ProfilerLogUtil.ReadULong(stream);
                        ulong rootId = ProfilerLogUtil.ReadULong(stream);

                    }
                }
                // resolve makers

                if (version >= ProfilerDataStreamVersion.Unity2022_2)
                {
                    makerSymbolDictionary.ResolveThread(this.m_ThreadData);
                }

                uint end = ProfilerLogUtil.ReadUint(stream);
                if ((end != EndCode))
                {
                    ProfilerLogUtil.LogError("EndCode was wrong " + end);
                    ProfilerLogUtil.LogError("frameSize : " + frameSize + " , readSize:" + (stream.Position - before));
                }
                return true;
            }


            private void CollectCounterValueSamples(List<ProfilerSample> sampleList)
            {
                foreach (var sample in sampleList)
                {
                    if (sample == null || sample.profilerInfo == null)
                    {
                        continue;
                    }
                    if (sample.profilerInfo.IsCounter)
                    {
                        this.AddCounterSample(sample);
                    }
                }
            }
        }
    }
}
