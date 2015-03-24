using UnityEngine;
using System.Collections;
using System.Linq;
using Leap;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Globalization;
using System;
using System.Collections.Generic;
using Gestures;

public class PositionCapture : MonoBehaviour {

    HandController controller;
    enum Mode
    {
        Idle,
        Capturing,
        Comparing
    }
    Mode mode = Mode.Idle;

    LinkedList<GenericHand> hands = new LinkedList<GenericHand>();
    GenericHand[] toSave;

    LeapAnimationVector leapVector = null;
    float max = -1;

    private string m_title = "";
    private bool m_isAnimation = true;

	// Use this for initialization
	void Start () {
        controller = GetComponent<HandController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (mode == Mode.Capturing)
        {
            CaptureHand();
        }
        if (mode == Mode.Comparing)
        {
            CaptureHand();
           // Debug.Log(String.Format("Vector: {0} Hands: {1}", leapVector.HandCount, hands.Count ));
            if (hands.Count >= leapVector.HandCount)
            {
                while (hands.Count > leapVector.HandCount)
                {
                    hands.RemoveLast();
                }

                var newVector = new LeapAnimationVector(hands);

                float similarity = SimilarityVector.Similarity(newVector, leapVector);
                if (similarity > 0.8f)
                {
                    Debug.Log("MATCH " + similarity);
                }
            }
            // Do additional stuff..
        }
	}

    void OnGUI()
    {
        m_isAnimation = GUI.Toggle(new Rect(10, 150, 150, 20), m_isAnimation, "Is Animation");

        if (mode == Mode.Capturing)
        {
            if (GUI.Button(new Rect(10, 10, 150, 100), "Stop"))
            {
                StopCapturing();
                mode = Mode.Comparing;
            }
        }
        else if(mode == Mode.Idle)
        {
            if (GUI.Button(new Rect(10, 10, 150, 100), "Capture"))
            {
                mode = Mode.Capturing;
            }
        }
        else if (mode == Mode.Comparing)
        {
            m_title = GUI.TextField(new Rect(200, 10, 150, 50), m_title);

            if (GUI.Button(new Rect(10, 10, 150, 100), "Save"))
            {
                if(m_title.Length == 0) m_title = "hand_" + DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss");

                if (!m_isAnimation)
                {
                    SaveGesture(Application.dataPath + "/Snapshots/" + m_title + ".bin", toSave.Take(1).ToArray());
                }
                else
                {
                    SaveGesture(Application.dataPath + "/Snapshots/" + m_title + ".bin", toSave);
                }

                m_title = "";
                
                mode = Mode.Idle;
            }
        }
            if (GUI.Button(new Rect(400, 10, 150, 100), "Load"))
            {
                LoadGesture();
            }
    }

    private void StopCapturing()
    {
        leapVector = new LeapAnimationVector(hands);
        toSave = hands.ToArray();

        hands.Clear();
    }

    void CaptureHand()
    {
        var hand = GetHand();
        if(hand != null)
            hands.AddFirst(hand);
    }

    GenericHand GetHand()
    {

        var gHands = controller.GetAllGraphicsHands();
        var hand = gHands.FirstOrDefault();
        if (hand != null && hand.GetLeapHand() != null && hand.GetLeapHand().IsValid)
            return new GenericHand(hand);
        else
            return null;
    }


    void LoadGesture()
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.dataPath + "/Snapshots/HandFlickLeft.bin";
        GenericHand[] hands;

        using (var stream = File.OpenRead(path))
        {
            hands = bf.Deserialize(stream) as GenericHand[];
        }
        leapVector = new LeapAnimationVector(hands);
        mode = Mode.Comparing;

        Debug.Log(hands.Length);
    }

    void SaveGesture(string path, GenericHand[] hands)
    {
        Debug.Log(hands.Length);

        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(path)) File.Delete(path);
        using (var stream = File.Create(path))
        {
            bf.Serialize(stream, hands);
        }

        Debug.Log("Saved to " + path);

    }

}
