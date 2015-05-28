using UnityEngine;
using System.Collections;

public class BrowseController : ModeController {
    protected override void Start()
    {
        LoadingScreen.Instance.enabled = true;
        var view = ChangeView<ArtistOverView>();
        //view.artist = ResourceManager.DB.Artists[0];
       
    }
}
