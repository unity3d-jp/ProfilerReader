
using System.Collections;


namespace UTJ.ProfilerReader.BinaryData.Stats
{
    // before unity 5.4
    public partial class AudioProfilerInfo
    {
        [StatsData(0,0,ProfilerDataStreamVersion.Unity54)]
        public int assetInstanceId;
        [StatsData(1, 0, ProfilerDataStreamVersion.Unity54)]
        public int objectInstanceId;
        [StatsData(2, 0, ProfilerDataStreamVersion.Unity54)]
        public int assetNameOffset;
        [StatsData(3, 0, ProfilerDataStreamVersion.Unity54)]
        public int objectNameOffset;
        [StatsData(4, 0, ProfilerDataStreamVersion.Unity54)]
        public int parentId;
        [StatsData(5, 0, ProfilerDataStreamVersion.Unity54)]
        public int uniqueId;
        [StatsData(6, 0, ProfilerDataStreamVersion.Unity54)]
        public int flags;
        [StatsData(7, 0, ProfilerDataStreamVersion.Unity54)]
        public int playCount;

        [StatsData(8, 0, ProfilerDataStreamVersion.Unity54)]
        public float distanceToListener;
        [StatsData(9, 0, ProfilerDataStreamVersion.Unity54)]
        public float volume;
        [StatsData(10, 0, ProfilerDataStreamVersion.Unity54)]
        public float audibility;
        [StatsData(11, 0, ProfilerDataStreamVersion.Unity54)]
        public float minDist;
        [StatsData(12, 0, ProfilerDataStreamVersion.Unity54)]
        public float maxDist;
        [StatsData(13, 0, ProfilerDataStreamVersion.Unity54)]
        public float time;
        [StatsData(14, 0, ProfilerDataStreamVersion.Unity54)]
        public float duration;
        [StatsData(15, 0, ProfilerDataStreamVersion.Unity54)]
        public float frequency;


    }
}