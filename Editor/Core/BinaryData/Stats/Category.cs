
using System.Collections;


namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class Category
    {
        public uint categoryId;
        public uint colorIndex;
        public uint flags;
        public string name;

        public void Read(System.IO.Stream stream, uint version)
        {
            this.categoryId = ProfilerLogUtil.ReadUint(stream);
            this.colorIndex = ProfilerLogUtil.ReadUint(stream);
            this.flags = ProfilerLogUtil.ReadUint(stream);
            this.name = ProfilerLogUtil.ReadString(stream);
        }

    }
}