using System.Collections.Generic;
using UTJ.ProfilerReader.UnityType;

namespace UTJ.ProfilerReader.RawData.Protocol
{
    public struct SampleWithMetadata
    {
        const uint kInvalidSampleID = 0xffffffff;

        enum Flags
        {
            kNone = 0,
            kMonoSample = 1 << 0,
        };

        public byte flags;
        public uint id;
        public ulong time;
        public byte metadataCount;

        public List<MetadataValue> metadatas;

        private int readMetadataNum ;
        private byte[] currentBlob;
        private int currentReadBlobSize;
        RawDataDefines.MetadataDescriptionType currnetType;

        internal bool IsInvalid()
        {
            return (id == kInvalidSampleID);
        }
        internal void SetInvalid()
        {
            id = kInvalidSampleID;
        }

        internal bool Read(RawBinaryReader reader)
        {
            // read sample
            flags = reader.ReadByte();
            reader.Align();
            id = reader.ReadUint();
            time = reader.ReadUlong();

            metadataCount = reader.ReadByte();
            reader.Align();
            metadatas = new List<MetadataValue>(metadataCount);

            this.readMetadataNum = 0;

            for (int i = 0; i < metadataCount; ++i)
            {
                // to read more...
                if (!reader.CanTransfer())
                {
                    return false;
                }
                bool flag = ReadMetaData(reader);
                if(!flag) { 
                    return false;
                }
                this.readMetadataNum = i + 1;
            }
            return true;
        }

        internal bool ReadMore(RawBinaryReader reader)
        {
            for( int i = readMetadataNum; i< metadataCount; ++i)
            {
                // to read more...
                if (!reader.CanTransfer())
                {
                    return false;
                }
                if ( this.currentBlob != null ){
                    int left = currentBlob.Length - currentReadBlobSize;
                    int readSize = reader.ReadByteToArray(currentBlob, currentReadBlobSize, left);
                    currentReadBlobSize += readSize;
                    if( this.currentReadBlobSize == this.currentBlob.Length)
                    {
                        this.AddBlobValue(reader,this.currnetType, currentBlob);
                        this.currentBlob = null;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    bool flag = ReadMetaData(reader);
                    if (!flag) {
                        return false; 
                    }
                    this.readMetadataNum = i + 1;
                }
            }
            return true;
        }

        private void AddBlobValue(RawBinaryReader reader,
            RawDataDefines.MetadataDescriptionType metadataType , 
            byte[] readVal) 
        {

            if (metadataType == RawDataDefines.MetadataDescriptionType.kBlob8)
            {
                AddMetaDataValue(metadataType, readVal);
            }
            else if (metadataType == RawDataDefines.MetadataDescriptionType.kString)
            {
                AddMetaDataValue(metadataType, reader.ConvertToAsciiString(readVal));
            }
            else if (metadataType == RawDataDefines.MetadataDescriptionType.kString16)
            {
                AddMetaDataValue(metadataType, reader.ConvertToUtfString(readVal));
            }
            this.currentBlob = null;
        }


        private bool ReadMetaData(RawBinaryReader reader)
        {
            byte val = reader.ReadByte();
            reader.Align();
            RawDataDefines.MetadataDescriptionType metadataType = (RawDataDefines.MetadataDescriptionType)val;

            //Debug.DebugLogWrite.Log("  MetaDatatype " + metadataType);

            switch (metadataType)
            {
                case RawDataDefines.MetadataDescriptionType.kInstanceId:
                    {
                        int readVal = reader.ReadInt();
                        InstanceId instanceId = new InstanceId { id = readVal };
                        AddMetaDataValue(metadataType, instanceId);
                    }
                    break;
                case RawDataDefines.MetadataDescriptionType.kInt32:
                    {
                        var readVal = reader.ReadInt();
                        AddMetaDataValue(metadataType, readVal);
                    }
                    break;
                case RawDataDefines.MetadataDescriptionType.kUInt32:
                    {
                        var readVal = reader.ReadUint();
                        AddMetaDataValue(metadataType, readVal);
                    }
                    break;
                case RawDataDefines.MetadataDescriptionType.kInt64:
                    {
                        var readVal = reader.ReadLong();
                        AddMetaDataValue(metadataType, readVal);
                    }
                    break;
                case RawDataDefines.MetadataDescriptionType.kUInt64:
                    {
                        var readVal = reader.ReadUlong();
                        AddMetaDataValue(metadataType, readVal);
                    }
                    break;
                case RawDataDefines.MetadataDescriptionType.kFloat:
                    {
                        var readVal = reader.ReadFloat();
                        AddMetaDataValue(metadataType, readVal);
                    }
                    break;
                case RawDataDefines.MetadataDescriptionType.kDouble:
                    {
                        var readVal = reader.ReadDouble();
                        AddMetaDataValue(metadataType, readVal);
                    }
                    break;
                case RawDataDefines.MetadataDescriptionType.kString:
                case RawDataDefines.MetadataDescriptionType.kString16:
                case RawDataDefines.MetadataDescriptionType.kBlob8:
                    {
                        int size = reader.ReadInt();
                        byte[] readVal = new byte[size];
                        int readSize = reader.ReadByteToArray(readVal, 0, size);
                        if (readSize == size)
                        {
                            reader.Align();
                            this.AddBlobValue(reader, metadataType, readVal);
                        }
                        else
                        {
                            this.currentBlob = readVal;
                            this.currentReadBlobSize = readSize;
                            this.currnetType = metadataType;
                            return false;
                        }
                    }
                    break;
                case RawDataDefines.MetadataDescriptionType.kVec3:
                    {
                        var readX = reader.ReadFloat();
                        var readY = reader.ReadFloat();
                        var readZ = reader.ReadFloat();
                        var vec3 = new Vector3 { x = readX, y = readY, z = readZ };
                        AddMetaDataValue(metadataType, vec3);
                    }
                    break;
                default:
                    ProfilerLogUtil.LogError("strange metadataType " +
                        metadataType + " position:" + reader.Position + "/" + reader.Length +
                        " metadataCount:"+this.metadataCount);
#if UTJ_CHECK
                    foreach( var metadata in metadatas)
                    {
                        ProfilerLogUtil.LogError( "Metadata:"+ metadata.type );
                    }
#endif    
                    break;
            }
            return true;
        }

        private void AddMetaDataValue( RawDataDefines.MetadataDescriptionType type , System.Object val)
        {
            metadatas.Add( new MetadataValue(type,val) );
        }

        public static explicit operator SampleWrappedData(SampleWithMetadata src)
        {
            SampleWrappedData data = new SampleWrappedData();
            var sample = new Sample();
            sample.id = src.id;
            sample.time = src.time;
            sample.flags = src.flags;

            data.sampleType = SampleWrappedData.EType.SampleWithMetadata;
            data.sample = sample;
            data.metadataCount = src.metadataCount;
            data.metadatas = src.metadatas;
            return data;
        }
    }
}