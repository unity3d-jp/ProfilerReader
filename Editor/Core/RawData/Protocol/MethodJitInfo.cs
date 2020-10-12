
namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct MethodJitInfo
    {
        public ulong methodPtr;
        public ulong codeAddr;
        public uint codeSize;
        public string name;
        public string sourceFileName;
        public uint sourceFileLine;


        internal void Read(RawBinaryReader reader)
        {
            methodPtr = reader.ReadUlong();
            codeAddr = reader.ReadUlong();
            codeSize = reader.ReadUint();
            name = reader.ReadStringAsTransfer();
            reader.Align();
            sourceFileName = reader.ReadStringAsTransfer();
            reader.Align();
            sourceFileLine = reader.ReadUint();
            /*
            PROFILER_TRANSFER(methodPtr);
            PROFILER_TRANSFER(codeAddr);
            PROFILER_TRANSFER(codeSize);
            PROFILER_TRANSFER_ARRAY(name);
            transfer.Align();
            PROFILER_TRANSFER_ARRAY(sourceFileName);
            transfer.Align();
            PROFILER_TRANSFER(sourceFileLine);
             */
        }

    }
}