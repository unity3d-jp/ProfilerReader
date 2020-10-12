using System.Collections;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct MetadataValue
    {
        public RawDataDefines.MetadataDescriptionType type;
        public System.Object val;

        public MetadataValue(RawDataDefines.MetadataDescriptionType t,System.Object v)
        {
            type = t;
            val = v;
        }

        public bool GetUintValue(out uint result)
        {
            switch(type)
            {
                case RawDataDefines.MetadataDescriptionType.kInt32:
                    {
                        int tmp = (int)val;
                        result = (uint)tmp;                        
                    }
                    return true;
                case RawDataDefines.MetadataDescriptionType.kInt64:
                    {
                        long tmp = (long)val;
                        result = (uint)tmp;
                    }
                    return true;
                case RawDataDefines.MetadataDescriptionType.kUInt32:
                    {
                        result = (uint)val;
                    }
                    return true;
                case RawDataDefines.MetadataDescriptionType.kUInt64:
                    {
                        ulong tmp = (ulong)val;
                        result = (uint)tmp;
                    }
                    return true;
            }
            result = 0;
            return false;
        }
    }
}