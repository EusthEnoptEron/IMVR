using UnityEngine;
using System.Collections;

public class ModeActivator : RingActivator {
    public ModeController controller;

    protected override bool IsStillActive
    {
        get { return RootPanel.Instance.Controller == controller; }
    }

    protected override void Activate()
    {
        RootPanel.Instance.Controller = controller;
    }
}
