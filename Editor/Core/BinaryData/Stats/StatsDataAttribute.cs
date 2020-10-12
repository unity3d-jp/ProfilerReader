using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public class StatsDataAttribute : System.Attribute
    {

        private uint m_minSupportVersion;
        private uint m_maxSupportVersion;
        private int m_sortParam;

        public uint MinSupportVersion
        {
            get { return m_minSupportVersion; }
        }
        public uint MaxSupportVersion
        {
            get { return m_maxSupportVersion; }
        }
        public int SortParam
        {
            get { return m_sortParam; }
        }


        public StatsDataAttribute(int sortParam)
        {
            m_sortParam = sortParam;
            m_minSupportVersion = 0;
            m_maxSupportVersion = 0xffffffff;
        }
        public StatsDataAttribute(int sortParam, uint minVer)
        {
            m_sortParam = sortParam;
            m_minSupportVersion = minVer;
            m_maxSupportVersion = 0xffffffff;
        }
        public StatsDataAttribute(int sortParam, uint minVer, uint maxVer)
        {
            m_sortParam = sortParam;
            m_minSupportVersion = minVer;
            m_maxSupportVersion = maxVer;
        }

    }
}