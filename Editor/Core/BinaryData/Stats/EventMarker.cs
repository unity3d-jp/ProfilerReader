using System.IO;
namespace UTJ.ProfilerReader.BinaryData.Stats
{
    // from 2017.1
    public class EventMarker
    {
        public ulong objectInstanceId {  get; set; } 

        public int objectInstanceIdAs32Bit
        {
            get
            {
                return ProfilerLogUtil.ConvertUlongToInt(objectInstanceId);
            }
        }
        public int nameOffset { get; set; }
        public int frame { get; set; }

        public void Read(Stream stream,uint version)
        {
            if (version >= ProfilerDataStreamVersion.Unity6000_5)
            {
                objectInstanceId = ProfilerLogUtil.ReadULong(stream);
            }
            else
            {
                int id = ProfilerLogUtil.ReadInt(stream);
                objectInstanceId = ProfilerLogUtil.ConvertIntToUlong(id);

            }
            nameOffset = ProfilerLogUtil.ReadInt(stream);
            frame = ProfilerLogUtil.ReadInt(stream);
        }
    }
}