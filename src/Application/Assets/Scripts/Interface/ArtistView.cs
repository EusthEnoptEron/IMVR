using UnityEngine;
using System.Collections;
using IMVR.Commons;

public class ArtistView : View {
    public Artist artist;

    protected override void Awake()
    {
        base.Awake();

    }

    protected void Start()
    {
        if (artist == null)
        {
            Debug.LogError("NO ARTIST GIVEN");
        }
    }
}
