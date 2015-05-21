using UnityEngine;
using System.Collections;

public class BrowseController : ModeController {
    protected override void Start()
    {
        LoadingScreen.Instance.enabled = true;
        ChangeView<ArtistOverView>();
    }
}
