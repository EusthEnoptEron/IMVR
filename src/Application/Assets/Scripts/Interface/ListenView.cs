using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using LinqToTwitter;
using System.Security.Cryptography.X509Certificates;
using Foundation;
using IMVR.Commons;

public class ListenView : View {
    class NoCheckCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }

    public SongSelection selection;
    private TwitterContext _twitter;

    private List<Status> _tweets = new List<Status>();

    // Back buffer
    private List<Status> _tweetsBuffer = new List<Status>();
    private bool _isFetching;
    
	// Use this for initialization
	void Start () {
        ServicePointManager.CertificatePolicy = new NoCheckCertificatePolicy();

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

    protected override void OnViewEnable()
    {
        BeatDetector.Instance.Beat += OnBeat;
        Jukebox.Instance.Playlist.IndexChange += OnSongChange;
        _twitter = new TwitterContext(new SingleUserAuthorizer()
        {
            Credentials = new SingleUserInMemoryCredentials()
            {
                ConsumerKey = "rIuWk230ArpHcMe94CImgC9Ze",
                ConsumerSecret = "h3SilJlfoi8lfSR7nHuJkW1zZ8qlc0Jphd1sw0P2KULc3uZQrR",
                TwitterAccessToken = "568595062-JhjU5vNZON8mP0jV9n4ogkNp833RgZmq8CPT7YDX",
                TwitterAccessTokenSecret = "nRA2EaLWEIASvd6T1eGIkVCkoqjAYAuE7gDCXe0Ew679D",
            }
        });
    }

    private void OnSongChange(object sender, System.EventArgs e)
    {
        var song = Jukebox.Instance.Playlist.Current;
        if(song.Artist.TwitterHandle != null)
            StartCoroutine(FetchTweets(Jukebox.Instance.Playlist.Current));
    }

    private void OnBeat(object sender, BeatEventArgs e)
    {
        // Spawn a twitter message!
        
    }

    protected override void OnViewDisable()
    {
        // Unregister event handler because it ain't needed anymore... for now.
        _twitter.Dispose();
        Jukebox.Instance.Playlist.IndexChange -= OnSongChange;
        BeatDetector.Instance.Beat -= OnBeat;
    }

    private IEnumerator FetchTweets(Song song)
    {
        if (_isFetching) yield break;
        _isFetching = true;

        var task = Task.Run(delegate
        {
            _tweetsBuffer = _twitter.Search.Where(
             status =>
                     status.Query == string.Format("from:{0}", song.Artist.TwitterHandle)
                     && status.Type == SearchType.Search
             ).First().Statuses.ToList();
        });
        yield return StartCoroutine(task.WaitRoutine());
        
        // Swap!
        var temp = _tweets;
        _tweets = _tweetsBuffer;
        _tweetsBuffer = temp;

        _isFetching = false;
        yield break;
    }
}
