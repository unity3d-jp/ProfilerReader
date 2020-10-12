using System.Collections;
using System.Collections.Generic;

namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct SampleWithInstanceID
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

        public int instanceID;

        internal void Read(RawBinaryReader reader)
        {
            flags = reader.ReadByte();
            reader.Align();
            id = reader.ReadUint();
            time = reader.ReadUlong();
            instanceID = reader.ReadInt();
        }

        public static explicit operator SampleWrappedData(SampleWithInstanceID src)
        {
            SampleWrappedData data = new SampleWrappedData();
            var sample = new Sample();
            sample.id = src.id;
            sample.time = src.time;
            sample.flags = src.flags;

            data.sampleType = SampleWrappedData.EType.SampleWithInstanceID;
            data.sample = sample;
            data.instanceID = src.instanceID;
            return data;
        }
    }
}