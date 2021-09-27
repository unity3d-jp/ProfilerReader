using System.IO;
using System.Collections.Generic;
using UTJ.ProfilerReader.RawData.Protocol;

namespace UTJ.ProfilerReader.RawData
{


    public interface RawDataBehaviour
    {

        void OnSessionStart(ref SessionHeader sessionHeader);

        void OnBlockStart( ulong threadId );

        void OnGlobalDataRead(ref ProfilerState profilerState);
        void OnGlobalDataRead(ref ThreadInfo threadInfo);
        void OnGlobalDataRead(ref SamplerInfo sampleInfo);

        void OnDataRead(ref Frame frame);
        void OnDataRead(ref ThreadInfo threadInfo);

        void OnDataRead(ref Sample sample, bool isEndFrame);

        void OnDataRead(ref SampleWithInstanceID sampleWithInstanceID);

        void OnDataRead(ref SampleWithMetadata sampleWithMetadata);

        void OnDataRead(ref GCAlloc gcAlloc);
        void OnDataRead(ref Callstack callstack);
        void OnDataRead(ref AsyncMetadataAnchor asyncMetadataAnchor);
        void OnDataRead(ref GPUSample gpuSample);
        void OnDataRead(ref CleanupThread cleanupThread);
        void OnDataRead(ref UIEvents uiEvents);
        void OnDataRead(ref UISystemCanvas uiSystemCanvas);
        void OnDataRead(ref AudioInstanceData audioInstanceData);
        void OnDataRead(ref AllProfilerStats allProfilerStats);
        void OnDataRead(ref FlowEvent flowEvent);

        void OnDataRead(ref CategoryInfo categoryInfo);
        void OnDataRead(ref CategoryState categoryState);

        void OnDataRead(ref MethodJitInfo methodJitInfo);
    }
}