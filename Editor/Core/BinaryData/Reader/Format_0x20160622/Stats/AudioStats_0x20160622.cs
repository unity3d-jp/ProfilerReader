using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class AudioStats
    {

        internal void Read_0x20160622(System.IO.Stream stream)
        {
            totalAudioSourceCount = ProfilerLogUtil.ReadInt(stream);
            playingSources = ProfilerLogUtil.ReadInt(stream);
            pausedSources = ProfilerLogUtil.ReadInt(stream);

            audioClipCount = ProfilerLogUtil.ReadInt(stream);

            numSoundChannelInstances = ProfilerLogUtil.ReadInt(stream);
            numSoundChannelHandles = ProfilerLogUtil.ReadInt(stream);

            numSoundHandles = ProfilerLogUtil.ReadInt(stream);
            numSoundHandleInstances = ProfilerLogUtil.ReadInt(stream);
            numPendingSoundHandleInstances = ProfilerLogUtil.ReadInt(stream);
            numLoadedSoundHandleInstance = ProfilerLogUtil.ReadInt(stream);
            numDisposedSoundHandleInstances = ProfilerLogUtil.ReadInt(stream);
            numPendingSoundInstanceUnloads = ProfilerLogUtil.ReadInt(stream);
            numSoundChannelInstanceWeakPtrs = ProfilerLogUtil.ReadInt(stream);
            numSoundHandleInstanceWeakPtrs = ProfilerLogUtil.ReadInt(stream);
            numWeakPtrs = ProfilerLogUtil.ReadInt(stream);
            numWeakPtrSharedData = ProfilerLogUtil.ReadInt(stream);
            numWeakPtrSharedDataSoundChannel = ProfilerLogUtil.ReadInt(stream);
            numWeakPtrSharedDataSoundHandle = ProfilerLogUtil.ReadInt(stream);

            numFMODChannels = ProfilerLogUtil.ReadInt(stream);
            numVFSHandles = ProfilerLogUtil.ReadInt(stream);

            totalCPU = ProfilerLogUtil.ReadInt(stream);
            dspCPU = ProfilerLogUtil.ReadInt(stream);
            streamCPU = ProfilerLogUtil.ReadInt(stream);
            otherCPU = ProfilerLogUtil.ReadInt(stream);

            totalMemoryUsage = ProfilerLogUtil.ReadInt(stream);
            streamingMemory = ProfilerLogUtil.ReadInt(stream);
            sampleMemory = ProfilerLogUtil.ReadInt(stream);
            channelMemory = ProfilerLogUtil.ReadInt(stream);
            dspMemory = ProfilerLogUtil.ReadInt(stream);
            extraDSPBufferMemory = ProfilerLogUtil.ReadInt(stream);
            codecMemory = ProfilerLogUtil.ReadInt(stream);
            recordMemory = ProfilerLogUtil.ReadInt(stream);
            reverbMemory = ProfilerLogUtil.ReadInt(stream);
            otherAudioBuffers = ProfilerLogUtil.ReadInt(stream);
            otherMemory = ProfilerLogUtil.ReadInt(stream);

            for (int i = 0; i < kMaxPlatformDependentStats; ++i)
            {
                this.platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }

        }
    }
}