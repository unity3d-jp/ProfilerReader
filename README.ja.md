# ProfilerReader
プロファイラーのログファイルを解析するツールです<br />
Read this in other languages: [English](README.md), 日本語<br />

## 概要
このツールを "Unity Profiler".
For example, generate csv files that shows Samples allocating many Managed Heap from "Unity Profiler" binary log.

## 対応バージョン
2019.4 / 2020.3/2021.1/2021.2/2021.3/2022.2/2022.3
<br/>2022.1 skip

## Filter検索機能
「Tools->UTJ->ProfilerReader->AnalyzeToCsv」で下記ウィンドウが出ます。<br />
![alt text](Documentation~/img/ProfilerReaderFilter.png)
<br />
1.プロファイラーのログを指定します<br />
2.見つけたいサンプルの条件を指定します<br />
3.条件に合うサンプルを見つけます<br />
4.条件に合ったサンプルの結果を表示します<br />
5.結果をCSVに書き出します。<br />

<br />

## CSV化機能
### GUI操作について
「Tools->UTJ->ProfilerReader->AnalyzeToCsv」で下記ウィンドウが出ます。<br />
![alt text](Documentation~/img/ProfilerLogToCsv.png)

<br />
このWindowではCSV化されたサマリーを生成します。<br />
1.書き出しを行う種別の指定を行います。<br />
2.Profilerのログファイルを指定します。<br />
3.解析を実行します<br />

### CUI操作でのサンプル
Unity.exe -batchMode -projectPath "ProjectPath" -logFile .\Editor.log -executeMethod UTJ.ProfilerReader.CUIInterface.ProfilerToCsv -PH.inputFile "Binary logFile(.data/.raw)" -PH.timeout 2400 -PH.log

バイナリログファイルと同じ場所にサブフォルダを作成し、そちらにCSVファイルが生成されます。

## CSV Files:
CSファイルは、元のファイル名にフッターを付けたファイル名で出力します。<br />
下記のような形でCSVはいくつか書き出します<br />

■「xxx_mainThread_frame.csv」<br />
フレーム毎のメインスレッドでのカテゴリ別のCPU処理負荷のリスト
<br />
■「xxx_gc_result.csv」<br />
全体を通したGCメモリ確保個所のリスト
<br />
■「xxx_gpu_sample.csv」<br />
ProfilerのGPU項目をフレーム毎にカテゴリ別に出されるリスト
<br />
■「xxx_main_self.csv」<br />
全体を通してサンプル別のCPU負荷リスト
<br />
■「xxx_memory.csv」<br />
ProfilerのMemory項目をフレーム毎に出したリスト
<br />
■「xxx_rendering.csv」<br />
ProfilerのRendering項目をフレーム毎に出したリスト
<br />
■「xxx_renderthread.csv」<br />
RenderThreadの状況をフレーム毎に出したリスト
<br />
■「xxx_result.csv」<br />
フレーム毎のThreadの状況リスト
<br />
■「xxx_shader_compile.csv」<br />
Shaderコンパイルが走った状況を書き出すリスト
<br />
■「xxx_urp_gpu_sample.csv」<br />
GPU項目をUniversal RP向けに書き出したリスト
<br />
■「xxx_worker.csv」<br />
WorkerThreadの状況を書き出したリスト
<br />

