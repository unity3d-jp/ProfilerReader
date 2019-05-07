using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.Analyzer;
using UnityEngine;

namespace UTJ.ProfilerReader
{
    public class CUIInterface
    {

        public enum CsvFileType
        {

        };

        private static int timeoutSec = 0;
        public static void SetTimeout(int sec)
        {
            timeoutSec = sec;
            System.Threading.Thread th = new System.Threading.Thread(TimeOutExecute);
            th.Start();
        }
        public static void TimeOutExecute()
        {
            System.Threading.Thread.Sleep(timeoutSec * 1000);
            System.Environment.Exit(1);
        }

        public static void ProfilerToCsv()
        {
            var args = System.Environment.GetCommandLineArgs();
            string inputFile = null;
            string outputDir = null;

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
                    SetTimeout( int.Parse( args[i + 1] ) );
                }
            }
            ProfilerToCsv(inputFile, outputDir);
        }

        public static void ProfilerToCsv(string inputFile,string outputDir)
        {
            if( string.IsNullOrEmpty(outputDir))
            {
                outputDir = System.IO.Path.GetDirectoryName(inputFile);
            }

            var logReader = ProfilerLogUtil.CreateLogReader(inputFile);

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
                frameData = logReader.ReadFrameData();

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
        }
    }
}