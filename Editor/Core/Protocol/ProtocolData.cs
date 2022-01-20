using System.Collections.Generic;
using UTJ.ProfilerReader.Protocol;
using UTJ.ProfilerReader.BinaryData;
using UTJ.ProfilerReader.BinaryData.Stats;

namespace UTJ.ProfilerReader
{
    // s_ProfilerCategoryInfos
    public class ProtocolData
    {
        private static string cacheVersion;
        private static Dictionary<uint, Category> cacheDictionary;

        public static Dictionary<uint,Category> GetCategories(ProfilerFrameData frame, string version)
        {
            var category = frame.CategoryInfo;
            if(category != null && category.Count>0 ) {
                return category;
            }

            if (cacheVersion == version)
            {
                return cacheDictionary;
            }
            var obj = CategoryVersionAttribute.GetMatchObject(version);
            if( obj == null)
            {
                cacheDictionary = null;
                cacheVersion = version;
                return null;
            }
            cacheDictionary = CreateCategoryDictionary(obj.GetCategories());
            cacheVersion = version;

            return cacheDictionary;
//            return obj.GetCategories();
        }
        public static string GetCategory(ProfilerFrameData frame, string version, int idx)
        {
            var dict = GetCategories(frame, version);
            Category category = null;
            if (dict.TryGetValue((uint)idx, out category))
            {
                return category.name;
            }
            return "";
        }

        private static Dictionary<uint, Category> CreateCategoryDictionary(string[] builtInCategory)
        {
            var dict = new Dictionary<uint, Category>();
            for (int i = 0; i < builtInCategory.Length; ++i)
            {
                Category category = new Category()
                {
                    categoryId = (uint)i,
                    name = builtInCategory[i]
                };
                dict.Add((uint)i,category);
            }
            return dict;
        }

        public static HashSet<string> GetEngineCounter(string unityVersion)
        {
            return new HashSet<string>();
        }
    }
}