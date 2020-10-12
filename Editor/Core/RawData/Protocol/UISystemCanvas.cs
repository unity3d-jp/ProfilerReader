using System.Collections;
using System.Collections.Generic;

namespace UTJ.ProfilerReader.RawData.Protocol
{

    public struct UISystemCanvas
    {
        public byte[] profilerInfo;
        public byte[] systemCanvasInfo;

        internal void Read(RawBinaryReader reader)
        {
            profilerInfo = reader.ReadByteArray();
            reader.Align();
            systemCanvasInfo = reader.ReadByteArray();
            reader.Align();
        }
    }
}
