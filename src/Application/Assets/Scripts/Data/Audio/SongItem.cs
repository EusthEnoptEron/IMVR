using UnityEngine;
using IMVR.Commons;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class SongEventArgs : EventArgs
{
    public Song Song { get; private set; }
    public SongItem Item { get; private set; }

    public SongEventArgs(SongItem item)
    {
        Song = item.song;
        Item = item;
    }
}


[RequireComponent(typeof(Toggle))]
public class SongItem : Selector, IPointerEnterHandler, IPointerExitHandler {
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
            GetComponentInChildren<Text>().text = String.Format("{0:00}. {1}", song.TrackNo, song.Title);

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
            var menu = Select(new Song[] { song });
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
}