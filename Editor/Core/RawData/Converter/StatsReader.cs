using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Converter
{
    public class StatsReader : UTJ.ProfilerReader.BinaryData.Stats.IStatsStream
    {
        private int[] m_data;
        private int m_readerIdx;
        
        public StatsReader(int[] data)
        {
            m_readerIdx = 0;
            m_data = data;
        }

        public int ReadInt()
        {
            if( m_readerIdx >= m_data.Length)
            {
                Debug.DebugLogWrite.Log("Error " + m_readerIdx + "/" + m_data.Length);
            }
            int val = m_data[m_readerIdx];
            ++m_readerIdx;
            return val;
        }
        public float ReadFloat()
        {
            float val = m_data[m_readerIdx];
            ++m_readerIdx;
            return val;
        }
        public uint ReadUint()
        {
            uint val = (uint)m_data[m_readerIdx];
            ++m_readerIdx;
            return val;
        }
        
    }

}