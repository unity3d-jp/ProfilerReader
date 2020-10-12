
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class ChartSample
    {
        internal void Read_0x20160622(System.IO.Stream stream)
        {

            rendering = ProfilerLogUtil.ReadFloat(stream);
            scripts = ProfilerLogUtil.ReadFloat(stream);
            physics = ProfilerLogUtil.ReadFloat(stream);
            gc = ProfilerLogUtil.ReadFloat(stream);
            vsync = ProfilerLogUtil.ReadFloat(stream);
            gi = ProfilerLogUtil.ReadFloat(stream);
            nativeMem = ProfilerLogUtil.ReadFloat(stream);
            others = ProfilerLogUtil.ReadFloat(stream);

            gpuOpaque = ProfilerLogUtil.ReadFloat(stream);
            gpuTransparent = ProfilerLogUtil.ReadFloat(stream);
            gpuShadows = ProfilerLogUtil.ReadFloat(stream);
            gpuPostProcess = ProfilerLogUtil.ReadFloat(stream);
            gpuDeferredPrePass = ProfilerLogUtil.ReadFloat(stream);
            gpuDeferredLighting = ProfilerLogUtil.ReadFloat(stream);
            gpuOther = ProfilerLogUtil.ReadFloat(stream);

            hasGPUProfiler = ProfilerLogUtil.ReadInt(stream);
        }
    }
}