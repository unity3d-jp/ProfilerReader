using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioProfilerDSPInfo
    {
#if false
        uint id;
        uint target;
        uint targetPort;
        uint numChannels;
        uint nameOffset;
        float weight;
        float cpuLoad;
        float level1;
        float level2;
        uint numLevels;
        uint flags;
#endif


        internal void Read_0x20160622(System.IO.Stream stream)
        {

            id = ProfilerLogUtil.ReadUint(stream);
            target = ProfilerLogUtil.ReadUint(stream);
            targetPort = ProfilerLogUtil.ReadUint(stream);
            numChannels = ProfilerLogUtil.ReadUint(stream);
            nameOffset = ProfilerLogUtil.ReadUint(stream);

            weight = ProfilerLogUtil.ReadFloat(stream);
            cpuLoad = ProfilerLogUtil.ReadFloat(stream);
            level1 = ProfilerLogUtil.ReadFloat(stream);
            level2 = ProfilerLogUtil.ReadFloat(stream);


            numLevels = ProfilerLogUtil.ReadUint(stream);
            flags = ProfilerLogUtil.ReadUint(stream);
        }
    }
}