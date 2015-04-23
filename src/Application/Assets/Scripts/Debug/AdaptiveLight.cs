using UnityEngine;
using System.Collections;

public class AdaptiveLight : MonoBehaviour {
    float[] samples = new float[128];
    private float refValue = 0.1f; // RMS value for 0 dB
    Light light;
    private DecayingFloat value = new DecayingFloat();

    void Start()
    {
        light = GetComponent<Light>();
    }

	// Update is called once per frame
	void Update () {
	    AudioListener.GetSpectrumData(samples, 0, FFTWindow.Hanning);

        var rms = (Mathf.Clamp(20 * Mathf.Log10(samples[1] / refValue), -80, 0) + 80) / 80;
        value.Update(rms);
        var val = value.Value;

        light.color = Color.Lerp(light.color, new Color(rms, rms, rms), Time.deltaTime * 10);
	}
}
