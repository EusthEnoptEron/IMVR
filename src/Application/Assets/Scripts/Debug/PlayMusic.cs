using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

public class PlayMusic : MonoBehaviour {
    public int count = 20;

    public void Start()
    {

        Jukebox.Instance.Playlist.Add(ResourceManager.DB.Songs.Take(count));
        Jukebox.Instance.Play();
    }
}
