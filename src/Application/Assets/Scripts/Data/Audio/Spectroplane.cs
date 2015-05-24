using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Spectroplane : MonoBehaviour {

    public int sampleCount = 512;
    public int binCount = 100;
    public int historyLength = 30;
    public float height = 10;
    public float speed = 10;

    private CustomPlane plane;
    private float[] samples;
    private MeshFilter filter;
    private float refValue = 0.1f; // RMS value for 0 dB

    private float ratio;
    private float slope = 6;

	// Use this for initialization
	void Start () {
        this.plane = GetComponent<CustomPlane>();
        this.samples = new float[sampleCount];
        this.filter = GetComponent<MeshFilter>();

        if (plane == null) plane = gameObject.AddComponent<CustomPlane>();


        // Create mesh
        plane.xSize = 10;
        plane.zSize = 10;
        plane.xSegments = binCount - 1;
        plane.zSegments = historyLength;
        plane.UpdateMesh();

        ratio = Mathf.Pow(AudioSettings.outputSampleRate, 1 / slope) / binCount;
	}

    public float FreqDelta
    {
        get
        {
            return AudioSettings.outputSampleRate / sampleCount;
        }
    }

    /// <summary>
    /// Gets the RMS value of a specified bin
    /// </summary>
    /// <param name="bin"></param>
    /// <returns></returns>
    public float GetValue(int bin)
    {
        int startIndex = GetBinIndex(bin);
        int endIndex   = Mathf.Max(startIndex+1, GetBinIndex(bin + 1));


        float sum = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            sum += samples[i] * samples[i];
        }

        return Mathf.Sqrt(sum / (endIndex - startIndex));
    }

    public float GetValue(int bin, int binCount)
    {
        int startIndex = GetBinIndex(bin, binCount);
        int endIndex = Mathf.Max(startIndex + 1, GetBinIndex(bin + 1, binCount));


        float sum = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            sum += samples[i] * samples[i];
        }

        return Mathf.Sqrt(sum / (endIndex - startIndex));
    }

    public int GetBinIndex(int bin)
    {
        // Skip DC part
        if (bin <= 0) return 1;

        //Debug.LogFormat("{0}: {1}", bin,  (float)binCount / AudioSettings.outputSampleRate *  (Mathf.Pow((bin) * ratio, slope) + 50));
        return Mathf.Clamp(
            Mathf.Max(GetBinIndex(bin-1) + 1,
            (int)((Mathf.Pow((bin) * ratio, slope) + 50) / FreqDelta)), 0, sampleCount - 1);
    }

    public int GetBinIndex(int bin, int binCount)
    {
        // Skip DC part
        if (bin <= 0) return 1;

        float ratio = Mathf.Pow(AudioSettings.outputSampleRate, 1 / slope) / binCount;
        //Debug.LogFormat("{0}: {1}", bin,  (float)binCount / AudioSettings.outputSampleRate *  (Mathf.Pow((bin) * ratio, slope) + 50));
        return Mathf.Clamp(
            Mathf.Max(GetBinIndex(bin - 1) + 1,
            (int)((Mathf.Pow((bin) * ratio, slope) + 50) / FreqDelta)), 0, sampleCount - 1);
    }


	// Update is called once per frame
	void Update () {
        // Fill data
        AudioListener.GetSpectrumData(samples, 0, FFTWindow.Hamming);


        var mesh = filter.sharedMesh;
        var vertices = mesh.vertices;

        // Update mesh
        for (int iz = plane.zSegments; iz >= 0; iz--)
        {
            var prevZ = iz - 1;

            for (int ix = 0; ix < plane.xSegments + 1; ix++)
            {
                var index = iz * (plane.xSegments + 1) + ix;
                if (prevZ >= 0)
                {
                    // Copy
                    vertices[index] = new Vector3(
                        vertices[index].x,
                        Mathf.Lerp(vertices[index].y, vertices[prevZ * (plane.xSegments + 1) + ix].y, Time.deltaTime * speed),
                        vertices[index].z);
                }
                else
                {
                    // Use input
                    vertices[index] = new Vector3(
                        vertices[index].x, 
                        Mathf.Lerp(vertices[index].y, (Mathf.Clamp(20 * Mathf.Log10(GetValue(ix) / refValue), -80, 0) + 80) / 80 * height, Time.deltaTime * speed), 
                        vertices[index].z);
                }
            }
        }

        mesh.vertices = vertices;
        filter.sharedMesh = mesh;
	}

}
