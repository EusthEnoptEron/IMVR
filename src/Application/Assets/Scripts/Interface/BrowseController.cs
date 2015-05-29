using UnityEngine;
using System.Collections;

public class BrowseController : ModeController {
    protected override void Start()
    {
        StartCoroutine(StartDelayed());
    }

    private IEnumerator StartDelayed()
    {
        yield return new WaitForSeconds(1);

        LoadingScreen.Instance.enabled = true;

        yield return null;

        var view = ChangeView<ArtistOverView>();
        //view.artist = ResourceManager.DB.Artists[0];
    }
}