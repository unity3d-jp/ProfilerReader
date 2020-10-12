
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioProfilerGroupInfo
    {
#if false
        /*InstanceID*/ int assetInstanceId;
        /*InstanceID*/ int objectInstanceId;
        int assetNameOffset;
        int objectNameOffset;
        int parentId;
        int uniqueId;
        int flags;
        int playCount;
        float distanceToListener;
        float volume;
        float audibility;
        float minDist;
        float maxDist;
        float time;
        float duration;
        float frequency;
#endif

        internal void Read_0x20161226(System.IO.Stream stream)
        {
            assetInstanceId = ProfilerLogUtil.ReadInt(stream);
            objectInstanceId = ProfilerLogUtil.ReadInt(stream);
            assetNameOffset = ProfilerLogUtil.ReadInt(stream);
            objectNameOffset = ProfilerLogUtil.ReadInt(stream);

            parentId = ProfilerLogUtil.ReadInt(stream);
            uniqueId = ProfilerLogUtil.ReadInt(stream);
            flags = ProfilerLogUtil.ReadInt(stream);
            playCount = ProfilerLogUtil.ReadInt(stream);


            distanceToListener = ProfilerLogUtil.ReadFloat(stream);
            volume = ProfilerLogUtil.ReadFloat(stream);
            audibility = ProfilerLogUtil.ReadFloat(stream);
            minDist = ProfilerLogUtil.ReadFloat(stream);
            maxDist = ProfilerLogUtil.ReadFloat(stream);
            time = ProfilerLogUtil.ReadFloat(stream);
            duration = ProfilerLogUtil.ReadFloat(stream);
            frequency = ProfilerLogUtil.ReadFloat(stream);
        }
    }
}