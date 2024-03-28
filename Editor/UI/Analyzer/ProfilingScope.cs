using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

namespace UTJ.ProfilerReader.Analyzer
{
    struct ProfilingScope : System.IDisposable
    {
        CustomSampler _sampler;
        public ProfilingScope(CustomSampler sampler)
        {
            _sampler = sampler;
            _sampler.Begin();
        }
        public void Dispose()
        {
            _sampler.End();
        }
    }
}