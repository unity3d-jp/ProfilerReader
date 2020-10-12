
using System.Text;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Thread;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        // Base profiler sample with CPU times;
        // minimal amount of actual per-sample data.
        public partial class ProfilerSample
        {

            internal void Read_0x20170929(System.IO.Stream stream)
            {
                this.timeUS = ProfilerLogUtil.ReadUint(stream);
                this.startTimeUS = ProfilerLogUtil.ReadUint(stream);
                this.nbChildren = ProfilerLogUtil.ReadInt(stream);
            }

            internal void ReadInformation_0x20170929(System.IO.Stream stream)
            {
                int isExistData = ProfilerLogUtil.ReadInt(stream);
                if (isExistData == 0) { return; }
                profilerInfo = new ProfilerInformation();
                profilerInfo.Read(stream);
            }
        }
    }
}
