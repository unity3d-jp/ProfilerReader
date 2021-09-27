using System.Collections;
using System.Collections.Generic;

namespace UTJ.ProfilerReader.RawData.Protocol
{
    public class CategoryState
    {
        public ushort categoryID;
        public ushort flags;
        internal void Read(RawBinaryReader reader)
        {
            categoryID = reader.ReadUshort();
            flags = reader.ReadUshort();
            reader.Align();
        }
    }
}
