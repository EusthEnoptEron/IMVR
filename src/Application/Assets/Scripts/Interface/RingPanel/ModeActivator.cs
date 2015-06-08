using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;

public class ModeActivator : RingActivator {
    public Mode controller;
    private RingMesh _floor;
    private RingMesh _ring;

    protected void Start()
    {
        _floor = transform.Siblings().First(s => s.name == "Floor").GetComponent<RingMesh>();
        _ring = GetComponent<RingMesh>();

        _ring.Color = controller.Theme.BaseColor;
    }

    protected override bool IsStillActive
    {
        get { return ModeController.Instance.Controller == controller; }
    }

    protected override void Activate()
    {
        ModeController.Instance.Controller = controller;

        DOTween.To(
            () => _floor.Color,
            (x) => _floor.Color = x,
            _model.color,
            0.5f
        );

    }
}
