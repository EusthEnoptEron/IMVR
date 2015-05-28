using UnityEngine;
using System.Collections;

public class ExploreController : ModeController {

    protected override void Start()
    {
        var view = ChangeView<SelectorView>();
    }
}
