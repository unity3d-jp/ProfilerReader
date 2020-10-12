using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.ProfilerReader
{
    public class UtjProfilerInitializer 
    {
        [UnityEditor.InitializeOnLoadMethod]
        public static void Init()
        {
            ProfilerLogUtil.logErrorException = (e) => {
            //    Debug.LogError(e);
            };
            ProfilerLogUtil.logErrorString = (str) => {
                //    Debug.LogError(str);
            };
        }
    }
}
