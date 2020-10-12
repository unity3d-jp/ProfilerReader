
namespace UTJ.ProfilerReader.Protocol
{
    // s_ProfilerCategoryInfos
    [CategoryVersion("2018.3", "2018.4")]
    internal class Category201803 : ICategory
    {
        // 2018_3
        public string[] GetCategories()
        {
            string[] category = new string[] {
                "Render"         ,
                "Scripts"        ,
                "Managed Jobs"   ,
                "Burst Jobs"     ,
                "GUI"            ,
                "Physics"        ,
                "Animation"      ,
                "AI"             ,
                "Audio"          ,
                "Video"          ,
                "Particles"      ,
                "Gi"             ,
                "Network"        ,
                "Loading"        ,
                "Other"          ,
                "GC"             ,
                "VSync"          ,
                "Overhead"       ,
                "PlayerLoop"     ,
                "Director"       ,
                "VR"             ,
                "NativeMem"      ,
                "Internal"       ,
                "FileIO"         ,
                "UI Layout"      ,
                "UI Render"      ,
                "VFX"            ,
                "Build Interface",
                "Input",
            };
            return category;
        }
    }
}