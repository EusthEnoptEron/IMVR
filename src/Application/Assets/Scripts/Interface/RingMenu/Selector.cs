using UnityEngine;
using System.Collections;
using IMVR.Commons;
using Gestures;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;


/// <summary>
/// Helper for items to interact with the selection of the ringmenu
/// </summary>
public abstract class Selector : MonoBehaviour {

    private static RingSubMenu _appendedMenu = null;
    private RingSubMenu _myMenu = null;

    private static Song[] _songs;

    [System.NonSerialized]
    private Toggle _toggle;

    protected virtual void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleChanged);

    }

    protected RingSubMenu Select(IEnumerable<Song> songList)
    {
        if (songList.Count() == 0) return null;

        _songs = songList.ToArray();

        var parentMenu = RingMenu.Instance.EntryPoint;

        // --- optional
        if (_appendedMenu)
        {
            parentMenu = _appendedMenu.transform.parent.GetComponentInParent<IRingMenu>();
            RingMenu.Instance.Remove(_appendedMenu);
        }
        // ---


        string title;
        if (_songs.Count() > 1)
            title = System.String.Format("Album ({0}):\n {1}", _songs.Count(), _songs.First().Album.Name);
        else
            title = System.String.Format("Song:\n {0}", _songs.First().Title);

        // Make new selection
        var songMenu = RingMenuBuilder.CreateMenu(
            FingerType.Pinky,
            _songs.Count() > 1
            ? _songs.First().Album.Name
            : _songs.First().Title,
            ImageAtlas.LoadSprite(_songs.First().Album.Atlas),
            parentMenu
        );

        songMenu.InfoText = title;
        {
            InitItems(songMenu);
        }
        
        parentMenu.UpdateItems();
        _appendedMenu = songMenu;
        _myMenu = songMenu;

        return songMenu;
    }

    protected virtual void InitItems(IRingMenu songMenu)
    {
        var cancelItem = RingMenuBuilder.CreateItem(FingerType.Thumb, "Cancel", songMenu);
        var playItem = RingMenuBuilder.CreateItem(FingerType.Index, "Play", songMenu);
        var enqueueItem = RingMenuBuilder.CreateItem(FingerType.Middle, "Enqueue", songMenu);

        // Set up actions
        cancelItem.OnClick.AddListener(OnCancel);
        playItem.OnClick.AddListener(OnPlay);
        enqueueItem.OnClick.AddListener(OnEnqueue);
    }

    private void OnToggleChanged(bool enabled)
    {
        if (!enabled && _myMenu)
        {
            Debug.Log("REMOVE");
            RingMenu.Instance.Remove(_myMenu);
        }
    }

    protected void OnEnqueue()
    {
        Jukebox.Instance.Playlist.Add(_songs);
    }

    protected void OnPlay()
    {

        Jukebox.Instance.Playlist.Override(_songs);
        Jukebox.Instance.Playlist.MoveForward();
        Jukebox.Instance.Play();

        //_toggle.isOn = false;
        
    }

    protected void OnCancel()
    {
        RingMenu.Instance.GoBack(true);
    }

}
