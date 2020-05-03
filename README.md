# ProfilerReader
Tools for analyze profiler log data.<br />
Read this in other languages: English, [日本語](README.ja.md)<br />

## Summary
With this tool, you can analyze the binary log of "Unity Profiler".
For example, generate csv files that shows Samples allocating many Managed Heap from "Unity Profiler" binary log.

## Avalable versions
5.6 / 2017.1 / 2017.2 / 2017.3 / 2017.4 / 2018.1 / 2018.2 / 2018.3 / 2018.4 /2019.1/2019.2/2019.3

## GUI Interface

## CUI Interface
### Example Command
Unity.exe -batchMode -projectPath "ProjectPath" -logFile .\Editor.log -executeMethod UTJ.ProfilerReader.CUIInterface.ProfilerToCsv -PH.inputFile "Binary logFile(.data/.raw)" -PH.timeout 2400 -PH.log

And some csv file will be generated at subfolder for binary data.


## CSV Files:


