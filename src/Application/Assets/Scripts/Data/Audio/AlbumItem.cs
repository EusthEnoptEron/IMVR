using UnityEngine;
using IMVR.Commons;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class AlbumEventArgs : EventArgs
{
    public Album Album { get; private set; }
    public AlbumItem Item { get; private set; }

    public AlbumEventArgs(AlbumItem item)
    {
        Album = item.album;
        Item = item;
    }
}

public class AlbumItem : MonoBehaviour, IPointerDownHandler
{
    public Album album;
    private ArtistView _artistView;

    [SerializeField]
    private UnityEngine.UI.Image _coverImage;

    [SerializeField]
    private Text _coverText;

    public event EventHandler<AlbumEventArgs> Touched = delegate { };

    void Start()
    {
        _artistView = GetComponentInParent<ArtistView>();

        _coverImage.sprite = ImageAtlas.LoadSprite(album.Atlas);
        _coverText.text = album.Name;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Touched(this, new AlbumEventArgs(this));

        //_artistView.selector.transform.SetParent(transform, false);

        //_artistView.selector.songs.Clear();
        //_artistView.selector.songs.AddRange(album.Tracks);

        //_artistView.selector.gameObject.SetActive(true);

    }
}
