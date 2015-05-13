using UnityEngine;
using System.Collections;
using IMVR.Commons;
using System.Linq;
using Foundation;
using System.Collections.Generic;


[RequireComponent(typeof(ListGroupedCircleLayout))]
public class ArtistFeeder : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        var layout = GetComponent < ListGroupedCircleLayout>();
        IMDB db = null;
        var path = Prefs.Instance.DBPath;
        var task = Task.Run(delegate
        {
            db = IMDB.FromFile(path);
        });

        yield return StartCoroutine(task.WaitRoutine());

        //Debug.Log(db.Artists.Count);
        var groups = db.Artists.GroupBy(o => {
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
