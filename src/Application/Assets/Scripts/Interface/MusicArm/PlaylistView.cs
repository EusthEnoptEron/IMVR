﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using UnityEngine.UI;

public class PlaylistView : ArmWear {
    public Transform itemContainer;
    public GameObject itemTemplate;
    public ScrollRect scrollRect;

    public BufferedList bufferedList;
 
	// Use this for initialization
	void Start () {
        base.Start();

        // Debug
        //Jukebox.Instance.Playlist.Add(
        //    ResourceManager.DB.Songs.Take(5000)
        //);


        // Wire up event handlers
        Jukebox.Instance.Playlist.Change += Playlist_Change;
        Jukebox.Instance.Playlist.IndexChange += Playlist_IndexChange;

        itemContainer.Children().Select(t => t.gameObject).ToList().ForEach(Destroy);

        // Generate playlist to boot
        Regenerate();
	}

    private void Playlist_IndexChange(object sender, EventArgs e)
    {
        UpdateSelection();
    }

    private void Playlist_Change(object sender, EventArgs e)
    {
        Regenerate();
    }

    private void Regenerate()
    {
        Debug.Log("Generate playlist");

        bufferedList.Clear();
        bufferedList.AddItems(
            Jukebox.Instance.Playlist.Songs.Select(song =>
                new BufferedList.LazyGameObject( () => {
                    var item = Instantiate<GameObject>(itemTemplate);
                    var songItem = item.GetComponentInChildren<PlaylistSongItem>();
                    songItem.song = song;
                    return item;
                })
            ).ToList()
        );
        //// Store scroll position
        //float verticalScroll = scrollRect.verticalNormalizedPosition;

        //// Kill all children in a socially sustainable way
        //itemContainer.Children().Select(t => t.gameObject).ToList().ForEach(Destroy);


        //// Make new children with the power of love
        //foreach (var song in Jukebox.Instance.Playlist.Songs)
        //{
        //    var item = Instantiate<GameObject>(itemTemplate);
        //    var songItem = item.GetComponentInChildren<PlaylistSongItem>();
        //    songItem.song = song;

        //    item.transform.SetParent(itemContainer, false);
        //}

        //// Scroll into position
        //scrollRect.verticalNormalizedPosition = verticalScroll;
    }



    private void UpdateSelection()
    {

    }
}
