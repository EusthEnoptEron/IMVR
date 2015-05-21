using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using IMVR.Commons;
using System.IO;
using Gestures;

public class RootPanel : Singleton<RootPanel> {

    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot menuSnapshot;


    private ModeController _controller;
    private float downThresholdEnter = 0.8f;
    private float downThresholdStay = 0.5f;
    private bool isMenuMode = false;
    private bool _invokeStarted = false;

    public ModeController Controller
    {
        get
        {
            return _controller;
        }
        set
        {
            if (_controller != null) _controller.enabled = false;

            _controller = value;

            _controller.enabled = true;
        }
    }

    private void Awake()
    {
        var menuSong = new Song()
        {
            Artist = new Artist()
            {
                Name = "煉獄庭園"
            },
            Album = new Album(),
            Title = "円-Madoka-",
            Path  = Path.Combine(Application.streamingAssetsPath, @"Audio\madoka.mp3")
        };

        Jukebox.Instance.Playlist.Add(menuSong);
        Jukebox.Instance.Playlist.Cyclic = true;

        Jukebox.Instance.Play();
    }

    private void Update()
    {
        bool lookingDown = IsLookingDown;
        if (lookingDown)
        {
            SetMode(true);

            if (isMenuMode)
            {
                var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                Debug.DrawRay(ray.origin, ray.direction);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 3f, LayerMask.GetMask("Ringpanel")))
                {
                    var activator = hit.collider.GetComponentInParent<RingActivator>();
                    if (activator != null)
                    {
                        activator.Fill();
                    }
                }
            }
        }
        else
        {
            SetMode(false);
        }
    }

    private void SetMode(bool menuMode)
    {
        if (isMenuMode && !menuMode)
        {
            _invokeStarted = false;
            CancelInvoke("Activate");
            // Enabled -> Disabled
            normalSnapshot.TransitionTo(0.1f);
            HandProvider.Instance.enabled = true;


            isMenuMode = false;
        }
        else if (!isMenuMode && menuMode && !_invokeStarted)
        {
            // Disabled -> Enabled
            menuSnapshot.TransitionTo(0.5f);
            _invokeStarted = true;
            Invoke("Activate", 0.5f);
        }
    }

    private void Activate()
    {
        HandProvider.Instance.GetComponent<HandController>().DestroyAllHands();

        HandProvider.Instance.enabled = false;
        isMenuMode = true;
        _invokeStarted = false;
    }

    private bool IsLookingDown
    {
        get
        {
            return Mathf.Abs(Vector3.Dot(Vector3.down, Camera.main.transform.forward)) > (isMenuMode ? downThresholdStay : downThresholdEnter);
        }
    }

}
