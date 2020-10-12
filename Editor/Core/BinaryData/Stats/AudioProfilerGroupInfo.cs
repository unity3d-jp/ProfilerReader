
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioProfilerGroupInfo
    {

        /*InstanceID*/
        [StatsData(0)]
        public int assetInstanceId;
        /*InstanceID*/
        [StatsData(1)]
        public int objectInstanceId;
        [StatsData(2)]
        public int assetNameOffset;
        [StatsData(3)]
        public int objectNameOffset;
        [StatsData(4)]
        public int parentId;
        [StatsData(5)]
        public int uniqueId;
        [StatsData(6)]
        public int flags;
        [StatsData(7)]
        public int playCount;
        [StatsData(8)]
        public float distanceToListener;
        [StatsData(9)]
        public float volume;
        [StatsData(10)]
        public float audibility;
        [StatsData(11)]
        public float minDist;
        [StatsData(12)]
        public float maxDist;
        [StatsData(13)]
        public float time;
        [StatsData(14)]
        public float duration;
        [StatsData(15)]
        public float frequency;
    }
}