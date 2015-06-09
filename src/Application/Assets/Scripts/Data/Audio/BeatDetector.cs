using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class BeatEventArgs : EventArgs
{
    public float Strength { get; private set; }

    internal BeatEventArgs(float strength)
    {
        Strength = strength;
    }
}
public class BeatDetector : Singleton<BeatDetector> {
    public float C = 1.3f;

    private LinkedList<float> E = new LinkedList<float>();
    public int historyLength = 100;

    private float lastBeat;
    public float beatInterval = 0.5f;

    private VisualizationHelper helper;

    public EventHandler<BeatEventArgs> Beat = delegate { };

	// Use this for initialization
	void Start () {

        helper = VisualizationHelper.Instance;
        E = new LinkedList<float>();
        for (int i = 0; i < historyLength; i++)
        {
            E.AddFirst(0.01f);
        }
	}
	
	// Update is called once per frame
	void Update () {
        //float e = helper.energies[0] + helper.energies[1];

        // bass frequencies
        int startIndex = helper.FreqToIndex(50);
        int endIndex = helper.FreqToIndex(200);

        float e = 0;
        for (int i = startIndex; i <= endIndex; i++)
        {
            e += helper.spectrum[i] * helper.spectrum[i];
        }

        e = Mathf.Sqrt(e / (endIndex - startIndex));

        E.AddFirst(e);
        E.RemoveLast();

        if (Time.time - lastBeat > beatInterval)
        {
            float Energy = E.Average();
            if (e > C * Energy)
            {
                lastBeat = Time.time;
                var args = new BeatEventArgs(e);
                SendMessage("OnBeat", e, SendMessageOptions.DontRequireReceiver);
                Beat(this, args);
            }
        }
	}
}
