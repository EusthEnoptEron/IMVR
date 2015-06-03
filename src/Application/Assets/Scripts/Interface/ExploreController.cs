using UnityEngine;
using System.Collections;

public class ExploreController : ModeController {

    protected override void Awake()
    {
        base.Awake();
        Theme = new BlueTheme();
    }

    protected override void Start()
    {
        var view = ChangeView<SelectorView>();
    }
}
