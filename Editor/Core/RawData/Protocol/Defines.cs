using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.RawData.Protocol
{
    public class RawDataDefines
    {
        public enum MessageType : ushort
        {
            // Profiler state change: enabled/disabled, mode, etc.
            kProfilerState = 0,
            // Sampler information - name, metadata layout
            kSamplerInfo = 1,
            // void* to function name
            kCallstack = 3,
            // Profiler stats (written on a main thread)
            kAllProfilerStats = 4,
            // Combined audio stats
            kAudioInstanceData = 5,
            // UI stats
            kUISystemCanvas = 6,
            kUIEvents = 7,

            // from 2019.3
            kMethodJitInfo = 8,
            kGlobalMessagesCount = 32,
            // Thread specific data below
            // Thread name and id
            kThreadInfo = kGlobalMessagesCount + 1,
            // Frame boundary
            kFrame = kGlobalMessagesCount + 2,
            kBeginSample = kGlobalMessagesCount + 4,
            kEndSample = kGlobalMessagesCount + 5,
            kSample = kGlobalMessagesCount + 6,

            kBeginSampleWithInstanceID = kGlobalMessagesCount + 7,
            kEndSampleWithInstanceID = kGlobalMessagesCount + 8,
            kSampleWithInstanceID = kGlobalMessagesCount + 9,

            kBeginSampleWithMetadata = kGlobalMessagesCount + 10,
            kEndSampleWithMetadata = kGlobalMessagesCount + 11,
            kSampleWithMetadata = kGlobalMessagesCount + 12,
            kGCAlloc = kGlobalMessagesCount + 20,
            // Thread local async metadata anchor (e.g. GPUSamples generated on render thread.)
            kLocalAsyncMetadataAnchor = kGlobalMessagesCount + 21,
            kLocalAsyncMetadata = kGlobalMessagesCount + 22,
            kLocalGPUSample = kGlobalMessagesCount + 23,
            kCleanupThread = kGlobalMessagesCount + 24,
            kFlowEvent = kGlobalMessagesCount + 25,// from 20180306
        };

        public enum MetadataDescriptionType : byte
        {
            kNone = 0,
            kInstanceId = 1,
            kInt32 = 2,
            kUInt32 = 3,
            kInt64 = 4,
            kUInt64 = 5,
            kFloat = 6,
            kDouble = 7,
            kString = 8,
            kString16 = 9,
            kVec3 = 10,
            kBlob8 = 11,
            kCount
        };
        public const int InstanceID_None = 0;

        public static readonly string EmitFramemetataSample = "Profiler.FrameMetadata";
    }
}
