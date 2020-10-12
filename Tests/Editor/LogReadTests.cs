using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UTJ.ProfilerReader;


public class LogReadTests
{

    const string TestLogDir = "TestDatas/logdatas";
    private static string currentFile = "";

    [Test]
    public static void ReadAllTestFile(){
        var files = GetTargetFiles();


        ProfilerLogUtil.logErrorException = LogError;
        ProfilerLogUtil.logErrorString = LogError;

        foreach ( var file in files)
        {
#if UTJ_DEBUGMODE
            UTJ.ProfilerReader.RawData.Debug.DebugLogWrite.SetFile(file.Replace('/','_').Replace('\\', '_') + ".txt" );
#endif
            currentFile = file;
            UnityEngine.Debug.Log("file " + file);
            UTJ.ProfilerReader.CUIInterface.ProfilerToCsv(file, "TestDatas/result",true,false);
            UTJ.ProfilerReader.RawData.Debug.DebugLogWrite.Flush();
        }
    }

    private static void LogError(string str)
    {
        UTJ.ProfilerReader.RawData.Debug.DebugLogWrite.Log("[Error]file " + currentFile + str);
        UnityEngine.Debug.LogError("[Error] " + currentFile + "::" + str);
    }
    private static void LogError(System.Exception e)
    {
        UTJ.ProfilerReader.RawData.Debug.DebugLogWrite.Log("[Error]file " + currentFile + e.ToString() );
        UnityEngine.Debug.LogError("[Error] " + currentFile + "::" + e);
    }

    private static List<string> GetTargetFiles()
    {
        var rawFiles = System.IO.Directory.GetFiles(TestLogDir, "*.raw", System.IO.SearchOption.AllDirectories);
        var dataFiles = System.IO.Directory.GetFiles(TestLogDir, "*.data", System.IO.SearchOption.AllDirectories);

        List<string> files = new List<string>(rawFiles.Length + dataFiles.Length);
        files.AddRange(rawFiles);
        files.AddRange(dataFiles);

        return files;
    }
}
