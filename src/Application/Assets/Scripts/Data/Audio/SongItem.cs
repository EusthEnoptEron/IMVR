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

[RequireComponent(typeof(Text))]
public class SongItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
    public Song song;
    private ArtistView _artistView;

    public event EventHandler<SongEventArgs> Touched = delegate { };

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = String.Format("{0:00}. {1}", song.TrackNo, song.Title);
        _artistView = GetComponentInParent<ArtistView>();
	}


    public void OnPointerClick(PointerEventData eventData)
    {
      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Touched(this, new SongEventArgs(this));

        //_artistView.selector.transform.SetParent(transform, false);

        //_artistView.selector.songs.Clear();
        //_artistView.selector.songs.Add(song);

        //_artistView.selector.gameObject.SetActive(true);
        //Jukebox.Instance.Play(song);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
