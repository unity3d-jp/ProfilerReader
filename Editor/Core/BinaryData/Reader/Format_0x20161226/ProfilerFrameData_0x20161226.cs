
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public partial class ProfilerFrameData
        {
            internal bool Read_0x20161226(System.IO.Stream stream, int frameSize, int threadCount, int index)
            {
                this.frameIndex = index;

                long before = stream.Position;

                if (stream.Length - before < frameSize || frameSize <= 0)
                {
                    ProfilerLogUtil.LogError("frame data maybe broken.");
                    return false;
                }

                
                int endian = ProfilerLogUtil.ReadInt(stream);
                int dataVersion = ProfilerLogUtil.ReadInt(stream);
                

                //
                this.frameIndexFromFile = ProfilerLogUtil.ReadInt(stream);
                this.realFrame = ProfilerLogUtil.ReadInt(stream);
                m_StartTimeUS.ReadAsInt(stream);

                this.m_TotalCPUTimeInMicroSec = ProfilerLogUtil.ReadInt(stream);
                this.m_TotalGPUTimeInMicroSec = ProfilerLogUtil.ReadInt(stream);

                this.allStats = new AllProfilerStats();
                allStats.Read_0x20161226(stream);

                // AudioInstance
                int audioIncentanceGroupInfoNum = ProfilerLogUtil.ReadInt(stream);
                m_AudioInstanceGroupInfo = new List<AudioProfilerGroupInfo>(audioIncentanceGroupInfoNum);
                for( int i = 0 ; i < audioIncentanceGroupInfoNum; ++ i ){
                    AudioProfilerGroupInfo audioGroupInfo = new AudioProfilerGroupInfo();
                    audioGroupInfo.Read_0x20161226( stream);
                    m_AudioInstanceGroupInfo.Add( audioGroupInfo);
                }
                int audioDspInfoNum =  ProfilerLogUtil.ReadInt(stream);
                m_AudioInstanceDSPInfo = new List<AudioProfilerDSPInfo>(audioDspInfoNum);
                for( int i = 0 ; i < audioDspInfoNum; ++ i){
                    AudioProfilerDSPInfo dspInfo = new AudioProfilerDSPInfo();
                    dspInfo.Read_0x20161226(stream);
                    m_AudioInstanceDSPInfo.Add(dspInfo);
                }
                /* not saved
                int clipInfoNum =  ProfilerLogUtil.ReadInt(stream);
                m_AudioInstanceClipInfo = new List<AudioProfilerClipInfo>( clipInfoNum);
                for (int i = 0; i < clipInfoNum; ++i)
                {
                    AudioProfilerClipInfo clipInfo = new AudioProfilerClipInfo();
                    clipInfo.Read_0x20161226(stream);
                    m_AudioInstanceClipInfo.Add(clipInfo);
                }
                 */

                int nameSize = ProfilerLogUtil.ReadInt(stream);
                byte[] names = new byte[nameSize];
                stream.Read(names, 0, nameSize);
                m_AudioInstanceNames = "";
                for (int i = 0; i < names.Length; ++i)
                {
                    if (names[i] == 0)
                    {
                        m_AudioInstanceNames += "\n";
                        continue;
                    }
                    m_AudioInstanceNames += (char)names[i];
                }

                // thread Data
                this.m_ThreadCount = ProfilerLogUtil.ReadInt(stream);
                m_ThreadData = new List<ThreadData>(m_ThreadCount);
                for (int i = 0; i < m_ThreadCount; ++i)
                {
                    ThreadData threadData = new ThreadData();
                    threadData.Read_0x20161226(stream);
                    m_ThreadData.Add(threadData);
                }
                uint end = ProfilerLogUtil.ReadUint(stream);
                if ((end != EndCode))
                {
                    ProfilerLogUtil.LogError("EndCode was wrong " + end);
                    ProfilerLogUtil.LogError("frameSize : " + frameSize + " , readSize:" + (stream.Position - before));
                }
                return true;
            }

        }
    }
}
