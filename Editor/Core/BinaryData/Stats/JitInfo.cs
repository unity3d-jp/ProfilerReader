using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public class JitInfo 
    {
        public ulong codeAddr;
        public uint size;
        public uint sourceFileLine;
        public string name;
        public string sourceFileName;

        public void Read(System.IO.Stream stream)
        {
            this.name = ProfilerLogUtil.ReadString(stream);
            this.sourceFileName = ProfilerLogUtil.ReadString(stream);

            this.codeAddr = ProfilerLogUtil.ReadUint(stream);
            this.size = ProfilerLogUtil.ReadUint(stream);
            this.sourceFileLine = ProfilerLogUtil.ReadUint(stream);
        }

        public class CompareByAddr : IComparer<JitInfo>
        {
            public int Compare(JitInfo x, JitInfo y)
            {
                if(x.codeAddr > y.codeAddr)
                {
                    return 1;
                }
                else if(x.codeAddr < y.codeAddr)
                {
                    return -1;
                }else
                {
                    return 0;
                }
            }
        }
    }
}