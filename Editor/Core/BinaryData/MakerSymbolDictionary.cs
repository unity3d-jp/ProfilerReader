
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Stats;
using UTJ.ProfilerReader.BinaryData.Thread;
using UTJ.ProfilerReader.RawData.Protocol;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        public partial class MakerSymbolDictionary
        {
            private Dictionary<uint  , MakerInfo> makerDictionary = new Dictionary<uint , MakerInfo>();
            public class MakerInfo
            {
                public uint id;
                public string name;
                public uint groupFlag;
                public List<MetadataDescription> descriptions;

                public MakerInfo(uint makerId, string makerName, uint makerGroupFlag, int paramCount)
                {
                    id = makerId;
                    name = makerName;
                    groupFlag = makerGroupFlag;
                    if (paramCount > 0)
                    {
                        descriptions = new List<MetadataDescription>(paramCount);
                    }
                    else
                    {
                        descriptions = null;
                    }
                }

                public void AddDesctiption(MetadataDescription desc)
                {
                    this.descriptions.Add(desc);
                }
            }

            public void AddMakerInfo(MakerInfo maker)
            {
                if (maker == null)
                {
                    return;
                }
                makerDictionary.Add(maker.id, maker);
            }

            public void Clear()
            {
                makerDictionary.Clear();
            }

            public void ResolveThread(List<ThreadData> threads)
            {
                if (threads == null) { return; }
                foreach (ThreadData t in threads)
                {
                    ResolveSamples(t.m_AllSamples);
                }
            }

            public void ResolveSamples(List<ProfilerSample> samples)
            {
                if(makerDictionary == null) { return; }
                if(samples == null) { return; }
                foreach (ProfilerSample sample in samples)
                {
                    MakerInfo info;
                    if(makerDictionary.TryGetValue(sample.makerId,out info) ){
                        sample.profilerInfo = ConvertMakerInfo(info);
                    }
                }
            }

            private ProfilerSample.ProfilerInformation ConvertMakerInfo(MakerInfo info)
            {
                var converted = new ProfilerSample.ProfilerInformation();

                converted.name = info.name;
                converted.group = (ushort)((info.groupFlag >> 16) & 0xFFFF);
                converted.flags = (ushort)(info.groupFlag & 0xFFFF);
                return converted;
            }
        }
    }
}