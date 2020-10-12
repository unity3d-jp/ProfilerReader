using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct Sample
    {
        const uint kInvalidSampleID = 0xffffffff;

        enum Flags
        {
            kNone = 0,
            kMonoSample = 1 << 0,
        };

        public byte flags;
        public uint id;
        public ulong time;

        internal void Read(RawBinaryReader reader)
        {
            flags = reader.ReadByte();
            reader.Align();
            id = reader.ReadUint();
            time = reader.ReadUlong();
        }



        public static explicit operator SampleWrappedData(Sample src)
        {
            SampleWrappedData data = new SampleWrappedData();

            data.sampleType = SampleWrappedData.EType.Sample;
            data.sample = src;
            return data;
        }
        }
}