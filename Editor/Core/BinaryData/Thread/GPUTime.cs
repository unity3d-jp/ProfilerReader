
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Thread
{

    public class GPUTime
    {
        public enum GpuSection:int
        {
            kGPUSectionOther = 0,
            kGPUSectionOpaquePass = 1,
            kGPUSectionTransparentPass = 2,
            kGPUSectionShadowPass = 3,
            kGPUSectionDeferredPrePass = 4,
            kGPUSectionDeferredLighting = 5,
            kGPUSectionMotionVectors = 6,
            kGPUSectionPostProcess = 7
        };
        public const int SECTION_NUM = 8;

        public uint relatedSampleIndex;
        //GfxTimerQuery* timerQuery;
        public int gpuTimeInMicroSec;
        public int gpuSection;


        public void Read(System.IO.Stream stream)
        {
            gpuTimeInMicroSec = ProfilerLogUtil.ReadInt(stream);
            relatedSampleIndex = ProfilerLogUtil.ReadUint(stream);
            int section = ProfilerLogUtil.ReadInt(stream);
            gpuSection = section;
        }
    };
}