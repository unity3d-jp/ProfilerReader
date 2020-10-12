using System.Collections;
using System.Collections.Generic;

using UTJ.ProfilerReader.RawData;
using UTJ.ProfilerReader.RawData.Protocol;

using UTJ.ProfilerReader.BinaryData;

namespace UTJ.ProfilerReader.RawData.Converter
{

    public class SampleStackResolver
    {
        struct FrameThreadPair
        {
            private static readonly IEqualityComparer<FrameThreadPair> resolverComparer;
            public ulong threadId;
            public uint frameId;
            

            private sealed class BreedLocalIdEqualityComparer : IEqualityComparer<FrameThreadPair>
            {
                public bool Equals(FrameThreadPair a, FrameThreadPair b)
                {
                    return ((a.threadId == b.threadId) && (a.frameId == b.frameId));
                }
                public int GetHashCode(FrameThreadPair obj)
                {
                    return obj.threadId.GetHashCode() + obj.frameId.GetHashCode();
                }
            }
        }


        // key-thread/frame , value sampleIdStack
        private Dictionary<FrameThreadPair, Stack<SampleWrappedData> > dictionary;
        private FrameThreadPair currentFrameThread = new FrameThreadPair();

        List<FrameThreadPair> delKeys = new List<FrameThreadPair>(64);

        private Stack<SampleWrappedData> currentTargetStack;
        public Stack<SampleWrappedData> currentStack
        {
            get
            {
                if (!this.isDirty)
                {
                    return currentTargetStack;
                }
                if( !dictionary.TryGetValue(currentFrameThread, out currentTargetStack))
                {
                    currentTargetStack = new Stack<SampleWrappedData>(64);
                    dictionary.Add(currentFrameThread, currentTargetStack);
                }
                this.isDirty = false;
                return currentTargetStack;
            }
        }

        private bool isDirty = true;

        public SampleStackResolver()
        {
            this.dictionary = new Dictionary<FrameThreadPair, Stack<SampleWrappedData>>();
        }

        public void SetFrame( uint frameIdx)
        {
            this.currentFrameThread.frameId = frameIdx;
            this.isDirty = true;
        }
        public void SetThread( ulong threadId)
        {
            this.currentFrameThread.threadId = threadId;
            this.isDirty = true;
        }
        public void BeginSample(ref SampleWrappedData sample)
        {
            this.currentStack.Push(sample);
        }
        public SampleWrappedData EndSample()
        {
            if( this.currentStack.Count == 0) {
                SampleWrappedData data = new SampleWrappedData();
                data.sampleType = SampleWrappedData.EType.None;
                return data;
            }
            SampleWrappedData val = this.currentStack.Pop();
            return val;
        }

        public SampleWrappedData EndSampleWithSampleId(uint sampleId)
        {
            SampleWrappedData sample = new SampleWrappedData();

            while (true)
            {
                sample = EndSample();
                if (sampleId == sample.sample.id)
                {
                    return sample;
                }
                if (this.currentStack.Count == 0)
                {
                    SampleWrappedData data = new SampleWrappedData();
                    data.sampleType = SampleWrappedData.EType.None;
                    return data;
                }
            }
            return sample;
        }

        public void Check()
        {
            foreach( var kvs in dictionary)
            {
                if( kvs.Value.Count > 0)
                {
                    ProfilerLogUtil.LogError("Something is left " + kvs.Value.Count + " *** " + kvs.Key.frameId + "-" + kvs.Key.threadId);
                }
            }
        }

        public bool HasSample(uint sampleId)
        {
            foreach( var data in currentStack)
            {
                if( data.sample.id == sampleId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasParent()
        {
            return (this.currentStack.Count > 0);
        }

        public int GetStackNum()
        {
            return this.currentStack.Count;
        }


        public void ReleaseFrameData( uint frameId)
        {
            delKeys.Clear();
            var keys = this.dictionary.Keys;
            foreach( var key in keys)
            {
                if( key.frameId == frameId)
                {
                    delKeys.Add(key);
                }
            }

            // remove del keys
            foreach( var delKey in delKeys)
            {
                this.dictionary.Remove(delKey);
            }
        }
    }
}