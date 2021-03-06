﻿using UnityEngine;
using System.Collections;
using IMVR.Commons;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Different groups of meta data given to a song.
/// </summary>
public enum MetaGroup
{
    Danceability = 0,
    Speechiness = 1,
    Liveness = 2,
    Energy = 3,
    Instrumentalness = 4,
    Tempo = 5,
    Valence = 6,
    Acousticness = 7
}
public static class MetaGroupExtensions
{
    public static float? GetValue(this MetaGroup group, Song song)
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
            case MetaGroup.Tempo:
                return song.Tempo;
            case MetaGroup.Valence:
                return song.Valence;
            case MetaGroup.Acousticness:
                return song.Acousticness;
            default:
                return null;
        }
    }

    public static bool HasValue(this MetaGroup group, Song song)
    {
        return group.GetValue(song) != null;
    }
}

public class MetaGroupView : FlatGroupView {
    public MetaGroup group = MetaGroup.Danceability;

    protected override void Awake()
    {
        distance = 0.5f;
        height = -.3f;

        gameObject.AddComponent<CanvasGroup>();

        base.Awake();
    }
    // Use this for initialization
	protected override void Start () {

        titleElement.text = group.ToString();

        var itemList = ResourceManager.DB.Songs
                    .Where(HasNeededGroup);
        
        int total = itemList.Count();

        var groupedItems = itemList
                    .GroupBy(song => (int)(Mathf.Floor(GetNeededValue(song).Value * 10) * 10))
                    .ToDictionary(grouping => grouping.Key, grouping => grouping);


        Debug.Log(string.Join(", ", groupedItems.Keys.Select(k => k.ToString()).ToArray()));
        var dictionary = new Dictionary<string, List<GameObject>>();

        for (int i = 0; i < 100; i+= 10)
        {
            var go = new GameObject();
            dictionary[string.Format("{0}..{1}", i, i + 9)] =
                new List<GameObject>() {
                    go
                };

            var tile = go.AddComponent<SongStack>();
            tile.items = groupedItems.ContainsKey(i) ? groupedItems[i].AsEnumerable() : new Song[0];
            tile.totalCount = total;
        }

        items = dictionary;

        base.Start();

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
            case MetaGroup.Tempo:
                return song.Tempo;
        }
        return null;
    }
	
	// Update is called once per frame
	void Update () {

	}
}
