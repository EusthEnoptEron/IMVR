using UnityEngine;
using System.Collections;

public class BrowseMode : Mode {

    private ArtistOverView _overview;
    protected override void Awake()
    {
        base.Awake();
        Theme = new GreenTheme();
    }

    protected override void Start()
    {
        StartCoroutine(StartDelayed());
    }

    private IEnumerator StartDelayed()
    {
        yield return new WaitForSeconds(1);

        //LoadingScreen.Instance.enabled = true;

        yield return null;

        var view = ChangeView<ArtistOverView>();
        //view.artist = ResourceManager.DB.Artists[0];
    }
}