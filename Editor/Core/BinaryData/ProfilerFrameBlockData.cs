using System;
using System.Collections;


namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public partial class ProfilerFrameBlockData
        {
            public const int HeaderSize = 16;
            private int littleEndian;
            public int dataVersion { get; set; }
            public int frameDataSize { get; set; }
            public int threadCount { get; set; }

            // from Unity 2022
            public uint[] unityVersion { get; set; }

            public ProfilerFrameData frameData { get; set; }
        }
    }
}
