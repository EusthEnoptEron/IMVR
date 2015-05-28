using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;

public class ModeActivator : RingActivator {
    public ModeController controller;
    private RingMesh _floor;

    protected void Start()
    {
        _floor = transform.Siblings().First(s => s.name == "Floor").GetComponent<RingMesh>();
    }

    protected override bool IsStillActive
    {
        get { return RootPanel.Instance.Controller == controller; }
    }

    protected override void Activate()
    {
        RootPanel.Instance.Controller = controller;

        DOTween.To(
            () => _floor.Color,
            (x) => _floor.Color = x,
            _model.color,
            0.5f
        );

    }
}
