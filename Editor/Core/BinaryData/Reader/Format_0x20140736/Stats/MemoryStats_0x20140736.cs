using System.Collections;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public partial class MemoryStats
    {
        internal void Read_0x20140736(System.IO.Stream stream)
        {
            this.bytesUsedTotal = ProfilerLogUtil.ReadInt(stream) * 1024;

            this.bytesUsedUnity = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesUsedMono = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesUsedGFX = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesUsedFMOD = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesUsedProfiler = ProfilerLogUtil.ReadInt(stream) * 1024;

            this.bytesReservedTotal = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesReservedUnity = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesReservedMono = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesReservedGFX = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesReservedFMOD = ProfilerLogUtil.ReadInt(stream) * 1024;
            this.bytesReservedProfiler = ProfilerLogUtil.ReadInt(stream) * 1024;

            this.bytesVirtual = ProfilerLogUtil.ReadInt(stream) * 1024;


            textureCount = ProfilerLogUtil.ReadInt(stream);
            textureBytes = ProfilerLogUtil.ReadInt(stream);
            meshCount = ProfilerLogUtil.ReadInt(stream);
            meshBytes = ProfilerLogUtil.ReadInt(stream);
            materialCount = ProfilerLogUtil.ReadInt(stream);
            materialBytes = ProfilerLogUtil.ReadInt(stream);
            animationClipCount = ProfilerLogUtil.ReadInt(stream);
            animationClipBytes = ProfilerLogUtil.ReadInt(stream);
            audioCount = ProfilerLogUtil.ReadInt(stream);
            audioBytes = ProfilerLogUtil.ReadInt(stream);
            assetCount = ProfilerLogUtil.ReadInt(stream);
            sceneObjectCount = ProfilerLogUtil.ReadInt(stream);
            gameObjectCount = ProfilerLogUtil.ReadInt(stream);
            totalObjectsCount = ProfilerLogUtil.ReadInt(stream);

            int classSize = ProfilerLogUtil.ReadInt(stream);

            int idx = 0;
            for (int i = 0; i < classSize + 1; ++i )
            {
                idx = ProfilerLogUtil.ReadInt(stream);
                if (idx == -1)
                {
                    break;
                }
                int param = ProfilerLogUtil.ReadInt(stream);
            }


            for (int i = 0; i < kMaxPlatformDependentStats; i++)
            {
                platformDependentStats[i] = ProfilerLogUtil.ReadInt(stream);
            }
        }
    }
}