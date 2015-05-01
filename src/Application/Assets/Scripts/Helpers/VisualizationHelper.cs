using UnityEngine;
using System.Collections;
using System.Linq;

public class VisualizationHelper : Singleton<VisualizationHelper> {
    [HideInInspector]
    public float[] spectrum;

    [HideInInspector]
    public float[][] samples;

    public float[] energies;

    public int sampleCount = 512;

    private int sampleRate;

    [HideInInspector]
    public float stepSize;
    public float refValue = 1f; // RMS value for 0 dB
    public float minVal = 160;

    //public float maxVol = 0.1;

	// Use this for initialization
	void Awake () {
        spectrum = new float[sampleCount];
        samples = new float[2][];
        samples[0] = new float[sampleCount];
        samples[1] = new float[sampleCount];

        sampleRate = AudioSettings.outputSampleRate;
        stepSize = (sampleRate / 2f) / sampleCount;

        energies = new float[2];
	}
	
	// Update is called once per frame
    void Update()
    {
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < 2; i++)
        {
            AudioListener.GetOutputData(samples[i], i);

            // Determine volume
            float rms = Mathf.Sqrt(samples[i].Average(sample => sample * sample));
            energies[i] = rms;
            //volumes[i] = Mathf.Clamp01((20 * Mathf.Log10(rms / refValue) + minVal) / minVal);
        }
    }

    public int FreqToIndex(float freq)
    {
        return (int)(freq / stepSize);
    }

    public float IndexToFreq(int index)
    {
        return index * stepSize;
    }

    //public float GetSpectrum(int freq)
    //{
    //}
}
