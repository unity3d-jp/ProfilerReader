using System.Collections;
using System.Collections.Generic;

namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct SessionHeader
    {
        const uint kSignature = 'U' << 24 | '3' << 16 | 'D' << 8 | 'P';
        const uint kVersion = 0x20171110;

        const uint kVersionFrom2019 = 0x20180306;
        // ツメツメにして間は空けない前提( 36 Byte)
        public uint signature;
        public byte isLittleEndian;
        public byte isAlignedMemoryAccess;
        public ushort platform;
        public uint version;

        public ulong timeNumerator;
        public ulong timeDenominator;
        public ulong mainThreadId;

        public int[] unityVersion;

        public void ReadData(byte[] buffer)
        {
            this.signature = UTJ.ProfilerReader.ProfilerLogUtil.GetUIntValue(buffer, 0);
            this.isLittleEndian = buffer[4];
            this.isAlignedMemoryAccess = buffer[5];
            this.platform = UTJ.ProfilerReader.ProfilerLogUtil.GetUshortValue(buffer, 6);
            this.version = UTJ.ProfilerReader.ProfilerLogUtil.GetUIntValue(buffer, 8);
            this.timeNumerator = UTJ.ProfilerReader.ProfilerLogUtil.GetULongValue(buffer, 12);
            this.timeDenominator = UTJ.ProfilerReader.ProfilerLogUtil.GetULongValue(buffer, 20);
            this.mainThreadId = UTJ.ProfilerReader.ProfilerLogUtil.GetULongValue(buffer, 28);
        }
        public void AddUnityVersionData(byte[] buffer)
        {
            const int versionTypeNum = 5;
            this.unityVersion = new int[versionTypeNum];
            for(int i = 0; i < versionTypeNum; ++i)
            {
                this.version = UTJ.ProfilerReader.ProfilerLogUtil.GetUIntValue(buffer, i*4);
            }
        }
        public bool CheckSignature()
        {
            return (signature == kSignature);
        }
    }
}