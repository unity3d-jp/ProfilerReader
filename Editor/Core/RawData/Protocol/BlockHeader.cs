using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.RawData.Protocol
{

    public struct BlockHeader
    {
        const uint kSignature = 0xB10C7EAD;
        const ulong kGlobalThreadId = 18446744073709551615;
        // ツメツメにして間は空けない前提( 20 Byte)
        public uint signature;
        public uint blockId;
        public ulong threadId;
        public uint length;

        public bool ReadData(byte[] buffer, bool isLitteleEndian)
        {
            this.signature = UTJ.ProfilerReader.ProfilerLogUtil.GetUIntValue(buffer, 0);
            this.blockId = UTJ.ProfilerReader.ProfilerLogUtil.GetUIntValue(buffer, 4);
            this.threadId = UTJ.ProfilerReader.ProfilerLogUtil.GetULongValue(buffer, 8);
            this.length = UTJ.ProfilerReader.ProfilerLogUtil.GetUIntValue(buffer, 16);
            return (signature == kSignature);
        }
        
        public static bool IsGlobalThread( ulong thId)
        {
            return (thId == kGlobalThreadId);
        }
    };
}