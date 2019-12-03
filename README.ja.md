# ProfilerReader
プロファイラーのログファイルを解析するツールです<br />
Read this in other languages: [English](README.md), 日本語<br />

## 概要
このツールを "Unity Profiler".
For example, generate csv files that shows Samples allocating many Managed Heap from "Unity Profiler" binary log.

## 対応バージョン
5.6 / 2017.1 / 2017.2 / 2017.3 / 2017.4 / 2018.1 / 2018.2 / 2018.3 / 2018.4 
対応中：2019.1/2019.2/2019.3

## GUI操作について

## CUI操作について
### サンプルコマンド
Unity.exe -batchMode -projectPath "ProjectPath" -logFile .\Editor.log -executeMethod UTJ.ProfilerReader.CUIInterface.ProfilerToCsv -PH.inputFile "Binary logFile(.data/.raw)" -PH.timeout 2400 -PH.log

バイナリログファイルと同じ場所に、CSVファイルが生成されます。

## CSV Files:


