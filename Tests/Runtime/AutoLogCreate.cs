using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class AutoLogCreate 
{
    [UnityTest]
    public IEnumerator Create10SecLog()
    {
        var waitEndOfFrame = new WaitForEndOfFrame();
        var oneSecWait = new WaitForSeconds(1.0f);

        LogSaveBehaviour.Initialize();
        yield return waitEndOfFrame;
        var prefab = new GameObject("MainCamera");
        prefab.AddComponent<Camera>();
        GameObject.Instantiate(prefab);
        yield return waitEndOfFrame;

        for (int i = 0; i < 10; ++i)
        {
            yield return oneSecWait;
        }
    }
}
