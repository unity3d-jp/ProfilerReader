using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct UnityNativeTypeInfo
    {
        public uint runtimeTypeId;
        public uint baseRuntimeTypeId;
        public string name;


        internal void Read(RawBinaryReader reader)
        {
            runtimeTypeId = reader.ReadUint();
            baseRuntimeTypeId = reader.ReadUint();
            name = reader.ReadStringAsTransfer();
            reader.Align();
        }
    }
}