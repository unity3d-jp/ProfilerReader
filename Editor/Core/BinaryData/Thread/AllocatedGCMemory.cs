
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Thread
{

    // Managed GC memory allocated during the call.
    public class AllocatedGCMemory
    {
        public uint relatedSampleIndex { get; set; }
        public uint allocatedGCMemory { get; set; }

        public void Read(System.IO.Stream stream)
        {
            this.relatedSampleIndex = ProfilerLogUtil.ReadUint(stream);
            this.allocatedGCMemory = ProfilerLogUtil.ReadUint(stream);
        }
    };
}
