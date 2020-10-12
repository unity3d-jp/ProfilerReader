
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Thread
{
    // to 2017.2
    public class MetaDataOffset 
    {
        uint relatedSampleIndex;
        uint offset;

        public void Read(System.IO.Stream stream)
        {
            this.relatedSampleIndex = ProfilerLogUtil.ReadUint(stream);
            this.offset = ProfilerLogUtil.ReadUint(stream);
        }
    }


}