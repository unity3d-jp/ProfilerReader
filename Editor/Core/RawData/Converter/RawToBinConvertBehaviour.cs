using System.Collections;
using System.Collections.Generic;

using UTJ.ProfilerReader.RawData;
using UTJ.ProfilerReader.RawData.Protocol;

using UTJ.ProfilerReader.BinaryData;
using UTJ.ProfilerReader.BinaryData.Thread;
using UTJ.ProfilerReader.UnityType;

namespace UTJ.ProfilerReader.RawData.Converter
{
    public class RawToBinConvertBehaviour : RawDataBehaviour
    {
        /// <summary>
        /// SampleInfo/ThreadInfo Dictionary
        /// </summary>
        private RawDataDictionaryData dictionaryData;

        private ulong currentThreadId;
        private uint currentFrameIdx = 0;

        private SampleStackResolver sampleStackResolver ;

        public uint currentFrameIndex
        {
            get
            {
                return currentFrameIdx;
            }
        }

        public bool isFrameDataRead
        {
            get;private set;
        }

        public SessionHeader header
        {
            get
            {
                return sessionHeader;
            }
        }

        private SessionHeader sessionHeader;


        // key:frameId - value:convertedFrameData
        private Dictionary<uint, OutputFrameInfo> outputFrameDatas;
        private OutputFrameInfo currentOutputFrame;
        private string unityVersion;
        

        public RawToBinConvertBehaviour()
        {
            this.dictionaryData = new RawDataDictionaryData();
            this.outputFrameDatas = new Dictionary<uint, OutputFrameInfo>();
            this.sampleStackResolver = new SampleStackResolver();

            this.isFrameDataRead = false;
        }

        public void SetUnityVersion(string unityVer)
        {
            this.unityVersion = unityVer;
        }

        public ProfilerFrameData GetFrameData(uint frameIdx)
        {
            OutputFrameInfo info = null;
            if(!outputFrameDatas.TryGetValue(frameIdx,out info))
            {
                return null;
            }
            sampleStackResolver.Check();
            var retVal = info.outputFrame;
            retVal.m_jitInfos = dictionaryData.GetMethodJitInfos();

#if UTJ_CHECK
            info.ValidateData();
#endif

            return retVal;
        }
        public void ReleaseFrameData( uint frameIdx)
        {
            if (outputFrameDatas.ContainsKey(frameIdx))
            {
                outputFrameDatas.Remove(frameIdx);
            }
            sampleStackResolver.ReleaseFrameData(frameIdx);
        }

        public void OnSessionStart(ref SessionHeader header)
        {
            this.sessionHeader = header;
        }


        public void OnBlockStart(ulong threadId)
        {

#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[RawToBinConvertBehaviour] OnBlockStart:" + threadId);
#endif
            ThreadInfo threadInfo = this.dictionaryData.GetThreadInfo(threadId);
            this.currentThreadId = threadId;


            if (currentOutputFrame != null)
            {
                currentOutputFrame.SetThread(ref threadInfo);
            }
            sampleStackResolver.SetThread(this.currentThreadId);
        }


        public void OnGlobalDataRead(ref ProfilerState profilerState)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnGlobalDataRead] ProfilerState:" + profilerState.time + "  " + profilerState.frameIndex);
#endif
        }
        public void OnGlobalDataRead(ref ThreadInfo threadInfo)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnGlobalDataRead] ThreadInfo:" + threadInfo.threadID + "  " + threadInfo.name);
#endif
            this.dictionaryData.AddThreadInfo(ref threadInfo);
        }
        public void OnGlobalDataRead(ref SamplerInfo sampleInfo)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnGlobalDataRead] SamplerInfo:" + sampleInfo.samplerId
                + " flag:" + sampleInfo.flags + " group:" + sampleInfo.group + "  " + sampleInfo.name);
#endif
            this.dictionaryData.AddSampleInfo(ref sampleInfo);
        }

        public void OnDataRead(ref Frame frame)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead] Frame:" + frame.index + "  " + frame.time +"__Thread " + this.currentThreadId);
#endif

            this.currentFrameIdx = frame.index;
            this.isFrameDataRead = true;
            // set outputdata
            OutputFrameInfo frameInfo = null;
            if( !outputFrameDatas.TryGetValue( this.currentFrameIdx , out frameInfo ) ){
                frameInfo = new OutputFrameInfo(currentFrameIdx,sessionHeader,dictionaryData);
                outputFrameDatas.Add(this.currentFrameIdx, frameInfo);
            }
            currentOutputFrame = frameInfo;
            sampleStackResolver.SetFrame(this.currentFrameIdx);

            // 
            sampleStackResolver.SetThread(this.currentThreadId);
        }
        public void OnDataRead(ref ThreadInfo threadInfo)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead] Thread:" + threadInfo.threadID );
#endif
            this.dictionaryData.AddThreadInfo(ref threadInfo);
        }

        public void OnDataRead(ref Sample sample,bool isEndFrame)
        {
#if UTJ_CHECK
            if (!isEndFrame)
            {
                Debug.DebugLogWrite.Log("[OnDataRead] BeginSample:" + sample.id + "  " + sample.time);
            }
            else
            {
                Debug.DebugLogWrite.Log("[OnDataRead] EndSample:" + sample.id + "  " + sample.time);
            }
#endif
            if (!isEndFrame)
            {
                SampleWrappedData wrappedData = (SampleWrappedData)sample;
                BeginSample(ref wrappedData);
            }
            else
            {
                EndSample(ref sample);
            }            
        }

        public void OnDataRead(ref SampleWithInstanceID sampleWithInstanceID)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead] SampleWithInstanceID:" + sampleWithInstanceID.id + "  " + sampleWithInstanceID.time);
#endif
            SampleWrappedData wrappedData = (SampleWrappedData)sampleWithInstanceID;
            BeginSample(ref wrappedData);
        }

        static bool flag = false;
        public void OnDataRead(ref SampleWithMetadata sampleWithMetadata)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead] SampleWithMetadata:" + sampleWithMetadata.id + "   " + sampleWithMetadata.time);
#endif
            SampleWrappedData wrappedData = (SampleWrappedData)sampleWithMetadata;
            BeginSample(ref wrappedData);
        }

        public void OnDataRead(ref GCAlloc gcAlloc)
        {
            if (currentOutputFrame == null) { 
                ProfilerLogUtil.LogError(" currentOutputFrame is null at GCAlloc");
                return;
            }
            AllocatedGCMemory allocatedGCMemory = new AllocatedGCMemory();
            allocatedGCMemory.allocatedGCMemory = gcAlloc.size;
            var profilerSample = currentOutputFrame.currentProfilerSample;
            profilerSample.AddAllocatedGC(allocatedGCMemory);
            ProfilerSample.AddChildAllocToParent(profilerSample);
        }
        public void OnDataRead(ref Callstack callstack)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead] Callstack:" + callstack.framesCount);
#endif
            BinaryData.Thread.ProfileCallstack profileCallstack = new ProfileCallstack();
            for(int i = 0; i < callstack.framesCount && i < profileCallstack.stack.Length; ++i)
            {
                profileCallstack.stack[i] = callstack.frames[i];
            }
            profileCallstack.hash = callstack.hash;
            //
           this.currentOutputFrame.SetCallStackInfo(profileCallstack);

        }
        public void OnDataRead(ref AsyncMetadataAnchor asyncMetadataAnchor)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead]" + currentFrameIdx +" AsyncMetadataAnchor:" + asyncMetadataAnchor.asyncMetadataId);
#endif
            this.currentOutputFrame.BindAsyncMetadataAnchor(ref asyncMetadataAnchor);
        }
        public void OnDataRead(ref GPUSample gpuSample)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead]" + currentFrameIdx +" GPUSample:" + gpuSample.asyncMetadataId +"-" +gpuSample.elapsedGpuTimeInMicroSec);
#endif
            bool result = false;
            for ( int i = 0;; ++i)
            {
                OutputFrameInfo outputFrame = null;
                if( this.outputFrameDatas.TryGetValue( currentFrameIdx - (uint)i ,out outputFrame)){
                    result = outputFrame.SetGPUSample(ref gpuSample);
                    if( result) { break; }
                }
                else
                {
                    break;
                }
            }

#if UTJ_CHECK
            if (!result)
            {
                Debug.DebugLogWrite.Log("[SetGPUSample] NotFound asyncMetadataId:" +
                    gpuSample.asyncMetadataId + "::" + currentFrameIdx);
            }
#endif
        }
        public void OnDataRead(ref CleanupThread cleanupThread)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead] CleanupThread:" + cleanupThread.endTime );
#endif
//            outputFrameDatas

        }
        public void OnDataRead(ref UIEvents uiEvents)
        {

        }
        public void OnDataRead(ref UISystemCanvas uiSystemCanvas)
        {

        }
        public void OnDataRead(ref AudioInstanceData audioInstanceData)
        {

        }

        public void OnDataRead(ref MethodJitInfo methodJitInfo)
        {
            this.dictionaryData.AddMethodJitInfo(ref methodJitInfo);
        }

        public void OnDataRead(ref Protocol.FlowEvent flowEvent)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[OnDataRead] FlowEvent:" + flowEvent.flowEventType);
#endif

        }

        public void OnDataRead(ref AllProfilerStats allProfilerStats)
        {
            AllStatsConverter converter = new AllStatsConverter();
            var allStats = converter.ConvertFromRaw(allProfilerStats,sessionHeader.version);
            if (this.currentOutputFrame != null && this.currentOutputFrame.outputFrame != null)
            {
                this.currentOutputFrame.outputFrame.allStats = allStats;
            }
            else
            {
                ProfilerLogUtil.LogError(" currentOutputFrame is null at allProfilerStats");
            }
        }

        private void BeginSample( ref SampleWrappedData wrappedData)
        {
            if (currentOutputFrame == null)
            {
                ProfilerLogUtil.LogError(" currentOutputFrame is null at BeginSample at " +this.currentFrameIdx);
                return;
            }
            SamplerInfo samplerInfo = this.dictionaryData.GetSamplerInfo(wrappedData.sample.id);
            ProfilerSample outputSample = CreateProfilerSample( ref samplerInfo ,ref  wrappedData);

            int stackNum = this.sampleStackResolver.GetStackNum();
           
            if (currentOutputFrame != null)
            {
                currentOutputFrame.BeginProfilerSample(outputSample);
            }
            this.sampleStackResolver.BeginSample(ref wrappedData);

        }

        private void EndSample( ref Sample endData)
        {
            double scale = (this.sessionHeader.timeNumerator / this.sessionHeader.timeDenominator) / 1000.0f;
            SampleWrappedData startData = this.sampleStackResolver.EndSample();



            if (this.currentOutputFrame == null)
            {
                ProfilerLogUtil.LogError("currentOutputFrame is null at EndSample at " + this.currentFrameIdx);
            }
            else { 
                bool endSampleResult = this.currentOutputFrame.EndProfilerSample(ref startData, ref endData, scale,this.unityVersion);

                if (!endSampleResult && this.sampleStackResolver.HasSample(endData.id) )
                {
                    startData = this.sampleStackResolver.EndSample();
                    this.currentOutputFrame.EndProfilerSample(ref startData, ref endData, scale,this.unityVersion);
                }
            }

        }

        private ProfilerSample CreateProfilerSample(ref SamplerInfo info,ref SampleWrappedData wrappedData)
        {
            ProfilerSample dest = new ProfilerSample();
            dest.startTimeUS = wrappedData.sample.time;
            dest.profilerInfo = new ProfilerSample.ProfilerInformation();
            dest.profilerInfo.flags = info.flags;
            dest.profilerInfo.name = info.name;
            dest.profilerInfo.group = info.group;
            // sample withMetadata
            if( wrappedData.sampleType == SampleWrappedData.EType.SampleWithMetadata)
            {
                dest.metaDatas = ConvertMetadataValue(wrappedData.metadatas);
            }
            // sample withInstanceId
            else if( wrappedData.sampleType == SampleWrappedData.EType.SampleWithInstanceID)
            {
                dest.metaDatas = CreateInstanceIdMetadata(wrappedData.instanceID);
            }
            return dest;
        }
        private MetaData CreateInstanceIdMetadata(int instanceId)
        {
            
            MetaData metadata = new MetaData();
            metadata.metadatas = new List<MetaData.MetaDataValue>();

            MetaData.MetaDataValue metadataValue = new MetaData.MetaDataValue();
            metadataValue.type = (int)RawDataDefines.MetadataDescriptionType.kInstanceId;
            metadataValue.convertedObject = new InstanceId { id = instanceId };
            metadata.metadatas.Add(metadataValue);

            return metadata;
        }
        private MetaData ConvertMetadataValue(List<MetadataValue> metadataValues)
        {
            if(metadataValues == null) { return null; }

            MetaData metadata = new MetaData();
            metadata.metadatas = new List<MetaData.MetaDataValue>();

            foreach( var metadataValue in metadataValues)
            {
                MetaData.MetaDataValue val = new MetaData.MetaDataValue();
                val.convertedObject = metadataValue.val;
                val.type = (int)metadataValue.type;
                metadata.metadatas.Add(val);
            }

            return metadata;
        }
    }
}
