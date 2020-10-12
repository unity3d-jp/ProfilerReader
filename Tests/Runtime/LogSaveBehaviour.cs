using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class LogSaveBehaviour : MonoBehaviour
{
    private string saveProfilerName;

    const string EditorSaveFolder = "ProfilerTemp";

    public static void Initialize()
    {
        var gameObject = new GameObject();
        gameObject.AddComponent<LogSaveBehaviour>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var date = System.DateTime.Now;
        var dateString = string.Format("{0:D4}{1:D2}{2:D2}_{3:D2}{4:D2}", date.Year,date.Month,date.Day,date.Hour,date.Minute);

        saveProfilerName = "profile_" + Application.unityVersion + "_" + Application.platform + "_" + dateString;

#if UNITY_EDITOR
        UnityEditorInternal.ProfilerDriver.enabled = true;
        if (!System.IO.Directory.Exists(EditorSaveFolder))
        {
            System.IO.Directory.CreateDirectory(EditorSaveFolder);
        }
        Profiler.logFile = System.IO.Path.Combine(EditorSaveFolder , saveProfilerName );
#else
        Profiler.logFile = System.IO.Path.Combine(Application.persistentDataPath, saveProfilerName );

#endif
        Profiler.enableBinaryLog = true;
        Profiler.enabled = true;
#if UNITY_2018_3_OR_NEWER
        foreach( var profilerarea in System.Enum.GetValues(typeof(ProfilerArea)))
        {
            Profiler.SetAreaEnabled((ProfilerArea)profilerarea, true);
        }
#endif

#if UNITY_2019_1_OR_NEWER
        guid = new System.Guid();
#endif
    }
#if UNITY_2019_1_OR_NEWER
    private System.Guid guid;
#endif

    private void Update()
    {
        byte[] data = new byte[128];
        for( int i = 0; i < data.Length; ++i)
        {
            data[i] = (byte)i;
        }
#if UNITY_2019_1_OR_NEWER     
        Profiler.EmitFrameMetaData(guid,0,data);
#endif
    }

    private void OnDestroy()
    {
        Profiler.logFile = null;
        Profiler.enableBinaryLog = false;
        Profiler.enabled = false;

#if UNITY_EDITOR && (UNITY_2017_3_OR_NEWER || UNITY_2018)
        UnityEditorInternal.ProfilerDriver.SaveProfile(System.IO.Path.Combine(EditorSaveFolder, saveProfilerName + ".data") );
#endif
    }

}
