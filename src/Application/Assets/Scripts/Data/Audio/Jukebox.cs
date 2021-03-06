﻿using UnityEngine;
using System.Collections;
using System;
using IMVR.Commons;
using System.Collections.Generic;
using UnityEngine.Audio;


[RequireComponent(typeof(AudioSource))]
public class Jukebox : Singleton<Jukebox> {
    private bool _playing = false;
    private bool _loading = false;
    private CSCAudioClip _currentClip;
    private AudioMixerGroup _audioGroup;


    /// <summary>
    /// Gets the playlist used by this jukebox.
    /// </summary>
    public Playlist Playlist
    {
        get;
        private set;
    }

    private AudioSource m_audio;

    protected void Awake()
    {
        Playlist = new Playlist();
        Playlist.IndexChange += OnIndexChange;

        _audioGroup = Resources.Load<AudioMixer>("Audio/Master Mix").FindMatchingGroups("BGM")[0];
        
        m_audio = GetComponent<AudioSource>();
        m_audio.outputAudioMixerGroup = _audioGroup;
    }

    private void OnIndexChange(object sender, EventArgs e)
    {
        StartCoroutine(PlayRoutine(Playlist.Current));
    }

    protected void Update()
    {
        if (_playing && !_loading)
        {
            if (!m_audio.isPlaying)
            {
                if (Playlist.MoveForward())
                    StartCoroutine(PlayRoutine(Playlist.Current));
                else
                    Stop();
            }
        }
    }

    public void Play()
    {

        _playing = true;
        if (!m_audio.isPlaying && !Playlist.IsEmpty) {
            if (Playlist.Current == null)
            {
                Playlist.MoveForward();
            }
            else
            {
                m_audio.Play();
            }
        }
    }

    public float Progress
    {
        get
        {
            if (!m_audio.clip) return 0;
            return m_audio.time / m_audio.clip.length;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public TimeSpan Time
    {
        get
        {
            // TODO: Cache
            return TimeSpan.FromSeconds(m_audio.timeSamples / m_audio.clip.frequency);
        }
    }

    public TimeSpan TotalTime
    {
        get
        {
            // TODO: Cache
            return TimeSpan.FromSeconds(m_audio.clip.length);
        }
    }

    private IEnumerator PlayRoutine(Song song)
    {
        _loading = true;
        Debug.Log("LOADING...");
        AudioClip audioClip = null;
        CSCAudioClip cscClip = null;

        if (song.Path.EndsWith(".ogg"))
        {
            var www = new WWW(new Uri(song.Path).AbsolutePath);
            yield return www;
            audioClip = www.audioClip;
        }
        else
        {
            cscClip = new CSCAudioClip(song.Path);
            audioClip = cscClip.Clip;
        }

        while(audioClip.loadState != AudioDataLoadState.Loaded)
            yield return null;

        //Debug.LogFormat("hey{0}/{1}", Playlist.Index, Playlist.Count);

        if (Playlist.Current == song)
        {
            if (_currentClip != null)
            {
                _currentClip.Dispose();
                _currentClip = null;
            }
            if(cscClip != null) _currentClip = new CSCAudioClip(song.Path);

            m_audio.time = 0;
            m_audio.clip = audioClip;
            
            //m_audio.clip = www.GetAudioClip(false);
            _loading = false;
            if (_playing)
            {
                m_audio.Play();
            }
        }
        else
        {
            // Was never used...
            cscClip.Dispose();
        }
    }

    public void Pause()
    {
        _playing = false;

        if (m_audio.isPlaying)
        {
            m_audio.Pause();
        }
    }

    public void Stop()
    {
        m_audio.Stop();
        _playing = false;

    }

    private void OnApplicationQuit()
    {
        // Clean CSCAudioClip if need be
        if (_currentClip != null)
            _currentClip.Dispose();
    }

    public void Seek(float time)
    {
        if (m_audio.clip)
        {
            m_audio.timeSamples = (int)(time * m_audio.clip.frequency);
            //m_audio.time = time;
        }
    }

    public void SeekRatio(float ratio)
    {
        if (m_audio.clip)
        {
            ratio = Mathf.Clamp01(ratio);
            Seek(m_audio.clip.length * ratio);            
        }
    }

    /// <summary>
    /// Plays a song immediately. Overrides the playlist.
    /// </summary>
    /// <param name="song">The song that will be played on the spot.</param>
    public void Play(Song song)
    {
        _playing = true;
        Playlist.Override(song);
        Playlist.MoveForward();
    }

    public bool IsPlaying
    {
        get
        {
            return _playing;
        }
    }
}
