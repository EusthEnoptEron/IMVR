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
    public Vector3 placementRange = new Vector3(3f,2f,5f);

    private TwitterContext _twitter;
    private GameObject _tweetPrefab;

    private List<Status> _tweets = new List<Status>();

    // Back buffer
    private List<Status> _tweetsBuffer = new List<Status>();
    private bool _isFetching;
    private Artist _currentArtist;

    private int _tweetCounter = 0;

    // Use this for initialization
    void Start()
    {
        ServicePointManager.CertificatePolicy = new NoCheckCertificatePolicy();

        if (selection == null)
            Debug.LogError("NO SELECTION MADE!");

        // Add songs to playlist.
        Jukebox.Instance.Playlist.Override(selection.Songs);
        Jukebox.Instance.Playlist.MoveForward();

        // Play.
        Jukebox.Instance.Play();

        _tweetPrefab = Resources.Load<GameObject>("Prefabs/UI/pref_Tweet");
    }
	
    //// Update is called once per frame
    //void Update () {
	
    //}

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

        OnSongChange(this, new System.EventArgs());
    }

    private void OnSongChange(object sender, System.EventArgs e)
    {
        var song = Jukebox.Instance.Playlist.Current;
        Debug.Log("ARTIST TWITTER: " + song.Artist.TwitterHandle);
        //if (song.Artist.TwitterHandle != null)
            StartCoroutine(FetchTweets(Jukebox.Instance.Playlist.Current));
    }

    private void OnBeat(object sender, BeatEventArgs e)
    {
        // Spawn a twitter message!
        if (_tweets.Count > 0)
        {
            int id = _tweetCounter++ % _tweets.Count;

            var tweet = Instantiate<GameObject>(_tweetPrefab).GetComponent<TweetView>();
            tweet.tweet = _tweets[id];


            var position = Vector3.Scale(Random.insideUnitSphere, placementRange);
            if (Mathf.Abs(position.x) < 0.3f) position.x = 0.3f;
            if (Mathf.Abs(position.z) < 0.3f) position.z = 0.3f;

            tweet.transform.position = Camera.main.transform.position + 
                position;

        }
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
        Debug.Log("Fetch");
        if (_isFetching || _currentArtist == song.Artist) yield break;
        _isFetching = true;

        var task = Task.Run(delegate
        {
            _tweetsBuffer.Clear();

            if (song.Artist.TwitterHandle != null && song.Artist.TwitterHandle.Length > 0)
            {
                _tweetsBuffer.AddRange(_twitter.Search.Where(
                 status =>
                         status.Query == string.Format("from:{0}", song.Artist.TwitterHandle)
                         && status.Type == SearchType.Search
                         && status.Count == 100
                 ).First().Statuses);
            }

            _tweetsBuffer.AddRange(_twitter.Search.Where(
             status =>
                     status.Query == string.Format("{0}", song.Artist.Name)
                     && status.Type == SearchType.Search
                     && status.Count == 200
             ).First().Statuses);

            _tweetsBuffer.Shuffle();
        });
        yield return StartCoroutine(task.WaitRoutine());

        // Swap!
        var temp = _tweets;
        _tweets = _tweetsBuffer;
        _tweetsBuffer = temp;

        // Reset counter
        _tweetCounter = 0;
        _currentArtist = song.Artist;

        _isFetching = false;

        Debug.LogFormat("FOUND {0}", _tweets.Count);
        yield break;
    }
}
