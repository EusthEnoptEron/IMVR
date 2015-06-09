using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class RingVisualizer : MonoBehaviour {
    public enum Side { Left = 0, Right = 1 }

    public Side side = Side.Left;
    public float radius = 1f;
    public float amplitude = 1f;
    public float speed = 30;
    public int resolution = 1000;

    private LineRenderer lineRenderer;
    private VisualizationHelper helper;
    private const float TWOPI = Mathf.PI * 2;

    private int binCount = 10;

    private LinkedList<float> volumes;
    private Vector3[] points;
    public float[] indices;

	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        helper = VisualizationHelper.Instance;

        // Initialize lineRenderer
        points = new Vector3[resolution];
        lineRenderer.SetVertexCount(resolution);
        lineRenderer.useWorldSpace = false;


        // Fill linked list
        volumes = new LinkedList<float>();
        for (int i = 0; i < resolution; i++)
            volumes.AddFirst(0);

        //BuildIndices();

	}

    //void BuildIndices()
    //{
    //    indices = new float[helper.sampleCount];
    //    float slope = 3;
    //    float ratio = Mathf.Pow(AudioSettings.outputSampleRate / 2, 1 / slope) / helper.sampleCount;

    //    for (int i = 0; i < indices.Length; i++)
    //    {
    //        indices[i] = ((1 - Mathf.Log((helper.sampleCount + 1) - i, (helper.sampleCount + 1))) * (helper.sampleCount - 1));
    //        indices[i] = Mathf.Clamp((((Mathf.Pow(i * ratio, slope) + 50) / helper.stepSize)), 0, helper.sampleCount - 1);
    //    }
    //}

	// Update is called once per frame
	void Update () {
        // Move everything behind one step
        volumes.AddFirst(helper.energies[(int)side]);
        volumes.RemoveLast();

        Vector3 position = Vector3.zero;

        int i = 0;
        foreach (float vol in volumes)
        {
            float rate = i / (resolution - 1f);
            position.x = Mathf.Cos(rate * TWOPI) * radius;
            position.y = vol * amplitude;
            position.z = Mathf.Sin(rate * TWOPI) * radius;

            //points[i] = position;
            points[i] = Vector3.Lerp(points[i], position, Time.deltaTime * speed);
            lineRenderer.SetPosition(i, points[i]);

            i++;
        }

        lineRenderer.SetPosition(0, points[resolution - 1]);


        //Vector3 position = Vector3.zero;
        //maxVal = Mathf.Max(maxVal, helper.spectrum.Max());

        //for (int i = 1; i < helper.sampleCount; i++)
        //{
        //    float rate = i / (helper.sampleCount-1f);
        //    position.x = Mathf.Cos(rate * TWOPI);
        //    position.y = GetValue(i) * amplitude;
        //    position.z = Mathf.Sin(rate * TWOPI);
           
        //    points[i] = Vector3.Lerp(points[i], position, Time.deltaTime * speed);

        //    lineRenderer.SetPosition(i, points[i]);
        //}
        //lineRenderer.SetPosition(0, points[helper.sampleCount - 1]);
	}

    //private float GetValue(int i)
    //{
    //    int baseIndex = (int)indices[i];
    //    float progress = indices[i] - baseIndex;

    //    return (Mathf.Lerp((helper.spectrum[baseIndex]), (helper.spectrum[baseIndex + 1]), progress)) / maxVal;
    //}

    //private float ToLogarithmic(float value)
    //{
    //    return Mathf.Clamp(20 * Mathf.Log10(value / 0.1f), -200, 0) / 200 + 1;
    //}
}
