using UnityEngine;
using System.Collections;

public class SpectrumBar : MonoBehaviour {

    public int startFrequency = 20;
    public int endFrequency = 80;
    public float speed = 20;

    private int m_startIndex;
    private int m_endIndex;

	// Use this for initialization
	void Start () {
        m_startIndex = VisualizationHelper.Instance.FreqToIndex(startFrequency);
        m_endIndex = VisualizationHelper.Instance.FreqToIndex(endFrequency);

	}
	
	// Update is called once per frame
	void Update () {

        float rms = 0;
        for(int i = m_startIndex; i < m_endIndex; i++) {
            rms += Mathf.Pow(VisualizationHelper.Instance.spectrum[m_startIndex], 2);
        }
        rms = Mathf.Sqrt( rms / (m_endIndex - m_startIndex) );

        var vol = Mathf.Clamp01((20 * Mathf.Log10(rms / 0.1f) + 60) / 60);
 
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            new Vector3(transform.localScale.x, vol, transform.localScale.z),
            Time.deltaTime * speed
        );
	}
}
