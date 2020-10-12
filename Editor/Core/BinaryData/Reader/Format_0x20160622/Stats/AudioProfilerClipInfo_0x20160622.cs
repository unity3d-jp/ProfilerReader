using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioProfilerClipInfo
    {
#if false
        /*InstanceID*/ int assetInstanceId;
        int assetNameOffset;
        int loadState;
        int internalLoadState;
        int age;
        int disposed;
        int numChannelInstances;
#endif
        internal void Read_0x20160622(System.IO.Stream stream)
        {
            assetInstanceId = ProfilerLogUtil.ReadInt(stream);
            assetNameOffset = ProfilerLogUtil.ReadInt(stream);
            loadState = ProfilerLogUtil.ReadInt(stream);
            internalLoadState = ProfilerLogUtil.ReadInt(stream);
            age = ProfilerLogUtil.ReadInt(stream);
            disposed = ProfilerLogUtil.ReadInt(stream);
            numChannelInstances = ProfilerLogUtil.ReadInt(stream);
        }
    }
}