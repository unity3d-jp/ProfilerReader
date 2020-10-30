using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.Analyzer;
using System.Reflection;
using System.IO;

namespace UTJ.ProfilerReader.UI {



    public class AnalyzeToCsvWindow : EditorWindow
    {


        private class FileWriterFlag
        {
            public string name;
            public System.Type type;
            public bool flag;
            
        }

        private List<FileWriterFlag> fileWriterFlags = new List<FileWriterFlag>();
        private List<IAnalyzeFileWriter> analyzeExecutes = new List<IAnalyzeFileWriter>();
        private ILogReaderPerFrameData logReader = null;
        private bool isFirstFrame = true;
        private Vector2 scrollPos;
        private string filePath;

        private string outputDir;
        private string logfilename;
        private bool isWindowExists;

        private bool forceStopRequest = false;
        private bool requestDialogFlag = false;

        private bool isMultiThreadExecute = true;

        [MenuItem("Tools/UTJ/ProfilerReader/AnalyzeToCsv")]
        public static void CreateWindow()
        {
            EditorWindow.GetWindow<AnalyzeToCsvWindow>();
        }

        private void OnEnable()
        {
            this.isWindowExists = true;
            this.fileWriterFlags.Clear();
            var types = AnalyzerUtil.GetInterfaceType<IAnalyzeFileWriter>();
            foreach( var t in types)
            {
                this.fileWriterFlags.Add(new FileWriterFlag() { name = t.Name, type = t, flag = true });
            }
        }
        private void OnDisable()
        {
            this.isWindowExists = false;
        }

        private void UpdateThread()
        {
            while (this.isWindowExists)
            {
                if(!ExecuteFrame()){
                    return;
                }
            }
        }

        bool ExecuteFrame()
        {
            if (logReader == null)
            {
                return false;
            }
            try
            {
                var frameData = logReader.ReadFrameData();
                if (isFirstFrame)
                {
                    InitOutputPathInfo();
                    SetAnalyzerInfo(analyzeExecutes, logReader);
                    isFirstFrame = false;
                }
                if (frameData == null || forceStopRequest)
                {
                    // write all result
                    foreach (var analyzer in this.analyzeExecutes)
                    {
                        analyzer.WriteResultFile(logfilename, outputDir);
                    }
                    requestDialogFlag = true;
                    logReader = null;
                    return false;
                }
                foreach (var analyzer in this.analyzeExecutes)
                {
                    try
                    {
                        analyzer.CollectData(frameData);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
                System.GC.Collect();

            }
            catch (System.Exception e)
            {
                logReader = null;
                Debug.LogError(e);
            }
            return true;
        }

        private void DisplayDialog()
        {
            requestDialogFlag = false;
            string dialogStr = "Write to csv files\n";
            foreach (var analyzer in this.analyzeExecutes)
            {
                dialogStr += analyzer.GetType() + "\n";
            }
            EditorUtility.DisplayDialog("Result", dialogStr, "ok");
            analyzeExecutes.Clear();
        }


        void OnGUI()
        {
            EditorGUILayout.LabelField("Convert profiler log to csv");
            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(filePath))
            {
                EditorGUILayout.LabelField("Select File");
            }
            else
            {
                EditorGUILayout.LabelField(this.filePath);
            }
            if (GUILayout.Button("File", GUILayout.Width(40.0f)))
            {
                this.filePath = EditorUtility.OpenFilePanelWithFilters("", "Select BinaryLogFile", new string[] { "profiler log", "data,raw" });
            }

            if (!IsExecute())
            {
                if (GUILayout.Button("Analyze", GUILayout.Width(100)))
                {
                    if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                    {
                        Debug.LogError("No such File ");
                    }
                    else
                    {
                        StartAnalyze();
                    }
                }
            }
            else
            {
                if (GUILayout.Button("ForceExit", GUILayout.Width(100)))
                {
                    forceStopRequest = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (IsExecute() )
            {
                EditorGUILayout.LabelField("Progress " + logReader.Progress * 100.0f + "%");
            }
            else
            {
                for (int i = 0; i < fileWriterFlags.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    this.fileWriterFlags[i].flag = EditorGUILayout.Toggle(this.fileWriterFlags[i].flag ,GUILayout.Width(20) );
                    EditorGUILayout.LabelField(this.fileWriterFlags[i].name);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.LabelField("The results are in csv file.");
        }

        private void StartAnalyze()
        {
            logReader = ProfilerLogUtil.CreateLogReader(filePath);
            isFirstFrame = true;
            forceStopRequest = false;
            analyzeExecutes.Clear();
            for (int i = 0; i < fileWriterFlags.Count; ++i)
            {
                if (this.fileWriterFlags[i].flag)
                {
                    var analyzer = System.Activator.CreateInstance(fileWriterFlags[i].type) as IAnalyzeFileWriter;
                    analyzeExecutes.Add(analyzer);
                }
            }
            // start analyze
            if (isMultiThreadExecute)
            {
                var thread = new System.Threading.Thread(this.UpdateThread);
                thread.Start();
            }
        }


        private void Update()
        {
            if (!isMultiThreadExecute)
            {
                this.ExecuteFrame();
            }
            if (IsExecute())
            {
                this.Repaint();
            }
            if (requestDialogFlag)
            {
                this.DisplayDialog();
            }
        }

        private bool IsExecute()
        {
            return (logReader != null && 0.0f < logReader.Progress && logReader.Progress < 1.0f);
        }

        private void InitOutputPathInfo()
        {
            this.logfilename = Path.GetFileName(this.filePath);
            this.outputDir = Path.Combine(Path.GetDirectoryName(this.filePath), logfilename.Replace('.', '_'));
            if (!Directory.Exists(this.outputDir))
            {
                Directory.CreateDirectory(this.outputDir);
            }
        }

        private void SetAnalyzerInfo(List<IAnalyzeFileWriter> analyzeExecutes, ILogReaderPerFrameData logReader)
        {

            ProfilerLogFormat format = ProfilerLogFormat.TypeData;
            if (logReader.GetType() == typeof(UTJ.ProfilerReader.RawData.ProfilerRawLogReader))
            {
                format = ProfilerLogFormat.TypeRaw;
            }
            foreach (var analyzer in analyzeExecutes)
            {
                analyzer.SetInfo(format, Application.unityVersion, logReader.GetLogFileVersion(), logReader.GetLogFilePlatform());
                analyzer.SetFileInfo(logfilename, this.outputDir);
            }
        }


    }


}