using System.Collections;


namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioProfilerInfo
    {
        internal void Read_0x20140736(System.IO.Stream stream)
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