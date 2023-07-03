using System.Collections.Generic;
using System.IO;
using UTJ.ProfilerReader.RawData.Debug;
using UTJ.ProfilerReader.RawData.Protocol;

namespace UTJ.ProfilerReader.RawData
{
    public class RawDataReader{
        delegate bool ReadMessageFunction(RawBinaryReader reader, ref SessionHeader sessionHeader,ulong threadId);

        private RawDataBehaviour rawDataBehaviour;

        private FileStream fs = null;
        private SessionHeader sessionHeader = new SessionHeader();

        private Dictionary<ulong,SampleWithMetadata> readingSampleWithMetadataByThread = new Dictionary<ulong, SampleWithMetadata>();
        
        byte[] buffer = new byte[64];
        private static byte[] staticBuffer;
        bool isExit = false;

        public Frame LastReadFrame { get; private set; }

        public bool IsComplete
        {
            get { return isExit; }
        }

        public string UnityVersion
        {
            get;set;
        }

        public float Progress
        {
            get
            {
                if( fs == null)
                {
                    return 1.0f;
                }
                return fs.Position / (float)fs.Length;
            }
        }

        public static bool IsRawDataFile(string path)
        {
            SessionHeader header = new SessionHeader();
            if(staticBuffer == null)
            {
                staticBuffer = new byte[64];
            }
            using (FileStream stream = File.OpenRead(path))
            {

                // 最初に 36Byte読み込む
                stream.Read(staticBuffer, 0, 36);
                header.ReadData(staticBuffer);
                stream.Close();
            }

            return header.CheckSignature();
        }

        public void StartReading(string file , RawDataBehaviour behaviour)
        {
            fs = File.OpenRead(file);
            // 最初に 36Byte読み込む
            fs.Read(buffer, 0, 36);
            sessionHeader.ReadData(buffer);
            if(sessionHeader.version >= UTJ.ProfilerReader.BinaryData.ProfilerDataStreamVersion.Unity2022_2)
            {
                fs.Read(buffer, 0, 20);
                sessionHeader.AddUnityVersionData(buffer);
            }

            isExit = false;
            // Set Frame Info
            var frame = new Frame() { index = Frame.kInvalidFrame, time = 0 };
            LastReadFrame = frame;

            rawDataBehaviour = behaviour;
            rawDataBehaviour.OnSessionStart(ref sessionHeader);

            // set reading invalid
            this.readingSampleWithMetadataByThread.Clear();
        }

        private byte[] messageDataBuffer = new byte[128*1024];
        private byte[] AllocMessageDataBuffer( int length)
        {
            if(messageDataBuffer.Length < length)
            {
                messageDataBuffer = new byte[length];
            }
            return messageDataBuffer;
        }

        private Dictionary<ulong, List<byte>> cacheDataByThreadId = new Dictionary<ulong, List<byte>>();
        private List<ulong> cachedDataThreadIds = new List<ulong>();

        private List<ulong> nextFrameCachedThreadIds = new List<ulong>();
        private Dictionary<ulong, List<byte>> nextFrameChacheDataByThreadId = new Dictionary<ulong, List<byte>>();


        public void ReadBlock(System.Action<ulong, byte[], int> act)
        {
            if (isExit)
            {
                return;
            }

            bool isLittleEndian = true;


            BlockHeader blockHeader = new BlockHeader();
            BlockFooter blockFooter = new BlockFooter();


            // block header
            long blockStartPos = fs.Position;
            ReadExpectedSize(fs, buffer, 20);

            bool headerCheck = blockHeader.ReadData(buffer, isLittleEndian);
            if (!headerCheck)
            {
                isExit = true;
                throw new LogReadException("wrong BlockHeader");
            }
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[ReadBlock] BlockHeader:" + blockHeader.threadId + "__" + blockHeader.blockId + "__" + blockHeader.length );
#endif

            // message body
            byte[] messageData = AllocMessageDataBuffer((int)blockHeader.length);
            ReadExpectedSize(fs, messageData, (int)blockHeader.length);

            // add message body to cache
            if ( act != null)
            {
                act(blockHeader.threadId, messageData, (int)blockHeader.length);
            }

            // block footer
            ReadExpectedSize(fs, buffer, 8 );

            bool footerCheck = blockFooter.ReadData(buffer, isLittleEndian);
            if (!footerCheck)
            {
                isExit = true;
                throw new LogReadException("wrong BlockFooter");
            }
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[ReadBlock] BlockFooter:" + blockFooter.nextBlockId );
#endif

            isExit = (fs.Position >= fs.Length);
            if (isExit)
            {
                DebugLogWrite.Flush();
                fs.Close();
                fs = null;
            }
        }
        public void AddBlockDataToCurrentFrameCache(ulong threadId, byte[] data, int length)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[ReadBlock] AddBlockToCurrent:" + threadId);
#endif
            AddCacheData(threadId, data, length, false);
        }

        public void AddBlockDataToNextFrameCache(ulong threadId, byte[] data, int length)
        {
#if UTJ_CHECK
            Debug.DebugLogWrite.Log("[ReadBlock] AddBlockDataToNextFrameCache:" + threadId);
#endif
            AddCacheData(threadId, data, length, true);
        }

        private void ReadExpectedSize(Stream stream , byte[] buffer, int size)
        {
            int readsize = stream.Read(buffer, 0, size);
            if( readsize != size)
            {
                throw new LogReadException("Unexpected file read at " + stream.Position + ":" + size + " " +  readsize);
            }
        }



        private void AddCacheData(ulong threadId , byte[] data, int length,bool nextFrame)
        {

            List<byte> cacheData;
            Dictionary<ulong, List<byte>> dataByThreadId;
            List<ulong> threadIds;
            if (!nextFrame)
            {
                dataByThreadId = cacheDataByThreadId;
                threadIds = cachedDataThreadIds;
            }
            else
            {
                dataByThreadId = nextFrameChacheDataByThreadId;
                threadIds = nextFrameCachedThreadIds;
            }

            if(!dataByThreadId.TryGetValue( threadId , out cacheData))
            {
                cacheData = new List<byte>(128 * 1024);
                dataByThreadId.Add(threadId, cacheData);
            }
            CopyData(data, length, cacheData);
            if (!threadIds.Contains(threadId))
            {
                threadIds.Add(threadId);
            }
        }

        private void CopyData(byte[] src , int length , List<byte> dest)
        {
            for (int i = 0; i < length; ++i)
            {
                dest.Add(src[i]);
            }
        }


        public void ExecuteData( ulong threadId , byte[] data , int length)
        {
            RawBinaryReader messageStream = new RawBinaryReader(this.UnityVersion,data, length,(sessionHeader.isAlignedMemoryAccess != 0));
            rawDataBehaviour.OnBlockStart(threadId);
            ExecuteMessageBlock(threadId, messageStream);
        }






        private void ExecuteMessageBlock(ulong threadId,  RawBinaryReader messageStream)
        {
            try
            {
                if (BlockHeader.IsGlobalThread(threadId))
                {
                    ExecGlobalTheadData(messageStream, ref sessionHeader, threadId);
                }
                else
                {
                    ExecNormalThreadData(messageStream, ref sessionHeader, threadId);
                }
            }
            catch (System.Exception e)
            {
                ProfilerLogUtil.LogError(e);
            }
        }

        bool ExecuteThreadData(ReadMessageFunction func, RawBinaryReader reader, ref SessionHeader sessionHeader, ulong threadId)
        {
            bool result = true;
            while (reader.CanTransfer())
            {
                try
                {
                    result &= func(reader, ref sessionHeader, threadId);
                }
                catch (System.Exception e)
                {
                    ProfilerLogUtil.LogError(e);
                    break;
                }
                if (!result) { break; }
            }
            return result;
        }

        bool ExecGlobalTheadData(RawBinaryReader reader, ref SessionHeader sessionHeader, ulong threadId)
        {
            return ExecuteThreadData(ExecGlobalThreadDataMessage, reader, ref sessionHeader, threadId);
        }

        bool ExecGlobalThreadDataMessage(RawBinaryReader reader, ref SessionHeader sessionHeader, ulong threadId)
        {
            ushort val = reader.ReadUshort();
            RawDataDefines.MessageType messageType = (RawDataDefines.MessageType)val;
            reader.Align();
            switch (messageType)
            {
                case RawDataDefines.MessageType.kProfilerState:
                    {
                        ProfilerState profilerState = new ProfilerState();
                        profilerState.Read(reader);
                        rawDataBehaviour.OnGlobalDataRead(ref profilerState);
                    }
                    break;
                case RawDataDefines.MessageType.kThreadInfo:
                    {
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.Read(reader , sessionHeader.version);
                        rawDataBehaviour.OnGlobalDataRead(ref threadInfo);
                    }
                    break;
                case RawDataDefines.MessageType.kSamplerInfo:
                    {
                        SamplerInfo sampleInfo = new SamplerInfo();
                        sampleInfo.Read(reader);
                        rawDataBehaviour.OnGlobalDataRead(ref sampleInfo);
                    }
                    break;
                case RawDataDefines.MessageType.kMethodJitInfo:
                    {
                        MethodJitInfo methodJitInfo = new MethodJitInfo();
                        methodJitInfo.Read(reader);
                        rawDataBehaviour.OnDataRead(ref methodJitInfo);
                    }
                    break;
                case RawDataDefines.MessageType.kCategoryInfo:
                    {
                        CategoryInfo categoryInfo = new CategoryInfo();
                        categoryInfo.Read(reader);
                        rawDataBehaviour.OnDataRead(ref categoryInfo);
                    }
                    break;
                case RawDataDefines.MessageType.kCategoryState:
                    {
                        CategoryState categoryState = new CategoryState();
                        categoryState.Read(reader);
                        rawDataBehaviour.OnDataRead(ref categoryState);
                    }
                    break;
                case RawDataDefines.MessageType.kUnityNativeTypeInfo:
                    {
                        UnityNativeTypeInfo nativeTypeInfo = new UnityNativeTypeInfo();
                        nativeTypeInfo.Read(reader);
                        rawDataBehaviour.OnDataRead(ref nativeTypeInfo);

                    }
                    break;
                case RawDataDefines.MessageType.kUnityObjectInfo:
                    {
                        UnityObjectInfo objectInfo = new UnityObjectInfo();
                        objectInfo.Read(reader, false);
                        rawDataBehaviour.OnDataRead(ref objectInfo);
                    }
                    break;
                case RawDataDefines.MessageType.kGfxResourceInfo:
                    {
                        GfxResourceInfo gfxResource = new GfxResourceInfo();
                        gfxResource.Read(reader);
                        rawDataBehaviour.OnDataRead(ref gfxResource);
                    }
                    break;

                default:
                    ProfilerLogUtil.LogError("UnknownMessage:" + messageType);
                    return false;
            }
            return true;
        }


        bool ExecNormalThreadData(RawBinaryReader reader, ref SessionHeader sessionHeader, ulong threadId)
        {
            return ExecuteThreadData(ExecNormalThreadDataMessage, reader, ref sessionHeader, threadId);
        }


        private RawDataDefines.MessageType prevMessageType;

        bool ExecNormalThreadDataMessage(RawBinaryReader reader, ref SessionHeader sessionHeader, ulong threadId)
        {
            bool isEnd = this.ExecuteReadingSampleWithMetadata(reader, threadId);
            if( isEnd) { return true; }

            ushort value = reader.ReadUshort();
            reader.Align();
            RawDataDefines.MessageType messageType = (RawDataDefines.MessageType)value;
#if UTJ_CHECK
            DebugLogWrite.Log("[MessageDbg]ExecNormalThreadDataMessage " + messageType + " thread:"+ threadId);
#endif

            switch (messageType)
            {
                case RawDataDefines.MessageType.kFrame:
                    {
                        Frame frame = new Frame();
                        frame.Read(reader);
                        this.LastReadFrame = frame;
                        rawDataBehaviour.OnDataRead(ref frame);                        
                    }
                    break;
                case RawDataDefines.MessageType.kBeginSample:
                case RawDataDefines.MessageType.kSample:
                    {
                        Sample sample = new Sample();
                        sample.Read(reader);
                        rawDataBehaviour.OnDataRead(ref sample,false);

                    }
                    break;
                case RawDataDefines.MessageType.kBeginSampleWithInstanceID:
                case RawDataDefines.MessageType.kSampleWithInstanceID:
                    {
                        SampleWithInstanceID sampleWithInstanceID = new SampleWithInstanceID();
                        sampleWithInstanceID.Read(reader);
                        rawDataBehaviour.OnDataRead(ref sampleWithInstanceID);
                    }
                    break;
                case RawDataDefines.MessageType.kFlowEvent:
                    {
                        FlowEvent flowEvent = new FlowEvent();
                        flowEvent.Read(reader);
                        rawDataBehaviour.OnDataRead(ref flowEvent);
                    }
                    break;

                case RawDataDefines.MessageType.kBeginSampleWithMetadata:
                case RawDataDefines.MessageType.kSampleWithMetadata:
                    {
                        SampleWithMetadata sampleWithMetadata = new SampleWithMetadata();
                        bool result = sampleWithMetadata.Read(reader);
                        if (result)
                        {
                            rawDataBehaviour.OnDataRead(ref sampleWithMetadata);
                        }
                        else
                        {
                            this.readingSampleWithMetadataByThread.Add(threadId, sampleWithMetadata);
                        }
                    }
                    break;
                case RawDataDefines.MessageType.kEndSample:
                    {
                        Sample sample = new Sample();
                        sample.Read(reader);
                        rawDataBehaviour.OnDataRead(ref sample,true);
                    }
                    break;
                case RawDataDefines.MessageType.kGCAlloc:
                    {
                        GCAlloc gCAlloc = new GCAlloc();
                        gCAlloc.Read(reader);
                        rawDataBehaviour.OnDataRead(ref gCAlloc);
                    }
                    break;
                case RawDataDefines.MessageType.kCallstack:
                    {
                        Callstack callstack = new Callstack();
                        callstack.Read(reader);
                        callstack.ReadFrames(reader, callstack.framesCount);
                        rawDataBehaviour.OnDataRead(ref callstack);
                    }
                    break;
                case RawDataDefines.MessageType.kLocalAsyncMetadataAnchor:
                    {
                        AsyncMetadataAnchor asyncMetadataAnchor = new AsyncMetadataAnchor();
                        asyncMetadataAnchor.Read(reader);
                        rawDataBehaviour.OnDataRead(ref asyncMetadataAnchor);
                    }
                    break;

                case RawDataDefines.MessageType.kLocalGPUSample:
                    {
                        GPUSample gpuSample = new GPUSample();
                        gpuSample.Read(reader);
                        rawDataBehaviour.OnDataRead(ref gpuSample);
                    }
                    break;

                case RawDataDefines.MessageType.kCleanupThread:
                    {
                        CleanupThread cleanupThread = new CleanupThread();
                        cleanupThread.Read(reader);
                        rawDataBehaviour.OnDataRead(ref cleanupThread);
                    }
                    break;

                case RawDataDefines.MessageType.kThreadInfo:
                    {
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.Read(reader,sessionHeader.version);
                        rawDataBehaviour.OnDataRead(ref threadInfo);
                    }
                    break;

                case RawDataDefines.MessageType.kUIEvents:
                    {
                        UIEvents uiEvents = new UIEvents();
                        uiEvents.Read(reader);
                        rawDataBehaviour.OnDataRead(ref uiEvents);
                    }
                    break;

                case RawDataDefines.MessageType.kUISystemCanvas:
                    {
                        UISystemCanvas uiSystemCanvas = new UISystemCanvas();
                        uiSystemCanvas.Read(reader);
                        rawDataBehaviour.OnDataRead(ref uiSystemCanvas);
                    }
                    break;

                case RawDataDefines.MessageType.kAudioInstanceData:
                    {
                        AudioInstanceData audioInstanceData = new AudioInstanceData();
                        audioInstanceData.Read(reader);
                        rawDataBehaviour.OnDataRead(ref audioInstanceData);
                    }
                    break;

                case RawDataDefines.MessageType.kAllProfilerStats:
                    {
                        AllProfilerStats allProfilerStats = new AllProfilerStats();
                        allProfilerStats.Read(reader, sessionHeader.version);
                        rawDataBehaviour.OnDataRead(ref allProfilerStats);
                    }
                    break;


                default:
                    ProfilerLogUtil.LogError("UnknownMessage:" + messageType +
                        " preview:" + prevMessageType + " position:" + reader.Position + "/" + reader.Length +
                        " thread:" + threadId);
                    return false;
            }
            prevMessageType = messageType;
            return true;
        }

        private bool ExecuteReadingSampleWithMetadata(RawBinaryReader reader, ulong threadId)
        {
            SampleWithMetadata readingSampleWithMetadata;
            // continue read from prev metadata info
            if (readingSampleWithMetadataByThread.TryGetValue(threadId, out readingSampleWithMetadata))
            {
#if UTJ_CHECK
                Debug.DebugLogWrite.Log("[ExecuteReadingSampleWithMetadata]" + threadId + ";;"+ readingSampleWithMetadata.metadataCount);

#endif
                bool readingEnd = readingSampleWithMetadata.ReadMore(reader);
                if (!readingEnd)
                {
                    readingSampleWithMetadataByThread[threadId] = readingSampleWithMetadata;
                    return true;
                }
                rawDataBehaviour.OnDataRead(ref readingSampleWithMetadata);
                readingSampleWithMetadataByThread.Remove(threadId);
                if (!reader.CanTransfer())
                {
                    return true;
                }
            }
            return false;
        }

    }
}