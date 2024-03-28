
using System.Text;
using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData.Thread;
using UTJ.ProfilerReader.RawData.Protocol;

namespace UTJ.ProfilerReader
{
    namespace BinaryData
    {
        // Base profiler sample with CPU times;
        // minimal amount of actual per-sample data.
        public partial class ProfilerSample
        {
            // maybe spend time to execute.
            public ulong startTimeUS { get; set; } // start time in microsecs
            public float timeUS { get; set; } // duration in microsecs (元はfloat)
            public int nbChildren { get; set; }
            public ProfilerInformation profilerInfo { get; set; }
            public MetaData metaDatas { get; set; }

            // from 2022_2
            public uint makerId { get; set; }

            public List<MetaData.MetaDataValue> metadataValues
            {
                get
                {
                    if(metaDatas == null) { return null; }
                    return metaDatas.metadatas;
                }
            }


            // parent( call from)
            public ProfilerSample parent { get; set; }
            // children(call to)
            public List<ProfilerSample> children { get; set; }
            // hierarchyLevel( call stack)
            public int hierarchyLevel { get; set; }
            // gc alloc
            public List<AllocatedGCMemory> gcAllocList { get; set; }
            public uint childGcAlloc { get; private set; }

            // call stackinfo
            public ProfileCallstack callStackInfo { get; set; }

            // name Info
            public string sampleName
            {
                get
                {
                    if (this.profilerInfo == null || this.profilerInfo.name == null) { return ""; }
                    return this.profilerInfo.name;
                }
            }
            public int group
            {
                get
                {
                    if (this.profilerInfo == null) { return -1; }
                    return profilerInfo.group;
                }
            }

            private string fullSampleNameStr = null;
            private static StringBuilder stringBuilderBuffer = new StringBuilder(1024);
            public string fullSampleName
            {
                get
                {
                    if (fullSampleNameStr == null)
                    {
                        if (this.parent == null)
                        {
                            fullSampleNameStr = this.sampleName;
                        }
                        else
                        {
                            var parentString = this.parent.fullSampleName;
                            lock (stringBuilderBuffer)
                            {
                                stringBuilderBuffer.Length = 0;
                                stringBuilderBuffer.Append(parentString).
                                    Append(" -> ").Append(this.sampleName);
                                fullSampleNameStr = stringBuilderBuffer.ToString();
                            }
                        }
                    }

                    return fullSampleNameStr;
                }
            }
            public float selfTimeUs
            {
                get
                {
                    if (children == null) { return this.timeUS; }
                    float tm = this.timeUS;
                    foreach (var child in children)
                    {
                        tm -= child.timeUS;
                    }
                    if (tm < 0) { tm = 0; }
                    return tm;
                }
            }



            // gc calculate cache
            private uint calcedCurrentGc = 0;

            public uint currenGcAlloc
            {
                get
                {
                    return this.calcedCurrentGc;
                }
            }
            public uint totalGcAlloc
            {
                get { return currenGcAlloc + childGcAlloc; }
            }

            public uint GetSelfChildGcAlloc()
            {
                uint gc = 0;
                var childs = this.children;
                var childCount = childs.Count;
                for(int i=0; i < childCount; ++i)
                {
                    gc += childs[i].calcedCurrentGc;
                }
                return gc;
            }


            public class ProfilerInformation
            {
                public string name { get; set; }
                public ushort group { get; set; }
                public ushort flags { get; set; }
                public byte isWarning { get; set; }

                // CounterSample?
                public bool IsCounter
                {
                    get
                    {
                        return ((flags & 128) == 128);
                    }
                }

                public void Read(System.IO.Stream stream)
                {
                    this.name = ProfilerLogUtil.ReadString(stream);
                    uint data = ProfilerLogUtil.ReadUint(stream);
                    this.group = (ushort)((data >> 16) & 0xffff);
                    this.flags = (ushort)((data >> 0) & 0xffff);
                    this.isWarning = (byte)(data  & 0xff);
                }
            }


            // key:BlockSize , value count
            public Dictionary<uint, int> GetBlockAllocCount()
            {
                Dictionary<uint, int> allocCnt = new Dictionary<uint, int>();

                CollectChildrenAllocBlock(allocCnt, this);
                return allocCnt;
            }

            private static void CollectChildrenAllocBlock(Dictionary<uint, int> allocCnt, ProfilerSample sample)
            {
                if (sample.gcAllocList != null)
                {
                    foreach (var alloc in sample.gcAllocList)
                    {
                        int cnt = 0;
                        if( allocCnt.TryGetValue(alloc.allocatedGCMemory,out cnt)){
                            allocCnt[alloc.allocatedGCMemory] = cnt + 1;
                        }else{
                            allocCnt.Add(alloc.allocatedGCMemory, 1);
                        }
                    }
                }

                if (sample.children == null) { return; }
                foreach (var child in sample.children)
                {
                    CollectChildrenAllocBlock(allocCnt, child);
                }
            }

            #region RESOLVE_AFTER_COLLECT_SAMPLES

            public static void ResolveCallGraph(List<ProfilerSample> list)
            {
                if (list == null || list.Count == 0) { return; }

                try
                {
                    int idx = 0;
                    while (idx < list.Count)
                    {
                        idx = ResolveCallGraphRecursive(list, idx);
                    }
                }
                catch (System.Exception e)
                {
                    ProfilerLogUtil.LogError(e);
                }
            }

            private static int ResolveCallGraphRecursive(List<ProfilerSample> list, int idx) {
                if( list.Count <= idx) { return list.Count; }
                ProfilerSample current = list[idx];
                ++idx;
                int childNum = current.nbChildren;
                if (childNum == 0)
                {
                    current.children = null;
                    return idx;
                }

                current.children = new List<ProfilerSample>(childNum);
                for (int i = 0; i < childNum; ++i)
                {
                    current.children.Add( list[idx] );
                    list[idx].parent = current;
                    idx = ResolveCallGraphRecursive(list, idx);
                }
                return idx;
            }

            /* old code
            private static void NameAddRecursive(StringBuilder sb, ProfilerSample sample, int call)
            {
                if (sample == null) { return; }
                NameAddRecursive(sb, sample.parent, call + 1);
                sb.Append(sample.sampleName);
                if (call != 0 && !string.IsNullOrEmpty(sample.sampleName) ) { sb.Append(" -> "); }
            }
            */

            public void AddAllocatedGC(AllocatedGCMemory alloc)
            {
                if (gcAllocList == null) { gcAllocList = new List<AllocatedGCMemory>(4); }
                gcAllocList.Add(alloc);
                calcedCurrentGc += alloc.allocatedGCMemory;
            }

            public static void ResolveGcAllocate(List<ProfilerSample> sampleList,List<AllocatedGCMemory> allocList)
            {
                if (allocList == null) { return; }
                if (sampleList == null) { return; }
                foreach (var alloc in allocList)
                {
                    sampleList[(int)alloc.relatedSampleIndex].AddAllocatedGC(alloc);
                }
                foreach (var sample in sampleList)
                {
                    if (sample.nbChildren != 0) { continue; }
                    if (sample.parent == null) { continue; }
                    AddChildAllocToParent(sample);
                }
            }
            public static void ResolveSampleMetadata(List<ProfilerSample> sampleList , List<MetaData> metadatas)
            {
                if(sampleList == null) { return; }
                if(metadatas == null) { return; }

                foreach( var metadata in metadatas)
                {
                    int idx = (int)metadata.relatedSampleIndex;
                    if( idx <0 || idx >= sampleList.Count) { continue; }

                    if( sampleList[idx].metaDatas == null)
                    {
                        sampleList[idx].metaDatas = metadata;
                    }
                    else
                    {
                        foreach (var metaValue in metadata.metadatas)
                        {
                            sampleList[idx].metaDatas.metadatas.Add(metaValue);
                        }
                    }
                }
                    

            }
            public static void ResolveCallstackInfo(List<ProfilerSample> sampleList,List<ProfileCallstack> callstackList)
            {
                if (callstackList == null) { return; }
                if (sampleList == null) { return; }
                int length = sampleList.Count;
                foreach (var callstatck in callstackList)
                {
                    int idx = (int)callstatck.relatedSampleIndex;
                    if (0 <= idx && idx < length)
                    {
                        sampleList[idx].callStackInfo = callstatck;
                    }
                }

            }


            public static void AddChildAllocToParent(ProfilerSample sample)
            {
                uint sampleAllocated = sample.currenGcAlloc;
                for (ProfilerSample current = sample.parent; current != null; current = current.parent)
                {
                    current.childGcAlloc += sampleAllocated;
                    sampleAllocated += current.currenGcAlloc;
                }
            }

            public static void ResolveHierarychyLevel(List<ProfilerSample> sampleList )
            {
                foreach (ProfilerSample sample in sampleList)
                {
                    if (sample == null || sample.parent != null) { continue; }
                    NumberingChildrenHierarychyLevel(sample, 0);
                }
            }

            private static void NumberingChildrenHierarychyLevel(ProfilerSample sample, int level)
            {
                sample.hierarchyLevel = level;
                if (sample.children == null) { return; }
                foreach (var child in sample.children)
                {
                    NumberingChildrenHierarychyLevel(child, level + 1);
                }
            }


            #endregion RESOLVE_AFTER_COLLECT_SAMPLES
        }
    }
}
