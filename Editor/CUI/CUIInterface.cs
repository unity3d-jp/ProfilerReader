using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.Analyzer;
using UnityEngine;

namespace UTJ.ProfilerReader
{
    public class CUIInterface
    {
        const int NormalCode = 0;
        const int TimeoutCode = 10;
        const int ReadErrorCode = 11;

        public enum CsvFileType
        {

        };

        private static int timeoutSec = 0;
        private static ILogReaderPerFrameData currentReader = null;
        private static bool timeouted = false;


        public static void SetTimeout(int sec)
        {
            Debug.Log("SetTimeout " + sec);
            timeoutSec = sec;
            System.Threading.Thread th = new System.Threading.Thread(TimeOutExecute);
            th.Start();
        }
        public static void TimeOutExecute()
        {
            System.Threading.Thread.Sleep(timeoutSec * 1000);
            Debug.Log("Timeout!!!");
            currentReader.ForceExit();
            timeouted = true;
        }

        public static void ProfilerToCsv()
        {
            var args = System.Environment.GetCommandLineArgs();
            string inputFile = null;
            string outputDir = null;
            bool exitFlag = true;
            bool logFlag = false;

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-PH.inputFile")
                {
                    inputFile = args[i + 1];
                    i += 1;
                }
                if (args[i] == "-PH.outputDir")
                {
                    outputDir = args[i + 1];
                    i += 1;
                }
                if (args[i] == "-PH.timeout")
                {
                    SetTimeout(int.Parse(args[i + 1]));
                }
                if (args[i] == "-PH.exitcode")
                {
                    exitFlag = true;
                }
                if (args[i] == "-PH.log")
                {
                    logFlag = true;
                }
            }
            int code = ProfilerToCsv(inputFile, outputDir, logFlag);
            if(timeouted)
            {
                code = TimeoutCode;
            }
            if (exitFlag)
            {
                UnityEditor.EditorApplication.Exit(code);
            }
        }

        public static int ProfilerToCsv(string inputFile,string outputDir,bool logFlag)
        {
            int retCode = NormalCode;
            if ( string.IsNullOrEmpty(outputDir))
            {
                outputDir = System.IO.Path.GetDirectoryName(inputFile);
            }

            var logReader = ProfilerLogUtil.CreateLogReader(inputFile);
            currentReader = logReader;

            List<IAnalyzeFileWriter> analyzeExecutes = new List<IAnalyzeFileWriter>();
            analyzeExecutes.Add(new ThreadAnalyzeToFile());
            analyzeExecutes.Add(new WorkerJobAnalyzeToFile());
            analyzeExecutes.Add(new MainThreadAnalyzeToFile());
            analyzeExecutes.Add(new MemoryAnalyzeToFile());
            analyzeExecutes.Add(new RenderingAnalyzeToFile());
            analyzeExecutes.Add(new RenderThreadToFile());

            var frameData = logReader.ReadFrameData();

            if( frameData == null)
            {
                Debug.LogError("No FrameDataFile " + inputFile);
            }
            // Loop and execute each frame
            while (frameData != null)
            {
                try
                {
                    frameData = logReader.ReadFrameData();
                    if (logFlag && frameData != null)
                    {
                        System.Console.WriteLine("ReadFrame:" + frameData.frameIndex);
                    }
                }
                catch (System.Exception e)
                {
                    retCode = ReadErrorCode;
                    Debug.LogError(e);
                }

                foreach (var analyzer in analyzeExecutes)
                {
                    try
                    {
                        if (frameData != null) {
                            analyzer.CollectData(frameData);
                        }
                    }catch(System.Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
                System.GC.Collect();
            }
            foreach (var analyzer in analyzeExecutes)
            {
                string path = System.IO.Path.Combine(outputDir , analyzer.ConvertPath(System.IO.Path.GetFileName( inputFile) ) );
                analyzer.WriteResultFile(path);
            }

            return retCode;
        }
    }
}