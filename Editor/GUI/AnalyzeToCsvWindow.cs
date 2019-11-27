using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader.Analyzer;
using System.Reflection;
using UnityEngine;

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
        private Vector2 scrollPos;
        private string filePath;



        [MenuItem("Tools/ProfilerReader/AnalyzeToCsv")]
        public static void CreateWindow()
        {
            EditorWindow.GetWindow<AnalyzeToCsvWindow>();
        }

        private void OnEnable()
        {
            this.fileWriterFlags.Clear();
            var types = AnalyzerUtil.GetInterfaceType<IAnalyzeFileWriter>();
            foreach( var t in types)
            {
                this.fileWriterFlags.Add(new FileWriterFlag() { name = t.Name, type = t, flag = true });
            }
        }

        void Update()
        {
            try
            {
                if (logReader != null)
                {
                    var frameData = logReader.ReadFrameData();
                    if (frameData == null)
                    {
                        // 終わったタイミングでcsv 
                        string dialogStr = "Write to csv files\n";
                        string outputDir = System.IO.Path.GetDirectoryName(this.filePath);
                        string logfilename = System.IO.Path.GetFileName(this.filePath);

                        foreach (var analyzer in this.analyzeExecutes)
                        {
                            analyzer.WriteResultFile( logfilename , outputDir);
                            dialogStr += analyzer.GetType() + "\n";
                        }
                        EditorUtility.DisplayDialog("Result", dialogStr, "ok");
                        analyzeExecutes.Clear();
                        logReader = null;
                        this.Repaint();
                        return;
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
                    this.Repaint();
                }
            }
            catch (System.Exception e)
            {
                logReader = null;
                Debug.LogError(e);
            }
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
                        logReader = ProfilerLogUtil.CreateLogReader(filePath);
                        for (int i = 0; i < fileWriterFlags.Count; ++i)
                        {
                            if (this.fileWriterFlags[i].flag)
                            {
                                analyzeExecutes.Add(System.Activator.CreateInstance(fileWriterFlags[i].type) as IAnalyzeFileWriter);
                            }
                        }
                    }
                }
            }
            else
            {
                if (GUILayout.Button("ForceExit", GUILayout.Width(100)))
                {
                    if (logReader != null)
                    {
                        logReader.ForceExit();
                    }
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

        private bool IsExecute()
        {
            return (logReader != null && 0.0f < logReader.Progress && logReader.Progress < 1.0f);
        }


    }


}