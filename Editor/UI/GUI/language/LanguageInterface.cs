using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.ProfilerReader.UI
{
    public abstract class LanguageInterface
    {
        private static LanguageInterface instance;

        protected abstract void CreateDictionary();
    }
}
