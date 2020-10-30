using System.Collections.Generic;

namespace UTJ.ProfilerReader.Protocol
{
    internal class CategoryVersionAttribute : System.Attribute
    {
        public string[] unityVersions;
        public CategoryVersionAttribute(params string[] versions)
        {
            unityVersions = versions;
        }

        private bool IsMatchVersion( string version)
        {
            foreach ( var unityVersion in unityVersions)
            {
                if (version.StartsWith(unityVersion))
                {
                    return true;
                }
            }
            return false;
        }

        private static List<System.Type> GetInterfaceType<T>()
        {
            List<System.Type> ret = new List<System.Type>();
            var domain = System.AppDomain.CurrentDomain;
            var assemblies = domain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces == null)
                    {
                        continue;
                    }
                    foreach (var interfacetype in interfaces)
                    {
                        if (interfacetype == typeof(T) && !type.IsAbstract)
                        {
                            ret.Add(type);
                        }
                    }
                }
            }
            return ret;
        }


        public static ICategory GetMatchObject( string version)
        {
            ICategory obj = null;
            var type = GetMatchVersionType(version);
            if( type != null)
            {
                obj = System.Activator.CreateInstance(type) as ICategory;
            }
            return obj;
        }

        private static System.Type GetMatchVersionType(string version)
        {
            var types = GetInterfaceType<ICategory>();
            foreach( var type in types)
            {
                var attributes = type.GetCustomAttributes(typeof(CategoryVersionAttribute),false);
                foreach( var attr in attributes)
                {
                    CategoryVersionAttribute categoryVersion = attr as CategoryVersionAttribute;
                    if( categoryVersion.IsMatchVersion(version))
                    {
                        return type;
                    }
                }
            }
            return null;
        }
    }
}