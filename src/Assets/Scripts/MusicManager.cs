using UnityEngine;
using System.Collections;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;
using System.Linq;
using System.Collections.Generic;

public class MusicManager : Singleton<MusicManager> {
    private WaveOutEvent player;
    private AudioFileReader reader;
    private NotifyingSampleProvider notifier;
    private SampleAggregator aggregator;

    public int fftLength = (int)Mathf.Pow(2, 10);
    public float[] amplitudes;
    public float maxAmplitude = 0;

    public int Frequencies
    {
        get
        {
            return fftLength / 2 - 1;
        }
    }

    public int BinSize
    {
        get
        {
            return Mathf.RoundToInt((float)reader.WaveFormat.SampleRate / fftLength);
        }
    }

	// Use this for initialization
	void Awake () {
        player = new WaveOutEvent();
        aggregator = new SampleAggregator(fftLength);
        aggregator.FftCalculated += aggregator_FftCalculated;
        aggregator.PerformFFT = true;

        amplitudes = new float[Frequencies];
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playmodeStateChanged = HandlePlayModeChanged;
#endif
	}



#if UNITY_EDITOR
    private void HandlePlayModeChanged()
    {
        if (player != null)
        {
            if (UnityEditor.EditorApplication.isPaused) Pause();
            else Resume();

        }
    }
#endif

    void aggregator_FftCalculated(object sender, FftEventArgs e)
    {
        //amplitudes = e.Result.Skip(1).Take(Frequencies).Select(c => Mathf.Sqrt(c.X * c.X + c.Y * c.Y)).ToArray();
        amplitudes = e.Result.Skip(1).Take(Frequencies).Select(c => Mathf.Log(Mathf.Sqrt(c.X * c.X + c.Y * c.Y) + 1)).ToArray();
        maxAmplitude = Mathf.Max(maxAmplitude, amplitudes.Max());
    }
	
	// Update is called once per frame
	void Update () {
	       
	}

    public void Play(string file)
    {
        if (reader != null)
        {
            player.Stop();
            reader.Dispose();
        }
        
        reader = new AudioFileReader(file);
        notifier = new NotifyingSampleProvider(reader);
        notifier.Sample += notifier_Sample;
        
        player.Init(notifier);

        player.Play();
    }

    public void Resume()
    {
        if (player.PlaybackState == PlaybackState.Paused)
        {
            player.Play();
        }
    }

    public void Pause()
    {
        if (player.PlaybackState == PlaybackState.Playing)
        {
            player.Pause();
        }
    }


    void notifier_Sample(object sender, SampleEventArgs e)
    {
        aggregator.Add(e.Left);
    }

    private void OnDestroy()
    {
        player.Stop();

        player.Dispose();
        player = null;


        if (reader != null)
        {
            reader.Close();

        }
    }

    /// <summary>
    /// Gets all values between the frequency range
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public IEnumerable<float> GetRange(float min, float max)
    {
        int startFrequency = Mathf.RoundToInt(min / BinSize);
        int endFrequency = Mathf.RoundToInt(max / BinSize);

        return amplitudes.Where((a, i) => i >= startFrequency && i <= endFrequency);
    }


    //private void OnApplicationPause(bool pauseStatus)
    //{
    //    if (!pauseStatus) Pause();
    //    else Resume();
    //}




}
