using System.Collections;
using System.Collections.Generic;

using UTJ.ProfilerReader.RawData;
using UTJ.ProfilerReader.RawData.Protocol;

using UTJ.ProfilerReader.BinaryData;
using UTJ.ProfilerReader.BinaryData.Thread;

namespace UTJ.ProfilerReader.RawData.Converter
{
    // output state
    class OutputFrameInfo
    {
        private struct AsyncMetadataAnchorInfo
        {
            public ProfilerSample profilerSample;
            public ThreadData threadData;
            public int sampleIdx;
            public AsyncMetadataAnchorInfo( ProfilerSample sample , ThreadData thread,int idx)
            {
                this.profilerSample = sample;
                this.threadData = thread;
                this.sampleIdx = idx;
            }
        }


        private ulong currentThreadId;
        private bool isFirst = true;

        public ProfilerFrameData outputFrame;
        // key:threadId
        private Dictionary<ulong, ThreadData> frameThreads;
        private Dictionary<ulong, ProfilerSample> frameParents;

        // gpu sampling
        private Dictionary<uint, AsyncMetadataAnchorInfo> asyncMetadataAnchorDictionary;

        // SessionID...
        private SessionHeader sessionHeader;
        // Dictionary For SpecialID( GC.Alloc)
        private RawDataDictionaryData dictionaryData;

        public ThreadData currentThreadOutput { private set; get; }
        public ProfilerSample currentProfilerSample { set; get; }

        public OutputFrameInfo(uint frameIdx, SessionHeader header, RawDataDictionaryData dict)
        {
            outputFrame = new ProfilerFrameData();
            outputFrame.frameIndex = (int)frameIdx;
            frameThreads = new Dictionary<ulong, ThreadData>();
            frameParents = new Dictionary<ulong, ProfilerSample>();

            this.sessionHeader = header;
            this.dictionaryData = dict;
        }

        public void SetThread(ref ThreadInfo threadInfo)
        {
            BackupSampleParentByThreadId(this.currentThreadId);
            ulong threadId = threadInfo.threadID;

            ThreadData targetThread = null;
            if (!frameThreads.TryGetValue(threadId, out targetThread))
            {
                targetThread = new ThreadData();
                targetThread.m_GroupName = threadInfo.group;
                targetThread.m_ThreadName = threadInfo.name;
                frameThreads.Add(threadId, targetThread);

                if (outputFrame.m_ThreadData == null)
                {
                    outputFrame.m_ThreadData = new List<ThreadData>();
                }
                outputFrame.m_ThreadData.Add(targetThread);
                outputFrame.m_ThreadCount++;
            }
            currentThreadOutput = targetThread;

            ProfilerSample parent = null;
            if (!frameParents.TryGetValue(threadId, out parent))
            {
                frameParents.Add(threadId, parent);
            }
            this.currentProfilerSample = parent;
            this.currentThreadId = threadId;
        }

        public void BeginProfilerSample(ProfilerSample sample)
        {
            if (currentThreadOutput == null) {
                //ProfilerLogUtil.LogError("currentThreadOutput is null.");
                return;
            }
            if (currentProfilerSample != null)
            {
                currentProfilerSample.nbChildren += 1;

                if (currentProfilerSample.children == null)
                {
                    currentProfilerSample.children = new List<ProfilerSample>();
                }
                currentProfilerSample.children.Add(sample);
            }

            sample.parent = this.currentProfilerSample;
            this.currentProfilerSample = sample;

            if (this.currentThreadOutput.m_AllSamples == null)
            {
                this.currentThreadOutput.m_AllSamples = new List<ProfilerSample>(1024 * 4);
            }
            this.currentThreadOutput.m_AllSamples.Add(sample);

            // append counter info
            if (sample.profilerInfo.IsCounter)
            {
                this.outputFrame.AddCounterSample(sample);
            }
        }

        public void SetCallStackInfo(ProfileCallstack callstack)
        {
            var samples = this.currentThreadOutput.m_AllSamples;
            if( samples != null && samples.Count > 0)
            {
                samples[samples.Count - 1].callStackInfo = callstack;
            }
        }

        public void BindAsyncMetadataAnchor(ref AsyncMetadataAnchor asyncMetadataAnchor){
            if(asyncMetadataAnchorDictionary == null)
            {
                this.asyncMetadataAnchorDictionary = new Dictionary<uint, AsyncMetadataAnchorInfo>();
            }
            this.asyncMetadataAnchorDictionary.Add(asyncMetadataAnchor.asyncMetadataId, 
                new AsyncMetadataAnchorInfo(currentProfilerSample,
                    this.currentThreadOutput,
                    this.currentThreadOutput.m_AllSamples.Count-1) );
        }

        public bool SetGPUSample(ref GPUSample gpuSample)
        {
            AsyncMetadataAnchorInfo info;
            uint asyncMetadatId = gpuSample.asyncMetadataId;
            if(!asyncMetadataAnchorDictionary.TryGetValue(asyncMetadatId, out info))
            {
                return false;
            }
            else
            {
                GPUTime gpuTime = new GPUTime();
                gpuTime.gpuTimeInMicroSec = (int)gpuSample.elapsedGpuTimeInMicroSec;
                gpuTime.gpuSection = gpuSample.gpuSection;
                gpuTime.relatedSampleIndex = (uint)info.sampleIdx;
                info.threadData.AddGPUTime( gpuTime);
                // remove
                asyncMetadataAnchorDictionary.Remove(asyncMetadatId);
                return true;
            }

        }

        public bool EndProfilerSample(ref SampleWrappedData startData, ref Sample endData,double scale,string unityVersion)
        {
            if (currentProfilerSample == null) {
                return false;
            }

            if (startData.sample.id != 0 && startData.sample.id != endData.id)
            {
                ProfilerLogUtil.LogError(" sampleID wronge " + startData.sample.id + "-" + endData.id + "(maybe missing EndSample)" );
                return false;
            }

            this.currentProfilerSample.timeUS = (endData.time - startData.sample.time) * (float)scale;   
            
            if(currentProfilerSample.timeUS  < 0.0f || currentProfilerSample.selfTimeUs < 0.0f )
            {
              //  ProfilerLogUtil.LogError(" Invalid Time" + currentProfilerSample.sampleName + "::" + currentProfilerSample.timeUS +"(" + currentProfilerSample.selfTimeUs);

            }
            // from 2018.3
            if( ProfilerDataStreamVersion.ConvertVersionForExecute( sessionHeader.version , unityVersion) >= ProfilerDataStreamVersion.Unity2018_3 )
            {
                if (startData.sample.id == dictionaryData.GCAllocSampleID)
                {
                    this.currentProfilerSample.gcAllocList = new List<AllocatedGCMemory>();
                    var gcAlloc = new AllocatedGCMemory();
                    uint tmpValue;
                    startData.metadatas[0].GetUintValue(out tmpValue);
                    gcAlloc.allocatedGCMemory = tmpValue;
                    this.currentProfilerSample.gcAllocList.Add(gcAlloc);
                    ProfilerSample.AddChildAllocToParent(currentProfilerSample);
                }
            }

            this.currentProfilerSample = this.currentProfilerSample.parent;
            return true;
        }

        public void SetCategoryInfo(Dictionary<ushort,CategoryInfo> categories)
        {
            this.outputFrame.m_categories = new List<BinaryData.Stats.Category>();
            foreach( var category in categories.Values)
            {
                var binData = new BinaryData.Stats.Category()
                {
                    categoryId = category.categoryID,
                    colorIndex = category.color,
                    flags = category.flags,
                    name = category.name
                };

                this.outputFrame.m_categories.Add(binData);
            }
        }

        private void BackupSampleParentByThreadId(ulong oldThreadId)
        {
            if (isFirst)
            {
                isFirst = false;
                return;
            }
            frameParents[oldThreadId] = this.currentProfilerSample;
        }

#if UTJ_CHECK
        public void ValidateData()
        {
            if( asyncMetadataAnchorDictionary != null)
            {
                if (asyncMetadataAnchorDictionary.Count > 0)
                {
                    var sb = new System.Text.StringBuilder(256);
                    sb.Append("[Validate]").Append(this.outputFrame.frameIndex).Append(" asyncMetadataAnchorDictionary ");
                    foreach( var kvs in asyncMetadataAnchorDictionary)
                    {
                        sb.Append(kvs.Key).Append(",");
                    }
                    Debug.DebugLogWrite.Log(sb.ToString());
                }
            }
        }
#endif
    }
}