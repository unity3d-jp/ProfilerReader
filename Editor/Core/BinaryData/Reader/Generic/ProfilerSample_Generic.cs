
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

            internal void Read_Generic(System.IO.Stream stream ,uint version)
            {
                if (version >= ProfilerDataStreamVersion.Unity2019_3)
                {
                    this.timeUS = ProfilerLogUtil.ReadFloat(stream);
                    this.startTimeUS = ProfilerLogUtil.ReadULong(stream);
                }
                else
                {
                    this.timeUS = ProfilerLogUtil.ReadUint(stream);
                    this.startTimeUS = ProfilerLogUtil.ReadUint(stream);
                }
                this.nbChildren = ProfilerLogUtil.ReadInt(stream);
            }

            internal void ReadInformation_Generic(System.IO.Stream stream,uint version)
            {
                int isExistData = ProfilerLogUtil.ReadInt(stream);
                if (isExistData == 0) { return; }
                profilerInfo = new ProfilerInformation();
                profilerInfo.Read(stream);
                if( version >= ProfilerDataStreamVersion.Unity2019_3)
                {
                    int paramCount = ProfilerLogUtil.ReadInt(stream);

                    for (int i = 0; i < paramCount; ++i)
                    {
                        int type = ProfilerLogUtil.ReadInt(stream);
                        string name = ProfilerLogUtil.ReadString(stream);
                    }
                }
            }
        }
    }
}
