using UnityEngine;
using System.Collections;
using IMVR.Commons;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


[RequireComponent(typeof(Text))]
public class SongItem : MonoBehaviour, IPointerClickHandler {
    public Song song;

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = String.Format("{0:00}. {1}", song.TrackNo, song.Title);
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        Jukebox.Instance.Play(song);
    }
}
