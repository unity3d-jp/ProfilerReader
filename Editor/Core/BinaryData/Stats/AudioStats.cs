
using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioStats
    {
        public const int kMaxPlatformDependentStats = 8;
        [StatsData(0)]
        public int totalAudioSourceCount;
        [StatsData(1)]
        public int playingSources;
        [StatsData(2)]
        public int pausedSources;

        [StatsData(4)]
        public int audioClipCount;

        [StatsData(5)]
        public int numSoundChannelInstances;
        [StatsData(6)]
        public int numSoundChannelHandles;

        [StatsData(7)]
        public int numSoundHandles;
        [StatsData(8)]
        public int numSoundHandleInstances;
        [StatsData(9)]
        public int numPendingSoundHandleInstances;
        [StatsData(10)]
        public int numLoadedSoundHandleInstance;
        [StatsData(11)]
        public int numDisposedSoundHandleInstances;
        [StatsData(12)]
        public int numPendingSoundInstanceUnloads;
        [StatsData(13)]
        public int numSoundChannelInstanceWeakPtrs;
        [StatsData(14)]
        public int numSoundHandleInstanceWeakPtrs;
        [StatsData(15,ProfilerDataStreamVersion.Unity56)]
        public int numSampleClipWeakPtrs; // from 5.6
        [StatsData(16)]
        public int numWeakPtrs;
        [StatsData(17)]
        public int numWeakPtrSharedData;
        [StatsData(18)]
        public int numWeakPtrSharedDataSoundChannel;
        [StatsData(19)]
        public int numWeakPtrSharedDataSoundHandle;
        [StatsData(20, ProfilerDataStreamVersion.Unity56)]
        public int numWeakPtrSharedDataSampleClip;// from 5.6

        [StatsData(21)]
        public int numFMODChannels;
        [StatsData(22)]
        public int numVFSHandles;

        [StatsData(23)]
        public int totalCPU;
        [StatsData(24)]
        public int dspCPU;
        [StatsData(25)]
        public int streamCPU;
        [StatsData(26)]
        public int otherCPU;

        [StatsData(27)]
        public int totalMemoryUsage;
        [StatsData(28)]
        public int streamingMemory;
        [StatsData(29)]
        public int sampleMemory;
        [StatsData(30)]
        public int channelMemory;
        [StatsData(31)]
        public int dspMemory;
        [StatsData(32)]
        public int extraDSPBufferMemory;
        [StatsData(33)]
        public int codecMemory;
        [StatsData(34)]
        public int recordMemory;
        [StatsData(35)]
        public int reverbMemory;
        [StatsData(36)]
        public int otherAudioBuffers;
        [StatsData(37)]
        public int otherMemory;

        [StatsData(100)]
        public int[] platformDependentStats = new int[kMaxPlatformDependentStats];

    }
}