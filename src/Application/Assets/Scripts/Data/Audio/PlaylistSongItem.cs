using UnityEngine;
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
    private UnityEngine.UI.Image _progressbar;

    private bool _playing = false;

    //private ArtistView _artistView;

    // Use this for initialization
    void Start()
    {
        if (song != null)
        {
            GetComponentInChildren<Text>().text = String.Format("{0}", song.Title);
            _toggle = GetComponent<Toggle>();
            _graphic = GetComponent<Graphic>();
            _progressbar = transform.FindRecursively("Progress").GetComponent<UnityEngine.UI.Image>();

            _toggle.onValueChanged.AddListener(OnValueChanged);

            _toggle.group = RingMenu.Instance.SelectionGroup;


            name = song.Title + " (Toggle)";

            Theme.Change += (s,e) => UpdateColors();
            UpdateColors();
        }
        else
        {
            enabled = false;
        }
        //_artistView = GetComponentInParent<ArtistView>();
    }

    private void UpdateColors()
    {
        _progressbar.color = Theme.Current.SpecialColor;
        _graphic.CrossFadeColor(_toggle.isOn
           ? Theme.Current.ActivatedColor
           : Theme.Current.NormalColor,
        0.1f, false, false);
    }

    private void Update()
    {

        if (Jukebox.Instance.Playlist.Current == song)
        {
            _progressbar.fillAmount = Jukebox.Instance.Progress;
        }
        else
        {
            _progressbar.fillAmount = 0;
        }
    }

    private void OnValueChanged(bool enabled)
    {
        _graphic.CrossFadeColor(enabled
           ? Theme.Current.ActivatedColor
           : Theme.Current.NormalColor, 
        0.1f, false, false);

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
                Theme.Current.NormalColor,
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
                Theme.Current.HighlightedColor,
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
