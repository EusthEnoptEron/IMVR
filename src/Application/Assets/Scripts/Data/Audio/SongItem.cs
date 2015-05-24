using UnityEngine;
using IMVR.Commons;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


[RequireComponent(typeof(Text))]
public class SongItem : MonoBehaviour, IPointerDownHandler {
    public Song song;
    private ArtistView _artistView;

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
        _artistView.selector.transform.SetParent(transform, false);

        _artistView.selector.songs.Clear();
        _artistView.selector.songs.Add(song);

        _artistView.selector.gameObject.SetActive(true);
        //Jukebox.Instance.Play(song);
    }
}
