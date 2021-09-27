using System.Collections;
using System.Collections.Generic;

namespace UTJ.ProfilerReader.RawData.Protocol
{
    public class CategoryInfo
    {
        public ushort categoryID;
        public ushort color;
        public ushort flags;
        public string name;

        internal void Read(RawBinaryReader reader)
        {
            categoryID = reader.ReadUshort();
            color = reader.ReadUshort();
            flags = reader.ReadUshort();
            name = reader.ReadStringAsTransfer();
            reader.Align();
        }
    }
}
