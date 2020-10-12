
namespace UTJ.ProfilerReader.Protocol
{
    // s_ProfilerCategoryInfos
    [CategoryVersion("2017.3","2017.4")]
    internal class Category201703 : ICategory
    {
        // 2018_3
        public string[] GetCategories()
        {
            string[] category = new string[] {
                "Render"    ,
                "Scripts"   ,
                "GUI"       ,
                "Physics"   ,
                "Animation" ,
                "AI"        ,
                "Audio"     ,
                "Video"     ,
                "Particles" ,
                "Gi"        ,
                "Network"   ,
                "Loading"   ,
                "Other"     ,
                "GC"        ,
                "VSync"     ,
                "Overhead"  ,
                "PlayerLoop",
                "Director"  ,
                "VR"        ,
                "NativeMem" ,
                "Internal"  ,
                "FileIO"    ,
                "UI Layout" ,
                "UI Render"
            };
            return category;
        }
    }
}