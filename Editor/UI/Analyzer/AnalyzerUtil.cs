using System;
using System.Collections.Generic;


namespace UTJ.ProfilerReader.Analyzer
{
    public class AnalyzerUtil
    {
        public static List<IAnalyzeFileWriter> CreateAnalyzerInterfaceObjects()
        {
            var types = GetInterfaceType<IAnalyzeFileWriter>();
            return CreateInstanciateObjects<IAnalyzeFileWriter>(types);
        }

        private static List<T> CreateInstanciateObjects<T>(List<System.Type> types) where T : class
        {
            List<T> ret = new List<T>();
            foreach (var t in types)
            {
                if (t.IsAbstract) { continue; }
                var inst = Activator.CreateInstance(t) as T;
                ret.Add(inst);
            }
            return ret;
        }

        public static List<System.Type> GetInterfaceType<T>()
        {
            List<System.Type> ret = new List<Type>();
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

    }
}