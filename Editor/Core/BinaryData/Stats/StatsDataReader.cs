using System.Collections;
using System.Collections.Generic;



namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public class StatsDataReader : IStatsStream
    {
        private System.IO.Stream m_stream;
        private int readNum = 0;
        public StatsDataReader( System.IO.Stream stream)
        {
            m_stream = stream;
        }
        public int ReadInt()
        {
            ++readNum;
            return ProfilerLogUtil.ReadInt(m_stream);
        }
        public float ReadFloat()
        {
            ++readNum;
            return ProfilerLogUtil.ReadFloat(m_stream);
        }
        public uint ReadUint()
        {
            ++readNum;
            return ProfilerLogUtil.ReadUint(m_stream);
        }
    }
}