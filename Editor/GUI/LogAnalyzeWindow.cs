using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UTJ.ProfilerReader;
using UTJ.ProfilerReader.BinaryData;
using UTJ.ProfilerReader.RawData;

namespace UTJ.ProfilerReader.UI{
    public class LogAnalyzeWindow : EditorWindow
    {
        private struct ColumnData
        {
            public ProfilerSample sample;
            public int frameIndex;
        }
        public enum EConditionType
        {
            StartsWith,
            Contains,
            EndsWith,
        }

        // 表示条件
        public enum ESampleCondition
        {
            Always,
            ParentOnly,
            ChildOnly,
        }
        private const int PagingNumber = 100;

        private ILogReaderPerFrameData logReader = null;
        private SampleDetailView treeView = new SampleDetailView();
        private List<ColumnData> columnList = new List<ColumnData>();
        private Vector2 scrollPos;
        private string filePath;

        private EConditionType stringCheckCondition;
        private ESampleCondition sampleCondition;

        private string sampleNameCondition;
        private float conditionExecuteTime;
        private int conditionAlloc;
        private int pageIndex;


        [MenuItem("Tools/UTJ/ProfilerReader/LogAnalyzer")]
        public static void CreateWindow()
        {
            EditorWindow.GetWindow<LogAnalyzeWindow>();
        }

        void OnEnable()
        {
        }

        void Update()
        {
            try
            {
                if (logReader != null)
                {
                    var data = logReader.ReadFrameData();
                    if (data == null) { return; }
                    this.CollectData(data);
                    System.GC.Collect();
                    if (data == null)
                    {
                        logReader = null;
                    }
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
            EditorGUILayout.LabelField("Profiler BinLog Hack");
            EditorGUILayout.BeginHorizontal();
            this.filePath = EditorGUILayout.TextField(this.filePath);
            if (GUILayout.Button("File", GUILayout.Width(40.0f)))
            {
                this.filePath = EditorUtility.OpenFilePanelWithFilters("", "Select BinaryLogFile", new string[]{ "profiler log", "data,raw" });
            }
            EditorGUILayout.EndHorizontal();
            {
                EditorGUILayout.LabelField("Concition");
                EditorGUILayout.BeginHorizontal();
                sampleNameCondition = EditorGUILayout.TextField("sample name ", sampleNameCondition);
                this.stringCheckCondition = (EConditionType)EditorGUILayout.EnumPopup(this.stringCheckCondition);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("HierarchyMode");
                this.sampleCondition = (ESampleCondition)EditorGUILayout.EnumPopup(this.sampleCondition);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                this.conditionExecuteTime = EditorGUILayout.FloatField("Execute time(ms)", this.conditionExecuteTime);
                this.conditionAlloc = EditorGUILayout.IntField("Alloc(byte)", conditionAlloc);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            if (!IsExecuting())
            {

                if (GUILayout.Button("Analyze", GUILayout.Width(100)))
                {
                    this.pageIndex = 1;
                    if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                    {
                        Debug.LogError("No such File ");
                    }
                    else
                    {
                        logReader = ProfilerLogUtil.CreateLogReader(filePath);
                        logReader.SetUnityVersion(Application.unityVersion);
                        columnList.Clear();
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Cancel", GUILayout.Width(100)))
                {
                    logReader.ForceExit();
                }
            }
            GUILayout.Label("");
            if (logReader != null && logReader.IsComplete)
            {
                if (GUILayout.Button("Write To CSV", GUILayout.Width(100)))
                {
                    SaveToCsv();
                }
            }
            EditorGUILayout.EndHorizontal();
            // execute now
            if (IsExecuting())
            {
                EditorGUILayout.LabelField("Progress " + logReader.Progress * 100.0f + "%");
            }
            else
            {
                this.ONGUIResultList();
                treeView.OnGUI();
            }
        }

        private bool IsExecuting()
        {
            return (logReader != null && 0.0f < logReader.Progress && logReader.Progress < 1.0f);
        }


        private void ONGUIResultList()
        {
            EditorGUILayout.BeginHorizontal();
            if( GUILayout.Button("<-",GUILayout.Width(40)))
            {
                if (pageIndex > 1)
                {
                    --pageIndex;
                }
            }
            var pageStr = EditorGUILayout.TextField(this.pageIndex.ToString(), GUILayout.Width(40));
            int.TryParse(pageStr, out this.pageIndex);
            EditorGUILayout.LabelField("/" + (columnList.Count + PagingNumber - 1) / PagingNumber, GUILayout.Width(40));

            if( GUILayout.Button("->", GUILayout.Width(40)))
            {
                if (pageIndex + 1 <= (columnList.Count + PagingNumber-1) / PagingNumber)
                {
                    ++pageIndex;
                }
            }
            EditorGUILayout.EndHorizontal();
            this.scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            for(int i = (this.pageIndex -1) * PagingNumber; i< columnList.Count && i < (this.pageIndex) * PagingNumber; ++ i)
            {
                if( i < 0 || i > columnList.Count) { continue; }
                ColumnData column = columnList[i];
                var sample = column.sample;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("More", GUILayout.Width(40)))
                {
                    this.treeView.SetCurrentSample(sample);
                }
                string str = column.frameIndex + "::" + sample.sampleName + "::" + (sample.timeUS / 1000.0f) +
                        "ms Alloc:";
                if (sample.totalGcAlloc < 1024 * 10)
                {
                    str += sample.totalGcAlloc + " Byte";
                }
                else if (sample.totalGcAlloc < 1024 * 1024 * 10)
                {
                    str += (sample.totalGcAlloc / 1024) + " KByte";
                }
                else
                {
                    str += (sample.totalGcAlloc / 1024 / 1024) + " MByte";
                }

                EditorGUILayout.LabelField(str);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void CollectData(ProfilerFrameData frameData)
        {
            var mainThread = frameData.MainThread;
            if( mainThread == null || mainThread.m_AllSamples == null ) { return; }
            List<ColumnData> hitInThisFrame = new List<ColumnData>();
            foreach (var sample in mainThread.m_AllSamples)
            {
                if (ChcekSearchCondition(sample))
                {
                    ColumnData data;
                    data.sample = sample;
                    data.frameIndex = frameData.frameIndex;
                    hitInThisFrame.Add(data);
                }
            }

            foreach (var hit in hitInThisFrame)
            {
                switch (this.sampleCondition)
                {
                    case ESampleCondition.Always:
                        this.columnList.Add(hit);
                        break;
                    case ESampleCondition.ChildOnly:
                        if (!IsChildExists(hit, hitInThisFrame))
                        {
                            this.columnList.Add(hit);
                        }
                        break;
                    case ESampleCondition.ParentOnly:
                        if (!IsParentExists(hit, hitInThisFrame))
                        {
                            this.columnList.Add(hit);
                        }
                        break;
                }
            }
        }
        private bool IsChildExists(ColumnData column, List<ColumnData> list)
        {
            foreach (var target in list)
            {
                if (target.sample != column.sample && IsParent(column.sample, target.sample))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsParentExists(ColumnData column, List<ColumnData> list)
        {
            foreach (var target in list)
            {
                if (target.sample != column.sample && IsParent(target.sample, column.sample))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsParent(ProfilerSample parent, ProfilerSample child)
        {
            for (ProfilerSample current = child.parent; current != null; current = current.parent)
            {
                if (current == parent)
                {
                    return true;
                }
            }
            return false;
        }


        private bool ChcekSearchCondition(ProfilerSample sample)
        {
            if (sample.totalGcAlloc < this.conditionAlloc)
            {
                return false;
            }
            if (sample.timeUS / 1000.0f < this.conditionExecuteTime)
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.sampleNameCondition))
            {
                return true;
            }
            switch (this.stringCheckCondition)
            {
                case EConditionType.Contains:
                    return sample.sampleName.Contains(this.sampleNameCondition);
                case EConditionType.StartsWith:
                    return sample.sampleName.StartsWith(this.sampleNameCondition);
                case EConditionType.EndsWith:
                    return sample.sampleName.EndsWith(this.sampleNameCondition);
            }
            return false;
        }

        void OnDisable()
        {
        }

        // CSV保存を押したときの処理
        private void SaveToCsv()
        {
            if (this.columnList == null || this.columnList.Count == 0)
            {
                EditorUtility.DisplayDialog("No Result", "There are no results.Please analyze before.", "OK");
                return;
            }
            if (0.0f < logReader.Progress && logReader.Progress < 1.0f)
            {
                EditorUtility.DisplayDialog("Progress", "Now executing...Wait a moment...", "OK");
                return;
            }
            string savePath = EditorUtility.SaveFilePanel("CSV file", "", "result", "csv");
            if (string.IsNullOrEmpty(savePath))
            {
                return;
            }
            WriteCsv(savePath);
        }

        // 実際にファイルを書き込むところ
        private void WriteCsv(string path)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("frame,SampleName,executeTime(ms),memoryAlloc(byte)\n");
            foreach (var column in columnList)
            {
                if (column.sample == null) { continue; }
                sb.Append(column.frameIndex).Append(",");
                // コンマあると
                sb.Append(column.sample.sampleName.Replace(',', '_')).Append(",");
                sb.Append(column.sample.timeUS / 1000.0f).Append(",");
                sb.Append(column.sample.totalGcAlloc).Append(",");
                sb.Append("\n");
            }

            System.IO.File.WriteAllText(path, sb.ToString());
            EditorUtility.DisplayDialog("Complete to write result","Save result to"+ path , "OK");

        }
    }



    public class SampleDetailView
    {

        private ProfilerSample currentSample;
        private Vector2 scrollPos;
        private Dictionary<ProfilerSample, bool> openList;
        private System.Text.StringBuilder stringBuilder;
        private Dictionary<uint, int> allocateBlock;
        private string[] guiTabStr = new string[] { "Children", "Memory" };

        private int guiTabSelect;

        public SampleDetailView()
        {
            openList = new Dictionary<ProfilerSample, bool>();
            stringBuilder = new System.Text.StringBuilder();
        }

        public void SetCurrentSample(ProfilerSample sample)
        {
            openList.Clear();
            this.currentSample = sample;
            scrollPos = Vector2.zero;
            if (sample != null)
            {
                allocateBlock = sample.GetBlockAllocCount();
            }
        }

        public void OnGUI()
        {
            if (this.currentSample == null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Not Select");
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.LabelField(currentSample.fullSampleName);
            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                this.SetCurrentSample(null);
                return;
            }
            this.guiTabSelect = GUILayout.Toolbar(this.guiTabSelect, this.guiTabStr);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.MinHeight(300));

            switch (this.guiTabSelect)
            {
                case 0:
                    ChildrenDraw(this.currentSample);
                    break;
                case 1:
                    AllocateBlockListDraw(); // test
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void ChildrenDraw(ProfilerSample sample)
        {
            if (sample == null) { return; }
            this.stringBuilder.Length = 0;
            this.stringBuilder.Append(sample.sampleName).Append("  ").Append((sample.timeUS / 1000.0f)).Append("ms Alloc:").Append(sample.totalGcAlloc).Append(" Byte");
            string outputStr = this.stringBuilder.ToString();

            //        style.contentOffset = new Vector2(sample.hierarchyLevel * 10.0f, 0.0f);

            bool isOpen = false;
            if (sample.children != null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.foldout);
                style.margin.left = (sample.hierarchyLevel - currentSample.hierarchyLevel) * 20;
                if (openList.TryGetValue(sample, out isOpen))
                {
                    bool openVal = EditorGUILayout.Foldout(isOpen, outputStr, style);
                    if (openVal != isOpen) { openList[sample] = openVal; }
                    if (isOpen)
                    {
                        foreach (var child in sample.children)
                        {
                            ChildrenDraw(child);
                        }
                    }
                }
                else
                {
                    openList.Add(sample, false);
                    EditorGUILayout.Foldout(isOpen, outputStr, style);
                }
            }
            else
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.padding.left = (sample.hierarchyLevel - currentSample.hierarchyLevel) * 20;
                EditorGUILayout.LabelField(outputStr, style);
            }
        }

        private void AllocateBlockListDraw()
        {
            if (allocateBlock == null) { return; }
            EditorGUILayout.LabelField("total Memory " + this.currentSample.totalGcAlloc + " Byte");
            EditorGUILayout.Space();
            List<uint> keys = new List<uint>(allocateBlock.Keys);
            keys.Sort();
            keys.Reverse();
            foreach (var key in keys)
            {
                EditorGUILayout.LabelField(key + " Byte X " + allocateBlock[key]);
            }
        }


        private void ParentGUIDraw(ProfilerSample sample)
        {
            if (sample == null) { return; }
            ParentGUIDraw(sample.parent);
            EditorGUILayout.LabelField(sample.sampleName);
        }
    }
}