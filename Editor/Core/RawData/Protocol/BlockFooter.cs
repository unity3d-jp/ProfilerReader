using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.RawData.Protocol
{

    public struct BlockFooter
    {
        const uint kSignature = 0xB10CF007;
        // ツメツメにして間は空けない前提( 8 Byte)
        public uint nextBlockId;
        public uint signature;

        public bool ReadData(byte[] buffer, bool isLitteleEndian)
        {
            this.nextBlockId = UTJ.ProfilerReader.ProfilerLogUtil.GetUIntValue(buffer, 0);
            this.signature = UTJ.ProfilerReader.ProfilerLogUtil.GetUIntValue(buffer, 4);
            return (signature == kSignature);
        }
    }
}
