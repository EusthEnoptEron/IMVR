using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ListenView : View {

    public SongSelection selection;

	// Use this for initialization
	void Start () {
        if (selection == null)
            Debug.LogError("NO SELECTION MADE!");

        // Add songs to playlist.
        Jukebox.Instance.Playlist.Override(selection.Songs);
        Jukebox.Instance.Playlist.MoveForward();

        // Play.
        Jukebox.Instance.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
