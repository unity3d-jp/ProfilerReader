using System.Collections;
using System.Collections.Generic;

using System;
using System.Reflection;

namespace UTJ.ProfilerReader.BinaryData.Stats
{
    public class StatsDeserializeManager<T> where T : class
    {
        Dictionary<uint, StatsDeserializer<T>> deserializers;
        static StatsDeserializeManager<T> s_instance;

        public static StatsDeserializeManager<T> Instance
        {
            get
            {
                if(s_instance == null) {
                    s_instance = new StatsDeserializeManager<T>();
                }
                return s_instance;
            }
        }
        public void Clear()
        {
            deserializers.Clear();
        }
        public StatsDeserializer<T> GetDeserializer(uint version)
        {
            if (deserializers == null) {
                deserializers = new Dictionary<uint, StatsDeserializer<T>>();
            }
            StatsDeserializer<T> val;
            if( deserializers.TryGetValue(version, out val))
            {
                return val;
            }
            val = new StatsDeserializer<T>(version);
            deserializers.Add(version, val);

            return val;
        }
    }

    public class StatsDeserializer <T> where T :class
    {
        private struct ReadInfo
        {
            public enum ReadType
            {
                Unknown,
                TypeInt,
                TypeUint,
                TypeBool,
                TypeFloat,
                TypeFixedIntArray,
            }
            public FieldInfo field;
            public int sortParam;
            public ReadType readType;
            public int arraySize;


            public void SetTypeInfo(object obj , FieldInfo field)
            {
                var type = field.FieldType;
                if (type == typeof(System.Int32))
                {
                    readType = ReadType.TypeInt;
                    arraySize = 0;
                }
                else if (type == typeof(System.UInt32))
                {
                    readType = ReadType.TypeUint;
                    arraySize = 0;
                }
                else if (type == typeof(System.Boolean))
                {
                    readType = ReadType.TypeBool;
                    arraySize = 0;
                }
                else if (type == typeof(System.Single))
                {
                    readType = ReadType.TypeFloat;
                    arraySize = 0;
                }
                else if (type == typeof(System.Int32[]))
                {
                    readType = ReadType.TypeFixedIntArray;
                    var val = field.GetValue(obj);
                    int[] array = val as int[];
                    if (array != null)
                    {
                        arraySize = array.Length;
                    }
                }
                else
                {
                    readType = ReadType.Unknown;
                }
            }

        }

        private List<ReadInfo> m_readFieldsInfo;
        private uint m_version;

        public StatsDeserializer(uint version)
        {
            m_version = version;
            m_readFieldsInfo = new List<ReadInfo>(32);
            Type t = typeof(T);
            // Instance object to get fixed array size 
            T obj = Activator.CreateInstance<T>();

            var fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if(fields == null) { return; }
            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(StatsDataAttribute), false);
                if( attributes == null || attributes.Length == 0) { continue; }
                StatsDataAttribute dataAttr = attributes[0] as StatsDataAttribute;
                if( dataAttr == null) { continue; }
                if( dataAttr.MinSupportVersion > version || version > dataAttr.MaxSupportVersion)
                {
                    continue;
                }
                ReadInfo info = new ReadInfo
                {
                    sortParam = dataAttr.SortParam,
                    field = field,
                };
                info.SetTypeInfo( obj, field );
                m_readFieldsInfo.Add(info);
            }

            m_readFieldsInfo.Sort(
                (a, b) => {
                    return (a.sortParam - b.sortParam);
                });
        }

        public T ReadObject(IStatsStream statsStream)
        {
            Type t = typeof(T);
            var fields = t.GetFields(System.Reflection.BindingFlags.Public);

            T obj = Activator.CreateInstance<T>();
            foreach( var fInfo in m_readFieldsInfo)
            {

                WriteField(statsStream,obj, fInfo);
            }
            return obj;
        }
        public void DebugPrint()
        {
            foreach (var fInfo in m_readFieldsInfo)
            {
                ProfilerLogUtil.Log(fInfo.field.Name + ":" + fInfo.readType);
                // UnityEngine.Debug.Log(fInfo.field.Name + ":" + fInfo.readType);
            }
        }

        private void WriteField(IStatsStream statsStream,T obj ,  ReadInfo info)
        {
            switch(info.readType){
                case ReadInfo.ReadType.TypeInt:
                    {
                        info.field.SetValue(obj, statsStream.ReadInt());
                    }
                    break;
                case ReadInfo.ReadType.TypeUint:
                    {
                        info.field.SetValue(obj, statsStream.ReadUint());
                    }
                    break;
                case ReadInfo.ReadType.TypeBool:
                    {
                        info.field.SetValue(obj, (statsStream.ReadInt() != 0) );
                    }
                    break;
                case ReadInfo.ReadType.TypeFloat:
                    {
                        info.field.SetValue(obj, statsStream.ReadFloat());
                    }
                    break;
                case ReadInfo.ReadType.TypeFixedIntArray:
                    {
                        int[] val = info.field.GetValue(obj) as int[];
                        if( val == null) { return; }
                        for (int i = 0; i < info.arraySize; ++i)
                        {
                            val[i] = statsStream.ReadInt();
                        }
                    }
                    break;
            }
        }
    }
}