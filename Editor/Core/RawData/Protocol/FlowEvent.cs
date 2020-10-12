using System.Collections;
using System.Collections.Generic;

namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct FlowEvent
    {
        
        public byte flowEventType; // UnityProfilerFlowEventType
        public uint flowId;
        internal void Read(RawBinaryReader reader)
        {
            flowEventType = reader.ReadByte();
            reader.Align();
            flowId = reader.ReadUint();
        }

    }
}