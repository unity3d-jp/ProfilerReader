using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;
using System.Text;


namespace UTJ.ProfilerReader.Analyzer
{
    public class RenderThreadToFile : AnalyzeToTextbaseFileBase
    {

        private struct CameraRenderData
        {
            public float renderTime;
            public float updateDepth;
            public float opaque;
            public float transparent;
            public float imageEffect;


            public void AppendToStringBuilder(CsvStringGenerator csvStringGenerator)
            {
                csvStringGenerator.AppendColumn(renderTime);
                csvStringGenerator.AppendColumn(updateDepth);
                csvStringGenerator.AppendColumn(opaque);
                csvStringGenerator.AppendColumn(transparent);
                csvStringGenerator.AppendColumn(imageEffect);
            }
        }


        private class FrameRenderingData
        {
            public int frameIdx;
            public float processCommandsTime;
            public float waitForCommandsTime;

            public float scheduleGeometryJobTime;
            public int scheduleGeometryJobNum;
            public float presentFrameTime;
            public float guiRepaint;

            public List<CameraRenderData> cameraRenders = new List<CameraRenderData>(8);

            public void AppendToCsvGenerator(CsvStringGenerator csvStringGenerator)
            {
                csvStringGenerator.AppendColumn(frameIdx);
                csvStringGenerator.AppendColumn(processCommandsTime);
                csvStringGenerator.AppendColumn(waitForCommandsTime);
                csvStringGenerator.AppendColumn(scheduleGeometryJobTime);
                csvStringGenerator.AppendColumn(scheduleGeometryJobNum);
                csvStringGenerator.AppendColumn(presentFrameTime);
                csvStringGenerator.AppendColumn(guiRepaint);
                csvStringGenerator.AppendColumn( cameraRenders.Count);

                foreach (var cameraRender in cameraRenders)
                {
                    csvStringGenerator.AppendColumn("");
                    cameraRender.AppendToStringBuilder(csvStringGenerator);
                }
            }
        }

        private List<FrameRenderingData> frameRenderingDatas = new List<FrameRenderingData>(512);
        private int maxCameraNum = 0;


        public override void CollectData(ProfilerFrameData frameData)
        {
            foreach( var threadData in frameData.m_ThreadData){
                if(threadData.m_ThreadName == "Render Thread")
                {
                    CollectRenderThreadData(frameData.frameIndex, threadData);
                }
            }
        }

        private void CollectRenderThreadData(int frameIdx,ThreadData threadData)
        {
            if(threadData.m_AllSamples == null) { return; }
            FrameRenderingData frameRenderingData = new FrameRenderingData();
            frameRenderingData.frameIdx = frameIdx;
            frameRenderingData.processCommandsTime = threadData.m_AllSamples[0].timeUS / 1000.0f;

            int cameraNum = 0;
            foreach ( var sample in threadData.m_AllSamples)
            {
                switch(sample.sampleName ){
                    case "Camera.Render":
                        {
                            var cameraRender = CollectCameraRenderData(sample);
                            frameRenderingData.cameraRenders.Add(cameraRender);
                            ++cameraNum;
                        }
                        break;
                    case "Gfx.WaitForCommands":
                        frameRenderingData.waitForCommandsTime += sample.timeUS / 1000.0f;
                        break;
                    case "Gfx.PresentFrame":
                        frameRenderingData.presentFrameTime += sample.timeUS / 1000.0f;
                        break;
                    case "ScheduleGeometryJobs":
                        frameRenderingData.scheduleGeometryJobTime += sample.timeUS / 1000.0f;
                        frameRenderingData.scheduleGeometryJobNum += 1;
                        break;
                    case "GUIRepaint":
                        frameRenderingData.guiRepaint += sample.timeUS / 1000.0f;
                        break;
                }
            }
            if(maxCameraNum < cameraNum)
            {
                maxCameraNum = cameraNum;
            }
            frameRenderingDatas.Add(frameRenderingData);
        }

        private CameraRenderData CollectCameraRenderData( ProfilerSample profilerSample)
        {
            CameraRenderData renderData = new CameraRenderData();
            renderData.renderTime = profilerSample.timeUS / 1000.0f; 
            VisitChildren(profilerSample, (sample) => {
                switch(sample.sampleName){
                    case "UpdateDepthTexture":
                        renderData.updateDepth = sample.timeUS / 1000.0f;
                        return true;
                    case "Render.OpaqueGeometry":
                        renderData.opaque = sample.timeUS / 1000.0f;
                        return true;
                    case "Render.TransparentGeometry":
                        renderData.transparent = sample.timeUS / 1000.0f;
                        return true;
                    case "Camera.ImageEffects":
                        renderData.imageEffect = sample.timeUS / 1000.0f;
                        return true;
                }
                return false;
            });
            return renderData;
        }

        private List<ProfilerSample> FindChildren(ProfilerSample profilerSample,string name)
        {
            List<ProfilerSample> results = new List<ProfilerSample>();

            return results;
        }
        private void VisitChildren(ProfilerSample sample, 
            System.Func<ProfilerSample,bool> filter)
        {
            if( sample == null) { return; }
            if( filter(sample)) {
                return;
            }
            if( sample.children == null)
            {
                return;
            }
            foreach( var child in sample.children)
            {
                VisitChildren(child, filter);
            }
        }

        /// <summary>
        /// 結果書き出し
        /// </summary>
        protected override string GetResultText()
        {
            CsvStringGenerator csvStringGenerator = new CsvStringGenerator();
            csvStringGenerator.AppendColumn("frameIdx").AppendColumn("processCommandsTime").
                AppendColumn("waitForCommandsTime").AppendColumn("scheduleGeometryJobTime").AppendColumn("scheduleGeometryJobNum").AppendColumn("presentFrameTime").AppendColumn("guiRepaint").AppendColumn("cameraRenders");

            for (int i = 0; i < maxCameraNum; ++i)
            {
                csvStringGenerator.AppendColumn("Camera"+i);
                csvStringGenerator.AppendColumn("renderTime").AppendColumn("updateDepth").AppendColumn("opaque").AppendColumn("transparent").AppendColumn("imageEffect");
            }
            csvStringGenerator.NextRow();

            foreach (var frameRenderingData in this.frameRenderingDatas)
            {
                frameRenderingData.AppendToCsvGenerator(csvStringGenerator);
                csvStringGenerator.NextRow();
            }

            return csvStringGenerator.ToString();
        }
        

        protected override string FooterName
        {
            get
            {
                return "_renderthread.csv";
            }
        }


    }

}