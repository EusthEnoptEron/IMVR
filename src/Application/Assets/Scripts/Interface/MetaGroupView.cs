using UnityEngine;
using System.Collections;
using IMVR.Commons;
using System.Linq;
using System.Collections.Generic;

public enum MetaGroup
{
    Danceability,
    Speechiness,
    Liveness,
    Energy,
    Instrumentalness
}
public class MetaGroupView : View {
    public MetaGroup group = MetaGroup.Danceability;

    private SimpleGroupedCircleLayout layout;
    private CylinderInteractor interactor;


    // Use this for initialization
	void Start () {

        layout = gameObject.AddComponent<SimpleGroupedCircleLayout>();
        interactor = layout.gameObject.AddComponent<CylinderRaycaster>()
                           .gameObject.AddComponent<CylinderInteractor>();

        layout.height = 1;
        layout.radius = 0.5f;

        var itemList = ResourceManager.DB.Songs
                    .Where(HasNeededGroup);
        
        int total = itemList.Count();

        var items = itemList
                    .GroupBy(song => (int)(Mathf.Floor(GetNeededValue(song).Value * 10) * 10))
                    .ToDictionary(grouping => grouping.Key, grouping => grouping);


        Debug.Log(string.Join(", ", items.Keys.Select(k => k.ToString()).ToArray()));
        var dictionary = new Dictionary<string, List<GameObject>>();

        for (int i = 0; i < 100; i+= 10)
        {
            var go = new GameObject();
            dictionary[string.Format("{0}..{1}", i, i + 9)] =
                new List<GameObject>() {
                    go
                };

            var tile = go.AddComponent<MetaGroupTile>();
            tile.items = items.ContainsKey(i) ? items[i].AsEnumerable() : new Song[0];
            tile.totalCount = total;
        }

        layout.Items = dictionary;
	}

    private bool HasNeededGroup(Song song)
    {
        return GetNeededValue(song) != null;
    }

    private float? GetNeededValue(Song song)
    {
        switch (group)
        {
            case MetaGroup.Danceability:
                return song.Danceability;
            case MetaGroup.Energy:
                return song.Energy;
            case MetaGroup.Instrumentalness:
                return song.Instrumentalness;
            case MetaGroup.Liveness:
                return song.Liveness;
            case MetaGroup.Speechiness:
                return song.Speechiness;
        }
        return null;
    }
	
	// Update is called once per frame
	void Update () {

	}
}
