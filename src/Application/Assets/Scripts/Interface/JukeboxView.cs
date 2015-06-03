using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Gestures;
using System.Linq;
using Leap;

public class JukeboxView : ArmWear {
    public float pixelsPerUnit = 1000f;

    private Jukebox m_jukebox;

    private Text m_sliderText;
    private Slider m_slider;
    private Text m_title;
    private bool m_updating = false;
    private CanvasGroup m_group;
    private UnityEngine.UI.Image m_coverImage;


    private bool visible = false;

    // Use this for initialization
	void Awake () {
        m_jukebox = Jukebox.Instance;
        m_slider = transform.GetComponentInChildren<Slider>();
        m_sliderText = transform.Find("SliderText").GetComponent<Text>();
        m_title = transform.Find("Title").GetComponent<Text>();
        m_coverImage = transform.FindChild("Cover").GetComponent<UnityEngine.UI.Image>();
        //m_jukebox.Playlist.Change += (sender, evt) => Rebuild();
        //m_jukebox.Playlist.IndexChange += (sender, evt) => Rebuild();


        transform.localScale = Vector3.one / pixelsPerUnit;

        Rebuild();
	}

    void Rebuild()
    {
        var song = m_jukebox.Playlist.Current;
        if (song == null)
        {
            m_title.text = "None";
            m_slider.value = 0;
            m_sliderText.text = "- / -";
        }
        else
        {
            m_title.text = song.Title;
        }
    }
	
	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateValues();
    }


    private void UpdateValues()
    {
        m_updating = true;
        {
            var song = m_jukebox.Playlist.Current;
            if (song != null)
            {
                var currentTime = m_jukebox.Time;
                var totalTime = m_jukebox.TotalTime;

                m_slider.value = m_jukebox.Progress;
                m_sliderText.text = String.Format("{0:00}:{1:00} / {2:00}:{3:00}",
                                    currentTime.Minutes, currentTime.Seconds,
                                    totalTime.Minutes, totalTime.Seconds);
                m_title.text = String.Format("{0} - {1}", song.Artist.Name, song.Title);
                m_coverImage.sprite = ImageAtlas.LoadSprite(song.Album.Atlas);
            }
            else
            {
                m_title.text = "None";
                m_slider.value = 0;
                m_sliderText.text = "- / -";
            }
        }
        m_updating = false;
    }

    public void Next()
    {
        Jukebox.Instance.Playlist.MoveForward();

    }

    public void Prev()
    {
        Jukebox.Instance.Playlist.MoveBackward();
    }


    private void SplitTime(float time, out int minutes, out int seconds)
    {
        minutes = (int)time / 60;
        seconds = Mathf.RoundToInt(time - minutes * 60);
    }

    public void Seek(float value)
    {
        if(!m_updating)
            m_jukebox.SeekRatio(value);
    }
}
