﻿using UnityEngine;
using System.Collections;

public class EnvironmentController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetVolume(float val)
    {
        AudioListener.volume = val;
    }
}
