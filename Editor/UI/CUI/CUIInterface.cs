using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.Analyzer;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

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

        private static string overrideUnityVersion = null;


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
            bool isLegacyOutputDirPath = false;

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
                    i += 1;
                }
                if( args[i] == "-PH.overrideUnityVersion")
                {
                    overrideUnityVersion = args[i + 1];
                    i += 1;
                }
                if (args[i] == "-PH.exitcode")
                {
                    exitFlag = true;
                }
                if (args[i] == "-PH.log")
                {
                    logFlag = true;
                }
                if (args[i] == "-PH.dirLegacy") ;
                {
                    isLegacyOutputDirPath = true;
                }
            }
            int code = ProfilerToCsv(inputFile, outputDir, logFlag, isLegacyOutputDirPath);
            if(timeouted)
            {
                code = TimeoutCode;
            }
            if (exitFlag)
            {
                UnityEditor.EditorApplication.Exit(code);
            }
        }

        public static int ProfilerToCsv(string inputFile,string outputDir,bool logFlag,bool isLegacyOutputDirPath)
        {
            int retCode = NormalCode;
            if ( string.IsNullOrEmpty(outputDir))
            {
                if (isLegacyOutputDirPath)
                {
                    outputDir = Path.GetDirectoryName(inputFile);
                }
                else
                {
                    string file = Path.GetFileName(inputFile);
                    outputDir = Path.Combine(Path.GetDirectoryName(inputFile), file.Replace('.', '_'));
                }
            }

            var logReader = ProfilerLogUtil.CreateLogReader(inputFile);
            currentReader = logReader;

            List<IAnalyzeFileWriter> analyzeExecutes = AnalyzerUtil.CreateAnalyzerInterfaceObjects();

            var frameData = logReader.ReadFrameData();
            SetAnalyzerInfo(analyzeExecutes, logReader,outputDir,inputFile);

            if ( frameData == null)
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

                if (frameData != null)
                {
                    List<Task> tasks = new List<Task>(analyzeExecutes.Count);
                    foreach (var analyzer in analyzeExecutes)
                    {
                        var task = Task.Run(() =>
                        {
                            try
                            {
                                analyzer.CollectData(frameData);

                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError(e);
                            }
                        });
                        tasks.Add(task);
                    }
                    while (true)
                    {
                        bool isComplete = true;
                        foreach (var task in tasks)
                        {
                            if (!task.IsCompleted)
                            {
                                isComplete = false;
                                break;
                            }
                        }
                        if (isComplete)
                        {
                            break;
                        }
                    }
                }
                System.GC.Collect();
            }
            foreach (var analyzer in analyzeExecutes)
            {
                analyzer.WriteResultFile(System.IO.Path.GetFileName(inputFile), outputDir);
            }

            return retCode;
        }
        private static void SetAnalyzerInfo(List<IAnalyzeFileWriter> analyzeExecutes,
            ILogReaderPerFrameData logReader,
            string outDir,string inFile)
        {
            ProfilerLogFormat format = ProfilerLogFormat.TypeData;
            if (logReader.GetType() == typeof(UTJ.ProfilerReader.RawData.ProfilerRawLogReader))
            {
                format = ProfilerLogFormat.TypeRaw;
            }
            string unityVersion = Application.unityVersion;
            if ( !string.IsNullOrEmpty(overrideUnityVersion))
            {
                unityVersion = overrideUnityVersion;
            }
            foreach (var analyzer in analyzeExecutes)
            {
                analyzer.SetInfo(format, unityVersion, logReader.GetLogFileVersion(), logReader.GetLogFilePlatform());
                analyzer.SetFileInfo(inFile,outDir);
            }
        }
    }
}