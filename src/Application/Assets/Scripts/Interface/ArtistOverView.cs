using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IMVR.Commons;
using Foundation;
using System.Linq;

public class ArtistOverView : View {
    private GroupedCircleLayout layout;
    private CylinderInteractor interactor;
    protected override void Awake()
    {
        base.Awake();

        layout = gameObject.AddComponent<GroupedCircleLayout>();
        interactor = layout.gameObject.AddComponent<CylinderRaycaster>()
                           .gameObject.AddComponent<CylinderInteractor>();

        layout.height = 1;
        layout.radius = 0.5f;
    }

    IEnumerator Start()
    {
        IMDB db = null;
        var path = Prefs.Instance.DBPath;
        var task = Task.Run(delegate
        {
            db = IMDB.FromFile(path);
        });

        yield return StartCoroutine(task.WaitRoutine());

        //Debug.Log(db.Artists.Count);
        var groups = db.Artists.GroupBy(o =>
        {
            string firstLetter = o.Name.Substring(0, 1).ToUpper();
            if (firstLetter[0] > 1000) firstLetter = "#";
            return firstLetter;
        })
        .ToDictionary(g => g.Key, g => g.Select(artist =>
        {
            var tile = new GameObject().AddComponent<ArtistTile>();
            tile.SetArtist(artist);
            return tile.gameObject;
        }).ToList());

        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (!groups.ContainsKey(c.ToString())) groups.Add(c.ToString(), new List<GameObject>());
        }

        layout.Items = groups;
    }

}
