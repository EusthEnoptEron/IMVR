﻿using UnityEngine;
using System.Collections;
using IMVR.Commons;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Gestures;

[RequireComponent(typeof(Toggle))]
public class PlaylistSongItem : Selector, IPointerEnterHandler, IPointerExitHandler {
    public Song song;
    private Toggle _toggle;
    private Graphic _graphic;

    public Color normalColor = Color.white;
    public Color highlightColor = Color.gray;
    public Color selectedColor = Color.blue;

    //private ArtistView _artistView;

    // Use this for initialization
    void Start()
    {
        if (song != null)
        {
            GetComponentInChildren<Text>().text = String.Format("{0}", song.Title);
            _toggle = GetComponent<Toggle>();
            _graphic = GetComponent<Graphic>();

            _toggle.onValueChanged.AddListener(OnValueChanged);

            _toggle.group = RingMenu.Instance.SelectionGroup;


            name = song.Title + " (Toggle)";
        }
        //_artistView = GetComponentInParent<ArtistView>();
    }

    private void OnValueChanged(bool enabled)
    {
        _graphic.CrossFadeColor(enabled
           ? selectedColor
           : normalColor, 0.1f, false, false);

        if (enabled)
        {
            var menu = Select(new Song[]{ song });
            RingMenu.Instance.Navigate(menu, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_toggle.isOn)
        {
            _graphic.CrossFadeColor(
                normalColor,
                0.1f,
                false,
                false
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_toggle.isOn)
        {
            _graphic.CrossFadeColor(
                highlightColor,
                0.1f,
                false,
                false
            );
        }
    }


    protected override void InitItems(IRingMenu songMenu)
    {
        var cancelItem = RingMenuBuilder.CreateItem(FingerType.Thumb, "Cancel", songMenu);
        var playItem = RingMenuBuilder.CreateItem(FingerType.Index, "Play", songMenu);

        // Set up actions
        cancelItem.OnClick.AddListener(OnCancel);
        playItem.OnClick.AddListener(OnPlaySong);
    }

    private void OnPlaySong()
    {
        Jukebox.Instance.Playlist.Select(song);
        Jukebox.Instance.Play();
    }
}
