using UnityEngine;
using IMVR.Commons;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class AlbumItem : MonoBehaviour, IPointerDownHandler
{
    public Album album;
    private ArtistView _artistView;

    void Start()
    {
        _artistView = GetComponentInParent<ArtistView>();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        _artistView.selector.transform.SetParent(transform, false);

        _artistView.selector.songs.Clear();
        _artistView.selector.songs.AddRange(album.Tracks);

        _artistView.selector.gameObject.SetActive(true);
        //Jukebox.Instance.Play(song);
    }
}
