﻿using UnityEngine;
using IMVR.Commons;
using UnityEngine.UI;
using System.Linq;
using Gestures;

public class ArtistView : View {
    public Artist artist;

    private GameObject m_artistView;
    private Transform m_albumList;
    private Song[] _selectedSongs = new Song[0];

    public MusicSelection selector;

    // UI Prefabs
    private static GameObject pref_albumView = Resources.Load<GameObject>("Prefabs/UI/pref_AlbumView");
    private static GameObject pref_songItem = Resources.Load<GameObject>("Prefabs/UI/pref_SongItem");
    private static GameObject pref_artistInfo = Resources.Load<GameObject>("Prefabs/UI/pref_ArtistInfo");
    private static GameObject pref_selector = Resources.Load<GameObject>("Prefabs/UI/pref_Selector");
    private static GameObject pref_biography = Resources.Load<GameObject>("Prefabs/UI/pref_Biography");

    // 3D prefabs
    private static GameObject pref_Earth    = Resources.Load<GameObject>("Prefabs/Objects/pref_Earth");
    private static GameObject pref_songChart = Resources.Load<GameObject>("Prefabs/Objects/pref_SongChart");

    private IRingMenu _artistMenu;

    private CanvasCircleLayout cylinder;
    protected override void Awake()
    {
        base.Awake();

        // Move down by 50cm
        transform.localPosition += Vector3.down * 0.5f;

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        gameObject.AddComponent<GraphicRaycaster>();


        // BUILD CYLINDER
        cylinder = new GameObject().AddComponent<CanvasCircleLayout>();
        cylinder.transform.SetParent(transform, false);
        cylinder.radius = 0.5f;
        cylinder.height = 1;
        cylinder.scale = 1 / 800f;
        cylinder.Resize(10, 1);

        cylinder.gameObject.AddComponent<CylinderInteractor>();

    }

    protected void Start()
    {
        if (artist == null)
        {
            Debug.LogError("NO ARTIST GIVEN");
        }
        else
        {

            // BUILD BIOGHRAPHY
            if (artist.Biography != null)
            {
                var textObj = GameObject.Instantiate<GameObject>(pref_biography);
                var text = textObj.GetComponentInChildren<Text>();

                textObj.transform.SetParent(transform, false);
                textObj.transform.localScale /= 800f;
                textObj.transform.localPosition = Vector3.forward + Vector3.up;
                //text.rectTransform.sizeDelta = new Vector2(800, 800);
                if (artist.Biography.Length > 1000)
                    text.text = artist.Biography.Substring(0, 1000) + "...";
                else
                    text.text = artist.Biography;
                //text.resizeTextForBestFit = true;
                //text.font = ResourceManager.Arial;
            }


            //m_artistView = GameObject.Instantiate<GameObject>(
            //    Resources.Load<GameObject>("Prefabs/UI/pref_ArtistView")
            //);

            //m_artistView.transform.SetParent(transform, false);
            //m_artistView.transform.localScale = Vector3.one / 640f;
            //m_artistView.transform.localPosition = Camera.main.transform.forward;
            //m_artistView.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

            //m_albumList = m_artistView.transform.FindRecursively("AlbumList");

            selector = GameObject.Instantiate<GameObject>(pref_selector).GetComponent<MusicSelection>();
            selector.gameObject.SetActive(false);

            int i = 0;

            cylinder.SetTile(9, 0, InitGeneralInfo());
            cylinder.SetTile(i++, 0, InitEarth());

            // Build song chart
            var chart = GameObject.Instantiate<GameObject>(pref_songChart).GetComponent<SongMetaChart>();
            cylinder.SetTile(8, 0, chart.gameObject);
            chart.transform.localScale /= (cylinder.scale * 2);
            chart.transform.localPosition += Vector3.up / (cylinder.scale * 3);
            chart.SetSongs(artist.Albums.SelectMany(album => album.Tracks));


            foreach (var album in artist.Albums)
            {
                cylinder.SetTile(i++, 0, InitAlbum(album));
            }
        }
    }

    private GameObject InitGeneralInfo()
    {
        var info = GameObject.Instantiate<GameObject>(pref_artistInfo);

        var picture = info.transform.FindChild("Picture").GetComponent<UnityEngine.UI.Image>();
        var nameObj = info.transform.FindChild("Name").GetComponent<Text>();

        var ticket = artist.Pictures.FirstOrDefault();
        if (ticket != null)
            picture.sprite = ImageAtlas.LoadSprite(ticket);

        nameObj.text = artist.Name;


        return info;
    }

    private GameObject InitEarth()
    {
        var earth = GameObject.Instantiate<GameObject>(pref_Earth);
        var marker = earth.GetComponentInChildren<GalleryVR.Dbg.EarthPlacer>();
        
        marker.transform.localScale *= 15;
        marker.transform.localPosition += Vector3.up * 3 * Tile.PIXELS_PER_UNIT;

        marker.latitude = artist.Coordinate.Latitude;
        marker.longitude = artist.Coordinate.Longitude;

        Debug.Log(artist.Coordinate);
        return earth;
    }

    private GameObject InitAlbum(Album album)
    {
        var albumView = GameObject.Instantiate<GameObject>(pref_albumView);
        //albumView.transform.SetParent(m_albumList, false);
        
        // Add cover
        var cover = albumView.transform.FindChild("Cover").GetComponent<UnityEngine.UI.Image>();
        cover.sprite = ImageAtlas.LoadSprite(album.Atlas);

        var albumItem = cover.gameObject.AddComponent<AlbumItem>();
        {
            albumItem.album = album;
            albumItem.Touched += OnSelectAlbum;
        }

        var songList = albumView.transform.FindChild("Songlist");

        // Add songs
        foreach (var song in album.Tracks)
        {
            var songItem = GameObject.Instantiate<GameObject>(pref_songItem).GetComponent<SongItem>();
            
            songItem.transform.SetParent(songList, false);
            songItem.song = song;
            songItem.Touched += OnSelectSong;
        }

        return albumView;
    }
    


    #region Event Handlers


    /// <summary>
    /// Deselects whatever is selected
    /// </summary>
    private void OnSelectionChanged()
    {
        // Make sure the old selection is removed
        RingMenuItem item;
        if (_artistMenu.Items.TryGetValue(FingerType.Pinky, out item))
        {
            RingMenu.Instance.Remove(item);
        }

        // Make new selection
        var songMenu = RingMenuBuilder.CreateMenu(
            FingerType.Pinky,
            _selectedSongs.Length > 1
            ? _selectedSongs[0].Album.Name
            : _selectedSongs[0].Title,
            _artistMenu
        );
        {
            var cancelItem = RingMenuBuilder.CreateItem(FingerType.Thumb, "Cancel", songMenu);
            var playItem = RingMenuBuilder.CreateItem(FingerType.Index, "Play", songMenu);
            var enqueueItem = RingMenuBuilder.CreateItem(FingerType.Middle, "Enqueue", songMenu);

            // Set up actions
            cancelItem.OnClick.AddListener(OnCancelSongs);
            playItem.OnClick.AddListener(OnPlaySongs);
            enqueueItem.OnClick.AddListener(OnEnqueueSongs);
        }
        _artistMenu.UpdateItems();

        RingMenu.Instance.Navigate(songMenu, true);
    }

    private void OnEnqueueSongs()
    {
        Jukebox.Instance.Playlist.Add(_selectedSongs);
    }

    private void OnPlaySongs()
    {
        Jukebox.Instance.Playlist.Override(_selectedSongs);
        Jukebox.Instance.Playlist.MoveForward();
    }

    private void OnCancelSongs()
    {
        RingMenu.Instance.Navigate(_artistMenu, true);
    }

    private void OnSelectSong(object sender, SongEventArgs e)
    {
        _selectedSongs = new Song[] { e.Song };

        OnSelectionChanged();
    }

    private void OnSelectAlbum(object sender, AlbumEventArgs e)
    {
        _selectedSongs = e.Album.Tracks.ToArray();

        OnSelectionChanged();
    }


    public override void BuildMenu(RingMenu menuBase)
    {
        _artistMenu = RingMenuBuilder.CreateMenu(FingerType.Pinky, artist.Name, menuBase);
        {
            var cancelItem  = RingMenuBuilder.CreateItem(FingerType.Thumb, "Cancel", _artistMenu);
            var playItem    = RingMenuBuilder.CreateItem(FingerType.Index, "Play", _artistMenu);
            var enqueueItem = RingMenuBuilder.CreateItem(FingerType.Middle, "Enqueue", _artistMenu);
        
            // Set up actions
            cancelItem.OnClick.AddListener(OnCancel);
            playItem.OnClick.AddListener(OnPlay);
            enqueueItem.OnClick.AddListener(OnEnqueue);
        }

        RingMenu.Instance.Navigate(_artistMenu, true);
    }

    private void OnEnqueue()
    {
        // Add all songs to playlist
        Jukebox.Instance.Playlist.Add(
            artist.Albums.SelectMany(album => album.Tracks)    
        );

    }

    private void OnPlay()
    {
        // Override all items in playlist...
        Jukebox.Instance.Playlist.Override(
            artist.Albums.SelectMany(album => album.Tracks)
        );

        // Go ahead and play the new playlist!
        Jukebox.Instance.Playlist.MoveForward();
    }

    private void OnCancel()
    {
        RingMenu.Instance.GoBack();
    }

    #endregion

}
