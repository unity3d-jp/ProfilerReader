using System.Collections.Generic;
using UTJ.ProfilerReader.Protocol;

namespace UTJ.ProfilerReader
{
    // s_ProfilerCategoryInfos
    public class ProtocolData
    {
        static Dictionary<string, string[]> cacheData;
        public static string GetCategory( string unityVersion,int categoryId)
        {
            string[] categories = null;
            if( cacheData == null) { cacheData = new Dictionary<string, string[]>(); }
            if(!cacheData.TryGetValue(unityVersion,out categories)){
                categories = GetCategories(unityVersion);
                cacheData.Add(unityVersion, categories);
            }
            if( categories == null || categoryId < 0 || categoryId >= categories.Length)
            {
                return "";
            }
            return categories[categoryId];
        }
        public static string[] GetCategories(string version)
        {
            var obj = CategoryVersionAttribute.GetMatchObject(version);
            if( obj == null)
            {
                return null;
            }
            return obj.GetCategories();
        }

        public static HashSet<string> GetEngineCounter(string unityVersion)
        {
            return new HashSet<string>();
        }
    }
}