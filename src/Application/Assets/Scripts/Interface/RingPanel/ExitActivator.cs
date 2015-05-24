using UnityEngine;
using System.Collections;

public class ExitActivator : RingActivator {


    protected override bool IsStillActive
    {
        get { return true; }
    }

    protected override void Activate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
