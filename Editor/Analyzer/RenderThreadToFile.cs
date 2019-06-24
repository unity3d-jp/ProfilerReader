using System.Collections.Generic;
using UTJ.ProfilerReader.BinaryData;
using System.Text;


namespace UTJ.ProfilerReader.Analyzer
{
    public class RenderThreadToFile : IAnalyzeFileWriter
    {

        private struct CameraRenderData
        {
            public float renderTime;
            public float updateDepth;
            public float opaque;
            public float transparent;
            public float imageEffect;
            /* TODO … 後回し
            public float shadowRendering;
            public float collectShadow;
            */


            public void AppendToStringBuilder(StringBuilder sb)
            {
                sb.Append(renderTime).Append(",");
                sb.Append(updateDepth).Append(",");
                sb.Append(opaque).Append(",");
                sb.Append(transparent).Append(",");
                sb.Append(imageEffect).Append(",");
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

            public void AppendToStringBuilder(StringBuilder sb)
            {
                sb.Append(frameIdx).Append(",");
                sb.Append(processCommandsTime).Append(",");
                sb.Append(waitForCommandsTime).Append(",");
                sb.Append(scheduleGeometryJobTime).Append(",");
                sb.Append(scheduleGeometryJobNum).Append(",");
                sb.Append(presentFrameTime).Append(",");
                sb.Append(guiRepaint).Append(",");
                sb.Append( cameraRenders.Count).Append(",");

                foreach (var cameraRender in cameraRenders)
                {
                    sb.Append(",");
                    cameraRender.AppendToStringBuilder(sb);
                }
            }
        }

        private List<FrameRenderingData> frameRenderingDatas = new List<FrameRenderingData>(512);
        private int maxCameraNum = 0;


        public void CollectData(ProfilerFrameData frameData)
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
        public void WriteResultFile(string path)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024 * 1024);
            sb.Append("frameIdx,processCommandsTime,waitForCommandsTime,scheduleGeometryJobTime,scheduleGeometryJobNum,presentFrameTime,guiRepaint,cameraRenders,");

            for (int i = 0; i < maxCameraNum; ++i)
            {
                sb.Append("Camera").Append(i). Append(",");
                sb.Append("renderTime,updateDepth,opaque,transparent,imageEffect,");
            }
            sb.Append("\n");

            foreach (var frameRenderingData in this.frameRenderingDatas)
            {
                frameRenderingData.AppendToStringBuilder(sb);
                sb.Append("\n");
            }

            System.IO.File.WriteAllText(path, sb.ToString());
        }

        public string ConvertPath(string originPath)
        {
            var path = originPath.Replace(".", "_") + "_renderthread.csv";
            return path;
        }
    }

}
