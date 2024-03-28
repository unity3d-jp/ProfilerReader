
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UTJ.ProfilerReader
{
    public class ProfilerLogUtil
    {
        public static System.Action<string> logErrorString
        {
            set; get;
        }
        public static System.Action<System.Exception> logErrorException
        {
            set; get;
        }

        private static byte[] dataBuffer;

        private static byte[] AllocateBuffer(int size)
        {
            if(dataBuffer == null) { dataBuffer = new byte[128]; }
            if( dataBuffer.Length < size)
            {
                dataBuffer = new byte[size];
            }
            return dataBuffer;
        }

        public static int GetIntValue(byte[] bin, int offset)
        {
            return (bin[offset + 0] << 0) +
                (bin[offset + 1] << 8) +
                (bin[offset + 2] << 16) +
                (bin[offset + 3] << 24);
        }
        public static int ReadInt(System.IO.Stream stream)
        {
            byte[] data = AllocateBuffer(4);
            int size = stream.Read(data, 0, 4);
            if (size < 3)
            {
                throw new System.Exception("ReadInt Error");
            }
            return ProfilerLogUtil.GetIntValue(data, 0);
        }


        public static ushort GetUshortValue(byte[] bin, int offset)
        {
            return (ushort)( (bin[offset + 0] << 0) + (bin[offset + 1] << 8) );
        }
        public static ushort ReadUShort( System.IO.Stream stream)
        {
            byte[] data = AllocateBuffer(2);
            int size = stream.Read(data, 0, 2);
            if (size < 1)
            {
                throw new System.Exception("ReadUShort Error " + size + " pos " + stream.Position + "/" + stream.Length);
            }
            return ProfilerLogUtil.GetUshortValue(data, 0);
        }


        public static uint GetUIntValue(byte[] bin, int offset)
        {
            return (uint)(bin[offset + 0] << 0) +
                (uint)(bin[offset + 1] << 8) +
                (uint)(bin[offset + 2] << 16) +
                (uint)(bin[offset + 3] << 24);
        }
        public static uint ReadUint(System.IO.Stream stream)
        {
            byte[] data = AllocateBuffer(4);
            int size = stream.Read(data, 0, 4);
            if (size < 3)
            {
                throw new System.Exception("ReadUint Error " + size + " pos " + stream.Position + "/" + stream.Length);
            }
            return ProfilerLogUtil.GetUIntValue(data, 0);
        }
        public static byte ReadUInt8Value(System.IO.Stream stream)
        {
            return (byte)(stream.ReadByte());
        }

        public static void AlignSkip(System.IO.Stream stream, int readBytes, int aligneByte)
        {

            int left = readBytes % aligneByte;
            if (left == 0)
            {
                return;
            }
            stream.Position += aligneByte - left; 
        }

        // 
        public static float ReadFloat(System.IO.Stream stream)
        {
            byte[] data = AllocateBuffer(4);
            int size = stream.Read(data, 0, 4);
            if (size < 3)
            {
                throw new System.Exception("ReadFloat Error");
            }
            return GetFloat(data,0);
        }
        public static float GetFloat(byte[] data,int idx)
        {
            return System.BitConverter.ToSingle(data, idx);
        }

        public static double ReadDouble(System.IO.Stream stream)
        {
            byte[] data = AllocateBuffer(8);
            int size = stream.Read(data, 0, 8);
            if (size < 7)
            {
                throw new System.Exception("ReadFloat Double");
            }
            return GetDouble(data, 0);
        }
        public static double GetDouble(byte[] data, int idx)
        {
            return System.BitConverter.ToDouble(data, idx);
        }


        public static long GetLongValue(byte[] bin, int offset)
        {
            return ((long)bin[offset + 0] << 0) +
                ((long)bin[offset + 1] << 8) +
                ((long)bin[offset + 2] << 16) +
                ((long)bin[offset + 3] << 24) +
                ((long)bin[offset + 4] << 32) +
                ((long)bin[offset + 5] << 40) +
                ((long)bin[offset + 6] << 48) +
                ((long)bin[offset + 7] << 56);
        }
        public static long ReadLong(System.IO.Stream stream)
        {
            byte[] data = AllocateBuffer(8);
            int size = stream.Read(data, 0, 8);
            if (size < 7)
            {
                throw new System.Exception("ReadLong Error");
            }
            return ProfilerLogUtil.GetLongValue(data, 0);
        }

        public static ulong GetULongValue(byte[] bin, int offset)
        {
            return ((ulong)bin[offset + 0] << 0) +
                ((ulong)bin[offset + 1] << 8) +
                ((ulong)bin[offset + 2] << 16) +
                ((ulong)bin[offset + 3] << 24) +
                ((ulong)bin[offset + 4] << 32) +
                ((ulong)bin[offset + 5] << 40) +
                ((ulong)bin[offset + 6] << 48) +
                ((ulong)bin[offset + 7] << 56);
        }
        public static ulong ReadULong(System.IO.Stream stream)
        {
            byte[] data = AllocateBuffer(8);
            int size = stream.Read(data, 0, 8);
            if (size < 7)
            {
                throw new System.Exception("ReadULong Error");
            }
            return ProfilerLogUtil.GetULongValue(data, 0);
        }

        public static string ReadString(System.IO.Stream stream)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            byte[] buffer = AllocateBuffer(4);
            bool flag = true;
            while(flag)
            {
                stream.Read(buffer, 0, 4);
                for( int i = 0 ; i < 4; ++i){
                    if (buffer[i] == 0) { flag = false; break; }
                    sb.Append((char)buffer[i]);
                }
            } 
            return sb.ToString();
        }

        public static string GetString(byte[] data,int idx)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(data.Length - idx);

            for (int i = idx; i < data.Length; ++i)
            {
                sb.Append((char)data[i]);
            }
            return sb.ToString();
        }

        private static System.Text.Encoding utf8Encoding = System.Text.Encoding.GetEncoding("utf-8");
        public static string GetString16(byte[] data,int idx)
        {
            string str = utf8Encoding.GetString(data, idx, data.Length -idx);
            return str;
        }

        public static List<string> ReadNamesString(System.IO.Stream stream)
        {
            List<string> list = new List<string>();
            int nameSize = ProfilerLogUtil.ReadInt(stream);
            byte[] names = AllocateBuffer(nameSize);
            stream.Read(names, 0, nameSize);
            ProfilerLogUtil.AlignSkip(stream, nameSize, 4);
            System.Text.StringBuilder sb = new System.Text.StringBuilder(nameSize);
            for (int i = 0; i < names.Length; ++i)
            {
                if (names[i] == 0)
                {
                    list.Add( sb.ToString());
                    sb.Length = 0;
                    continue;
                }
                sb.Append( (char)names[i] );
            }
            if (sb.Length > 0)
            {
                list.Add(sb.ToString());
            }
            return list;
        }

        private static bool ContainsZero(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; ++i)
            {
                if (buffer[i] == 0) { return true; }
            }
            return false;
        }



        public static void WriteUintValue(uint val, byte[] bin, int offset)
        {
            bin[offset + 0] = (byte)((val >> 0) & 0xff);
            bin[offset + 1] = (byte)((val >> 8) & 0xff);
            bin[offset + 2] = (byte)((val >> 16) & 0xff);
            bin[offset + 3] = (byte)((val >> 24) & 0xff);
        }

        public static void WriteUshortValue(ushort val, byte[] bin, int offset)
        {
            bin[offset + 0] = (byte)((val >> 0) & 0xff);
            bin[offset + 1] = (byte)((val >> 8) & 0xff);
        }




        public static ILogReaderPerFrameData CreateLogReader(string filePath)
        {
            ILogReaderPerFrameData logReader = null;

            if (UTJ.ProfilerReader.RawData.RawDataReader.IsRawDataFile(filePath))
            {
                logReader = new UTJ.ProfilerReader.RawData.ProfilerRawLogReader();
                logReader.SetFile(filePath);
            }
            else
            {
                logReader = new UTJ.ProfilerReader.BinaryData.ProfilerBinaryLogReader();
                logReader.SetFile(filePath);
            }
            return logReader;
        }

        internal static void Log(string str)
        {
        }


        internal static void LogError(string str)
        {
            if(logErrorString != null)
            {
                logErrorString(str);
            }
//            UnityEngine.Debug.LogError(str);
        }
        internal static void LogError(System.Exception e)
        {
            if(logErrorException != null)
            {
                logErrorException(e);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b)
        {
            if (a < b) { return a; }
            return b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            if (a > b) { return a; }
            return b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Min(uint a, uint b)
        {
            if (a < b) { return a; }
            return b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Max(uint a, uint b)
        {
            if (a > b) { return a; }
            return b;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b)
        {
            if (a < b) { return a; }
            return b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            if (a > b) { return a; }
            return b;
        }
    }
}
