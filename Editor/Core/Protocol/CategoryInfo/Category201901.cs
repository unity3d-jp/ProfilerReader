

namespace UTJ.ProfilerReader.Protocol
{
    // s_ProfilerCategoryInfos
    [CategoryVersion("2019.1")]
    internal class Category201901 : ICategory
    {
        static string[] category = new string[] {
                "Render"           ,
                "Scripts"          ,
                "Managed Jobs"     ,
                "Burst Jobs"       ,
                "GUI"              ,
                "Physics"          ,
                "Animation"        ,
                "AI"               ,
                "Audio"            ,
                "Audio Job"        ,
                "Audio Update Job" ,
                "Video"            ,
                "Particles"        ,
                "Gi"               ,
                "Network"          ,
                "Loading"          ,
                "Other"            ,
                "GC"               ,
                "VSync"            ,
                "Overhead"         ,
                "PlayerLoop"       ,
                "Director"         ,
                "VR"               ,
                "NativeMem"        ,
                "Internal"         ,
                "FileIO"           ,
                "UI Layout"        ,
                "UI Render"        ,
                "VFX"              ,
                "Build Interface"  ,
                "Input"            ,
            };

        // 2019.1
        public string[] GetCategories()
        {
            return category;
        }
        public string GetCategory(int idx)
        {
            if (idx < 0 || idx >= category.Length) { return null; }
            return category[idx];
        }

    }
}