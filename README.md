# ProfilerReader
Tools for analyze profiler log data.<br />
Read this in other languages: English, [日本語](README.ja.md)<br />

## Summary
With this tool, you can analyze the binary log of "Unity Profiler".
For example, generate csv files that shows Samples allocating many Managed Heap from "Unity Profiler" binary log.

## Avalable versions
2019.4 / 2020.3/2021.1/2021.2/2021.3/2022.2/2022.3
<br/>2022.1 skip

## Filter Search
Call "Tools->UTJ->ProfilerReader->AnalyzeToCsv" and then this window will be displayed.<br />
![alt text](Documentation~/img/ProfilerReaderFilter.png)
<br />
1.Set Profiler log file.<br />
2.Set Conditions to search samples.<br />
3.Execute Analyze<br />
4.The results are here<br />
5.Write the results to csv file.<br />

## CSV Feature
### GUI
Call "Tools->UTJ->ProfilerReader->AnalyzeToCsv" and then this window will be displayed.<br />
![alt text](Documentation~/img/ProfilerLogToCsv.png)

<br />
This tool generate summarized csv files.<br />
1.Select the categories to generate csv files.<br />
2.Set Profler log File.<br />
3.Execute Analyze<br />

### CUI Sample
Unity.exe -batchMode -projectPath "ProjectPath" -logFile .\Editor.log -executeMethod UTJ.ProfilerReader.CUIInterface.ProfilerToCsv -PH.inputFile "Binary logFile(.data/.raw)" -PH.timeout 2400 -PH.log

And some csv file will be generated at subfolder for binary data.


## CSV Files:
This tool generate csv files with footer name.<br />
These are samples.<br />

 -"xxx_mainThread_frame.csv"<br />
The CPU stats by category in each fraemes.
<br />
 -"xxx_gc_result.csv"<br />
The list that shows "GC.Alloc".
<br />
 -"xxx_gc_detail.csv"<br />
The list of "GC.Alloc" and with Callstackinfo.
<br />
 -"xxx_gpu_sample.csv"<br />
The list about GPU status in each frames .
<br />
 -"xxx_main_self.csv"<br />
The list about CPU Samples.
<br />
 -"xxx_memory.csv"<br />
The list about Memory status in each frames .
<br />
 -"xxx_rendering.csv"<br />
The list about Rendering status in each frames .
<br />
 -"xxx_renderthread.csv"<br />
The list about RenderThread status in each frames .
<br />
 -"xxx_result.csv"<br />
The list about threads status in each frames .
<br />
 -"xxx_shader_compile.csv"<br />
The list that shows Shader Compiling.
<br />
 -"xxx_urp_gpu_sample.csv"<br />
The list about GPU for Universal RP.
<br />
 -"xxx_worker.csv"<br />
The list about workerThread status.
<br />
 - "xxx_jitInfos.csv"<br />
The list of callstack symbol infos.( should enable "Profiler.enableAllocationCallstacks").
<br />
