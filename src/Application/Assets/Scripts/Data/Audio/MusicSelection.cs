using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IMVR.Commons;

public class MusicSelection : MonoBehaviour {

    public List<Song> songs = new List<Song>();

    public void Play()
    {
        Jukebox.Instance.Playlist.Override(songs);
        Jukebox.Instance.Playlist.MoveForward();
    }

    public void Enqueue()
    {
        Jukebox.Instance.Playlist.Add(songs);
    }
}
