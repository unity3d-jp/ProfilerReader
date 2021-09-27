using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine.Profiling;
using UTJ.ProfilerReader.BinaryData;

using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader.Analyzer
{

    public class EngineFileOperateAnalyzeToFile : AnalyzeToTextbaseFileBase
    {
        struct FileEvent
        {
            public string thread;

            public string eventStr;

            public int frameIdx;
            public string file;
            public ulong size;
            public long seekOffset;
            public int param;

            public ulong startTime;
            public float tm;
        }

        private List<FileEvent> fileEvents = new List<FileEvent>(2048);
        


        public override void CollectData(ProfilerFrameData frameData)
        {
            if( frameData == null )
            {
                return;
            }


            HashSet<ProfilerSample> doneList = new HashSet<ProfilerSample>();
            foreach( var thread in frameData.m_ThreadData)
            {
                if(thread.m_AllSamples == null) { continue; }
                foreach( var sample in thread.m_AllSamples)
                {
                    if (!sample.sampleName.StartsWith("File.")) { continue; }

                    if (sample.sampleName == "File.Open")
                    {
                        AddFileOpenCloseData(frameData.frameIndex, thread, sample);
                    }
                    else if (sample.sampleName == "File.Read")
                    {
                        AddFileReadData(frameData.frameIndex, thread, sample);
                    }
                    else if (sample.sampleName == "File.Seek")
                    {
                        AddFileSeekData(frameData.frameIndex, thread, sample);
                    }
                    else if (sample.sampleName == "File.Close")
                    {
                        AddFileOpenCloseData(frameData.frameIndex, thread, sample);
                    }
                }
            }
        }

        private void SetupInfo(ref FileEvent evt,int idx, ThreadData thread, ProfilerSample sample)
        {
            evt.frameIdx = idx;
            evt.thread = thread.FullName;
            evt.startTime = sample.startTimeUS;
            evt.tm = sample.selfTimeUs / 1000.0f;
            evt.eventStr = sample.sampleName;
        }

        private void AddFileOpenCloseData(int idx, ThreadData thread, ProfilerSample sample)
        {
            FileEvent evt = new FileEvent();
            SetupInfo(ref evt, idx, thread, sample);

            var metaData = sample.metadataValues;
            if (metaData != null)
            {
                try
                {
                    if (metaData.Count > 0)
                    {
                        evt.file = metaData[0].convertedObject.ToString();
                    }
                }
                catch (System.Exception e)
                {
                    ProfilerLogUtil.logErrorException(e);
                }
            }

            fileEvents.Add(evt);
        }


        private void AddFileSeekData(int idx, ThreadData thread, ProfilerSample sample)
        {
            FileEvent evt = new FileEvent();
            SetupInfo(ref evt, idx, thread, sample);

            var metaData = sample.metadataValues;
            if (metaData != null)
            {
                try
                {
                    if (metaData.Count > 0)
                    {
                        evt.file = metaData[0].convertedObject.ToString();
                    }
                    if (metaData.Count > 1)
                    {
                        evt.seekOffset = (long)metaData[1].convertedObject;
                    }
                    if (metaData.Count > 2)
                    {
                        evt.param = (int)metaData[2].convertedObject;
                    }
                }
                catch (System.Exception e)
                {
                    ProfilerLogUtil.logErrorException(e);
                }
            }

            fileEvents.Add(evt);
        }

        private void AddFileReadData(int idx ,ThreadData thread, ProfilerSample sample)
        {
            FileEvent evt = new FileEvent();
            SetupInfo(ref evt, idx, thread, sample);

            var metaData = sample.metadataValues;
            if (metaData != null )
            {
                try
                {
                    if (metaData.Count > 0)
                    {
                        evt.file = metaData[0].convertedObject.ToString();
                    }
                    if (metaData.Count > 1)
                    {
                        evt.param = (int)metaData[1].convertedObject;
                    }
                    if (metaData.Count > 2)
                    {
                        evt.size = (ulong)metaData[2].convertedObject;
                    }
                }catch(System.Exception e)
                {
                    ProfilerLogUtil.logErrorException(e);
                }
            }

            fileEvents.Add(evt);
        }




        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            AppendHeaderToStringBuilder(csvStringGenerator);

            fileEvents.Sort((a, b) =>
            {
                if( a.startTime > b.startTime)
                {
                    return 1;
                }
                else if (a.startTime < b.startTime)
                {
                    return -1;
                }
                return 0;

            });

            foreach (var evt in fileEvents)
            {
                int filePathIdx = 0;
                int length = 0;
                if (evt.file != null)
                {
                    filePathIdx = evt.file.LastIndexOf('/') +1;
                    length = evt.file.Length;
                }

                csvStringGenerator.AppendColumn(evt.frameIdx);
                csvStringGenerator.AppendColumn(evt.file, filePathIdx, length - filePathIdx);
                csvStringGenerator.AppendColumn(evt.eventStr);
                csvStringGenerator.AppendColumn(evt.thread);
                csvStringGenerator.AppendColumn(evt.param);
                csvStringGenerator.AppendColumn(evt.size);
                csvStringGenerator.AppendColumn(evt.seekOffset);
                csvStringGenerator.AppendColumn(evt.tm);
                csvStringGenerator.AppendColumn(evt.file);
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }
        private void AppendHeaderToStringBuilder(CsvStringGenerator csvStringGenerator)
        {

            csvStringGenerator.AppendColumn("frameIdx");
            csvStringGenerator.AppendColumn("file");
            csvStringGenerator.AppendColumn("event");
            csvStringGenerator.AppendColumn("thread");
            csvStringGenerator.AppendColumn("param");
            csvStringGenerator.AppendColumn("size");
            csvStringGenerator.AppendColumn("seekOffset");
            csvStringGenerator.AppendColumn("execTime");
            csvStringGenerator.AppendColumn("fullPath");
            csvStringGenerator.NextRow();

        }


        protected override string FooterName
        {
            get
            {
                return "_engine_file_operate.csv";
            }
        }

    }
}
