using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UTJ.ProfilerReader.RawData.Protocol
{

    class RawBinaryReader 
    {
        private int dataSize = 0;
        private System.IO.MemoryStream memoryStream;
        private bool alignEnabled;

        private byte[] stringBuffer;

        public RawBinaryReader(string unityVersion)
        {
            memoryStream = new System.IO.MemoryStream();
            alignEnabled = false;
            this.UnityVersion = unityVersion;
        }
        public RawBinaryReader(string unityVersion,byte[] data,int length,bool alignFlag)
        {

            memoryStream = new System.IO.MemoryStream(data,0,length);
            dataSize = length;
            alignEnabled = alignFlag;
            this.UnityVersion = unityVersion;
        }
        public void Append(byte[] data, int length)
        {
            memoryStream.Write(data, 0, length);
        }

        public long Length
        {
            get
            {
                return memoryStream.Length;
            }
        }

        public long Position
        {
            get
            {
                return memoryStream.Position;
            }
            set
            {
                memoryStream.Position = value;
            }
        }
        public string UnityVersion
        {
            get;private set;
        }

        public void Align(int val = 4)
        {
            if (!alignEnabled) { return; }
            long originPos = memoryStream.Position;
            long expectPos = ((originPos + (val - 1)) / val) * val;
            int offset = (int)(expectPos - originPos);
            if( offset > 0)
            {
                memoryStream.Position += offset;
            }
        }

        public byte ReadByte()
        {
            byte data = (byte)memoryStream.ReadByte();
            return data;
        }
        public ushort ReadUshort()
        {
            return UTJ.ProfilerReader.ProfilerLogUtil.ReadUShort(memoryStream);
        }

        private static System.Text.Encoding utf8Encoding = System.Text.Encoding.GetEncoding("utf-8");



        public uint ReadUint()
        {
            return UTJ.ProfilerReader.ProfilerLogUtil.ReadUint(memoryStream);
        }


        public ulong ReadUlong()
        {
            return UTJ.ProfilerReader.ProfilerLogUtil.ReadULong(memoryStream);
        }

        public int ReadInt()
        {
            return UTJ.ProfilerReader.ProfilerLogUtil.ReadInt(memoryStream);
        }

        public long ReadLong()
        {
            return UTJ.ProfilerReader.ProfilerLogUtil.ReadLong(memoryStream);
        }
        public float ReadFloat()
        {
            return UTJ.ProfilerReader.ProfilerLogUtil.ReadFloat(memoryStream);
        }

        public double ReadDouble()
        {
            return UTJ.ProfilerReader.ProfilerLogUtil.ReadDouble(memoryStream);
        }

        public string ReadStringAsTransfer()
        {
            int stringSize = (int)this.ReadUint();
            if(stringSize == 0)
            {
                return "";
            }
            if (stringSize > 1024 * 1024*5) {
                throw new System.Exception("error " + stringSize + " " + this.memoryStream.Position);
            }
            byte[] buffer = GetStringBuffer(stringSize);
            memoryStream.Read(buffer, 0, stringSize);
            var encoding = utf8Encoding;
            string str = encoding.GetString(buffer,0,stringSize);
            return str;
        }
        public string ReadAsAsciiString()
        {
            int size = this.ReadInt();
            byte[] buffer = GetStringBuffer(size);
            memoryStream.Read(buffer, 0, size);
            var encoding = System.Text.Encoding.ASCII;
            string str = encoding.GetString(buffer, 0, size);
            return str;
        }

        public string ConvertToUtfString(byte[] buffer)
        {
            var encoding = utf8Encoding;
            string str = encoding.GetString(buffer, 0, buffer.Length);
            return str;
        }
        public string ConvertToAsciiString(byte[] buffer)
        {
            var encoding = System.Text.Encoding.ASCII;
            string str = encoding.GetString(buffer, 0, buffer.Length);
            return str;
        }

        private byte[] GetStringBuffer(int strSize)
        {
            if( this.stringBuffer != null && strSize < this.stringBuffer.Length) {
                return this.stringBuffer;
            }
            int allocSize = strSize;
            if( allocSize < 128) { allocSize = 128; }
            this.stringBuffer = new byte[allocSize];
            return this.stringBuffer;

        }

        public int[] ReadIntArray()
        {
            var startPos = this.Position;
            int size = this.ReadInt();
            int[] data = new int[size];
            try
            {
                for (int i = 0; i < size; ++i)
                {
                    data[i] = this.ReadInt();
                }
            }catch(System.Exception e)
            {
                ProfilerLogUtil.LogError("ReadInt Error " + size + "_" + startPos + "-" + Position + "/" + Length );
                ProfilerLogUtil.LogError(e);
            }
            return data;
        }


        public byte[] ReadByteArray()
        {
            int size = this.ReadInt();
            byte[] buffer = new byte[size];
            int readSize = memoryStream.Read(buffer, 0, size);
            return buffer;
        }

        public int ReadByteToArray( byte[] buffer , int index ,int size)
        {
            int readSize = memoryStream.Read(buffer, index, size);
            return readSize;
        }

        public ulong[] ReadUlongArray()
        {
            int size = this.ReadInt();
            ulong[] val = new ulong[size];
            for(int i = 0; i < size; ++i)
            {
                val[i] = this.ReadUlong();
            }

            return val;
        }

        public bool CanTransfer()
        {
            return (memoryStream.Position < memoryStream.Length);
        }
    }
}