
using System.Collections.Generic;
using UTJ.ProfilerReader.RawData.Protocol;
using UTJ.ProfilerReader.UnityType;

namespace UTJ.ProfilerReader.BinaryData.Thread
{
    public class MetaData
    {
        public uint relatedSampleIndex;
        public List<MetaDataValue> metadatas;

        public class MetaDataValue
        {
            public int type;
            private byte[] val;
            public object convertedObject;


            public void Read(System.IO.Stream stream)
            {
                this.type = ProfilerLogUtil.ReadInt(stream);
                int tmpArrSize = ProfilerLogUtil.ReadInt(stream);
                this.val = new byte[tmpArrSize];
                for (int i = 0; i < tmpArrSize; ++i)
                {
                    this.val[i] = ProfilerLogUtil.ReadUInt8Value(stream);
                }
                ProfilerLogUtil.AlignSkip(stream, tmpArrSize, 4);
                ConvertObject();
            }

            private void ConvertObject()
            {
                switch(this.type)
                {
                    case (int)RawDataDefines.MetadataDescriptionType.kInstanceId:
                        {
                            var instanceId = ProfilerLogUtil.GetIntValue(this.val, 0);
                            this.convertedObject = new InstanceId { id = instanceId };
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kInt32:
                        {
                            this.convertedObject = ProfilerLogUtil.GetIntValue(this.val, 0);
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kInt64:
                        {
                            this.convertedObject = ProfilerLogUtil.GetLongValue(this.val, 0);
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kString:
                        {
                            this.convertedObject = ProfilerLogUtil.GetString(this.val, 0);
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kString16:
                        {
                            this.convertedObject = ProfilerLogUtil.GetString16(this.val, 0);
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kUInt32:
                        {
                            this.convertedObject = ProfilerLogUtil.GetUIntValue(this.val, 0);
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kUInt64:
                        {
                            this.convertedObject = ProfilerLogUtil.GetULongValue(this.val, 0);
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kVec3:
                        {
                            var readX = ProfilerLogUtil.GetFloat(this.val, 0);
                            var readY = ProfilerLogUtil.GetFloat(this.val, 0);
                            var readZ = ProfilerLogUtil.GetFloat(this.val, 0);
                            this.convertedObject = new Vector3 { x = readX, y = readY, z = readZ };
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kFloat:
                        {
                            this.convertedObject = ProfilerLogUtil.GetFloat(this.val, 0);
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kDouble:
                        {
                            this.convertedObject = ProfilerLogUtil.GetDouble(this.val, 0);
                        }
                        break;
                    case (int)RawDataDefines.MetadataDescriptionType.kBlob8:
                        this.convertedObject = this.val;
                        break;

                }
            }
        }

        public void Read(System.IO.Stream stream)
        {
            this.relatedSampleIndex = ProfilerLogUtil.ReadUint(stream);
            int metadataValueSize = ProfilerLogUtil.ReadInt(stream);
            this.metadatas = new List<MetaDataValue>(metadataValueSize);
            for (int i = 0; i < metadataValueSize; ++i)
            {
                MetaDataValue metaData = new MetaDataValue();
                metaData.Read(stream);
                this.metadatas.Add(metaData);
            }

            //            this.offset = ProfilerLogUtil.ReadUint(stream);
        }

    }


}