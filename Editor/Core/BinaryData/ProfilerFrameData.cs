
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
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

            // from 2020.2( counterValues)
            private List<ProfilerSample> m_CounterValues;
            // Even if there are same name samples...So "List<ProfilerSample>" 
            private Dictionary<string, List<ProfilerSample>> m_CounterValueDictionary;

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

            #region COUNTER_VALUE

            public void GetCounterKeys(List<string> list)
            {
                list.Clear();
                if (m_CounterValueDictionary == null) { return; }
                foreach( var kvs in m_CounterValueDictionary)
                {
                    list.Add(kvs.Key);
                }
            }

            public bool TryGetCounterAsType<T>(string name, out T val) where T :struct
            {
                var metadata = GetCounterMetadata(name);
                if(metadata == null)
                {
                    val = default(T);
                    return false;

                }
                var typeValue = metadata.GetType();
                if (typeValue != typeof(T))
                {
                    val = default(T);
                    return false;
                }
                val = (T)metadata;
                return true;
            }



            private object GetCounterMetadata(string name)
            {
                ProfilerSample sample;
                if (!TryGetCounterSample(name, out sample))
                {
                    return null;
                }
                var metadatas = sample.metadataValues;
                if (metadatas == null && metadatas.Count < 1)
                {
                    return null;
                }
                return metadatas[0].convertedObject;
            }

            private bool TryGetCounterSample(string name,out ProfilerSample sample) {
                List<ProfilerSample> list = GetCounterSampleList(name);
                if (list == null)
                {
                    sample = null;
                    return false;
                }
                if (list.Count != 1) 
                {
                    ProfilerLogUtil.logErrorString("[CounterError]"+name +" count:" + list.Count);
                    sample = null;
                    return false;
                }
                sample = list[0];
                return true;
            }
            private List<ProfilerSample> GetCounterSampleList(string name)
            {
                List<ProfilerSample> list = null;
                if (this.m_CounterValueDictionary == null)
                {
                    return null;
                }
                if (!this.m_CounterValueDictionary.TryGetValue(name, out list))
                {
                    return null;
                }
                return list;
            }


            internal void AddCounterSample(ProfilerSample sample)
            {
                if (this.m_CounterValues == null) { m_CounterValues = new List<ProfilerSample>(32); }
                m_CounterValues.Add(sample);

                if(sample.profilerInfo == null) { return; }
                string key = sample.profilerInfo.name;
                if (this.m_CounterValueDictionary == null) { m_CounterValueDictionary = new Dictionary<string, List<ProfilerSample>>(32); }
                List<ProfilerSample> list = null;
                if(!this.m_CounterValueDictionary.TryGetValue(key,out list))
                {
                    list = new List<ProfilerSample>(4);
                    this.m_CounterValueDictionary.Add(key, list);
                }
                list.Add(sample);
            }

            #region APPLY_TO_DEPLICATED_STATS
            internal void ApplyCountersToDeplicatedStats()
            {
                if(allStats == null) { return; }
                ApplyMemoryCounterToStats();
                ApplyDrawCounterToStats();
            }
            private void ApplyMemoryCounterToStats() {
                var memoryStats = allStats.memoryStats;
                if(memoryStats == null) { memoryStats = new MemoryStats(); }

                ApplyCounterValueFromLong("Total Used Memory", ref memoryStats.bytesUsedTotal);
                ApplyCounterValueFromLong("GC Used Memory", ref memoryStats.bytesUsedMono);
                ApplyCounterValueFromLong("Gfx Used Memory", ref memoryStats.bytesUsedGFX);
                ApplyCounterValueFromLong("Audio Used Memory", ref memoryStats.bytesUsedFMOD);
                ApplyCounterValueFromLong("Video Used Memory", ref memoryStats.bytesUsedVideo);
                ApplyCounterValueFromLong("Profiler Used Memory", ref memoryStats.bytesUsedProfiler);

                ApplyCounterValueFromLong("Total Reserved Memory", ref memoryStats.bytesReservedTotal);
                ApplyCounterValueFromLong("GC Reserved Memory", ref memoryStats.bytesReservedMono);
                ApplyCounterValueFromLong("Gfx Reserved Memory", ref memoryStats.bytesReservedGFX);
                ApplyCounterValueFromLong("Audio Reserved Memory", ref memoryStats.bytesReservedFMOD);
                ApplyCounterValueFromLong("Video Reserved Memory", ref memoryStats.bytesReservedVideo);
                ApplyCounterValueFromLong("Profiler Reserved Memory", ref memoryStats.bytesReservedProfiler);

                ApplyCounterValueFromLong("System Used Memory", ref memoryStats.bytesUsedUnity);

                ApplyCounterValueFromInt("Texture Count", ref memoryStats.textureCount);
                ApplyCounterValueFromLong("Texture Memory", ref memoryStats.textureBytes);
                ApplyCounterValueFromInt("Mesh Count", ref memoryStats.meshCount);
                ApplyCounterValueFromLong("Mesh Memory", ref memoryStats.meshBytes);
                ApplyCounterValueFromLong("System Used Memory", ref memoryStats.materialCount);
                ApplyCounterValueFromLong("System Used Memory", ref memoryStats.materialBytes);
                ApplyCounterValueFromInt("AnimationClip Count", ref memoryStats.animationClipCount);
                ApplyCounterValueFromLong("AnimationClip Memory", ref memoryStats.animationClipBytes);

                ApplyCounterValueFromInt("Asset Count", ref memoryStats.assetCount);
                ApplyCounterValueFromInt("Game Object Count", ref memoryStats.gameObjectCount);
                ApplyCounterValueFromInt("Scene Object Count", ref memoryStats.sceneObjectCount);
                ApplyCounterValueFromInt("Object Count", ref memoryStats.totalObjectsCount);

                ApplyCounterValueFromInt("GC Allocation In Frame Count", ref memoryStats.frameGCAllocCount);
                ApplyCounterValueFromLong("GC Allocated In Frame", ref memoryStats.frameGCAllocBytes);
            }

            private void ApplyDrawCounterToStats()
            {
                var drawStats = allStats.drawStats; 
                if (drawStats == null) { drawStats = new DrawStats(); }

                ApplyCounterValueFromInt("Batches Count", ref drawStats.batches);
                ApplyCounterValueFromInt("Draw Calls Count", ref drawStats.drawCalls);
                ApplyCounterValueFromLong("Triangles Count", ref drawStats.triangles);
                ApplyCounterValueFromLong("Vertices Count", ref drawStats.vertices);
                ApplyCounterValueFromInt("SetPass Calls Count", ref drawStats.setPassCalls);
                ApplyCounterValueFromInt("Shadow Casters Count", ref drawStats.shadowCasters);

                ApplyCounterValueFromInt("Visible Skinned Meshes Count", ref drawStats.visibleSkinnedMeshes);
                ApplyCounterValueFromInt("Render Textures Changes Count", ref drawStats.renderTextureStateChanges);
                ApplyCounterValueFromInt("Render Textures Count", ref drawStats.renderTextureCount);
                ApplyCounterValueFromLong("Render Textures Bytes", ref drawStats.renderTextureBytes);

                /*
                ApplyCounterValueFromInt("Used Buffers Count", ref drawStats.);
                ApplyCounterValueFromLong("Used Buffers Bytes", ref drawStats.renderTextureBytes);
                */

                ApplyCounterValueFromInt("Vertex Buffer Upload In Frame Count", ref drawStats.vboUploads);
                ApplyCounterValueFromLong("Vertex Buffer Upload In Frame Bytes", ref drawStats.vboUploadBytes);
                ApplyCounterValueFromInt("Index Buffer Upload In Frame Count", ref drawStats.ibUploads);
                ApplyCounterValueFromLong("Index Buffer Upload In Frame Bytes", ref drawStats.ibUploadBytes);
                ApplyCounterValueFromLong("Video Memory Bytes", ref drawStats.totalAvailableVRamMBytes);

//                ApplyCounterValueFromUlong("Dynamic Batching Time");

                ApplyCounterValueFromInt("Dynamic Batches Count", ref drawStats.dynamicBatches);
                ApplyCounterValueFromInt("Dynamic Batched Draw Calls Count", ref drawStats.dynamicBatchedDrawCalls);
                ApplyCounterValueFromLong("Dynamic Batched Triangles Count", ref drawStats.dynamicBatchedTriangles);
                ApplyCounterValueFromLong("Dynamic Batched Vertices Count", ref drawStats.dynamicBatchedVertices);


                ApplyCounterValueFromInt("Static Batches Count", ref drawStats.staticBatches);
                ApplyCounterValueFromInt("Static Batched Draw Calls Count", ref drawStats.staticBatchedDrawCalls);
                ApplyCounterValueFromLong("Static Batched Triangles Count", ref drawStats.staticBatchedTriangles);
                ApplyCounterValueFromLong("Static Batched Vertices Count", ref drawStats.staticBatchedVertices);

                ApplyCounterValueFromInt("Instanced Batches Count", ref drawStats.instancedBatches);
                ApplyCounterValueFromInt("Instanced Batched Draw Calls Count", ref drawStats.instancedBatchedDrawCalls);
                ApplyCounterValueFromLong("Instanced Batched Triangles Count", ref drawStats.instancedTriangles);
                ApplyCounterValueFromLong("Instanced Batched Vertices Count", ref drawStats.instancedVertices);


                ApplyCounterValueFromInt("Used Textures Count", ref drawStats.usedTextureCount);
                ApplyCounterValueFromLong("Used Textures Bytes", ref drawStats.renderTextureCount);

            }
            private void ApplyCounterValueFromInt(string name, ref int val)
            {
                int tmpVal = 0;
                if (this.TryGetCounterAsType<int>(name, out tmpVal))
                {
                    val = tmpVal;
                }
                else
                {
                    ProfilerLogUtil.logErrorString("[Counter]NotFound " + name);
                }
            }
            private void ApplyCounterValueFromUlong(string name, ref int val)
            {
                ulong tmpVal = 0;
                if (this.TryGetCounterAsType<ulong>(name, out tmpVal))
                {
                    val = (int)tmpVal;
                }
                else
                {
                    ProfilerLogUtil.logErrorString("[Counter]NotFound " + name);
                }
            }

            private void ApplyCounterValueFromLong(string name , ref int val)
            {
                long tmpVal = 0;
                if (this.TryGetCounterAsType<long>(name, out tmpVal))
                {
                    val = (int)tmpVal;
                }
                else
                {
                    ProfilerLogUtil.logErrorString("[Counter]NotFound " + name);
                }
            }
            #endregion APPLY_TO_DEPLICATED_STATS

            #endregion COUNTER_VALUE


        }


    }
}
