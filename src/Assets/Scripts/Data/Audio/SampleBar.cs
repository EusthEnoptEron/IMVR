using UnityEngine;
using System.Collections;
using System.Linq;

public class SampleBar : MonoBehaviour {
    public int frequency = 0;
    public float height = 1;
    public float speed = 10;
    public int startFrequency = 0;
    public int endFrequency = 0;
	// Update is called once per frame
	void Update () {

        if (MusicManager.Instance.amplitudes != null)
        {
            float val = MusicManager.Instance.amplitudes.Where((v, i) => i >= startFrequency && i < endFrequency).DefaultIfEmpty().Max();
            //float val = MusicManager.Instance.amplitudes.Length > frequency
            //            ? MusicManager.Instance.amplitudes[frequency]
            //            : 0;

            float maxAmplitude = Mathf.Max(MusicManager.Instance.maxAmplitude, 0.001f);


            var scale = new Vector3(transform.localScale.x,
                                                val / maxAmplitude,
                                               transform.localScale.z);

            transform.localScale = Vector3.Lerp(transform.localScale, scale, speed * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localScale.y / 2, transform.localPosition.z);
        }
	}
}
