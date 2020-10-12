using System.IO;
namespace UTJ.ProfilerReader.BinaryData.Stats
{
    // from 2017.1
    public class EventMarker
    {
        public int objectInstanceId { get; set; }
        public int nameOffset { get; set; }
        public int frame { get; set; }

        public void Read(Stream stream)
        {
            objectInstanceId = ProfilerLogUtil.ReadInt(stream);
            nameOffset = ProfilerLogUtil.ReadInt(stream);
            frame = ProfilerLogUtil.ReadInt(stream);
        }
    }
}