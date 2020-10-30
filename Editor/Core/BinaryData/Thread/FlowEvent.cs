
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Thread
{

    public class FlowEvent
    {
        uint relatedSampleIndex;
        uint flowId;
        byte flowEventType;
        public void Read(System.IO.Stream stream)
        {
            relatedSampleIndex = ProfilerLogUtil.ReadUint(stream);
            flowId = ProfilerLogUtil.ReadUint(stream);
            uint val = ProfilerLogUtil.ReadUint(stream);
            flowEventType = (byte)val;
        }
    };
}