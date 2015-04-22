﻿using UnityEngine;
using System.Collections;
using IMVR.Commons;
using UnityEngine.UI;

public class ArtistView : View {
    public Artist artist;

    private GameObject m_artistView;
    private Transform m_albumList;

    private GameObject pref_albumView;
    private GameObject pref_songItem;
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
        else
        {
            m_artistView = GameObject.Instantiate<GameObject>(
                Resources.Load<GameObject>("Prefabs/pref_ArtistView")
            );

            m_artistView.transform.SetParent(transform, false);
            m_artistView.transform.localScale = Vector3.one / 640f;
            m_artistView.transform.localPosition = Camera.main.transform.forward;
            m_artistView.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

            m_albumList = m_artistView.transform.FindRecursively("AlbumList");

            pref_albumView = Resources.Load<GameObject>("Prefabs/pref_AlbumView");
            pref_songItem = Resources.Load<GameObject>("Prefabs/pref_SongItem");

            foreach (var album in artist.Albums)
            {
                InitAlbum(album);
            }

        }
    }

    private void InitAlbum(Album album)
    {
        var albumView = GameObject.Instantiate<GameObject>(pref_albumView);
        albumView.transform.SetParent(m_albumList, false);
        
        // Add cover
        var cover = albumView.transform.FindChild("Cover").GetComponent<UnityEngine.UI.Image>();
        cover.sprite = ImageAtlas.LoadSprite(album.Atlas);

        var songList = albumView.transform.FindChild("Songlist");

        // Add songs
        foreach (var song in album.Tracks)
        {
            var songItem = GameObject.Instantiate<GameObject>(pref_songItem).GetComponent<SongItem>();
            
            songItem.transform.SetParent(songList, false);
            songItem.song = song;
        }
    }
}
