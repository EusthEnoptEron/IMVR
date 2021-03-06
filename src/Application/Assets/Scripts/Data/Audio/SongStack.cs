﻿using UnityEngine;
using System.Collections;
using IMVR.Commons;
using System.Collections.Generic;
using System.Linq;

public class SongStack : MonoBehaviour {
    private static GameObject pref_Plate = Resources.Load<GameObject>("Prefabs/Objects/pref_Plate");

    public IEnumerable<Song> items;
    public int totalCount;
    public float maxHeight = 10;
    public float margin = 0.05f;

    // Use this for initialization
	void Start () {
        float itemCount = items.Count();
        float rate = itemCount / totalCount;

        //Debug.LogFormat("{0}: {1} / {2}", name, itemCount, totalCount);
        // Create blocks!
        float reachedHeight = 0;
        Color low = Color.red;
        Color high = Color.green;

        while (reachedHeight < rate * maxHeight)
        {
            var plate = GameObject.Instantiate<GameObject>(pref_Plate);
            float height = plate.transform.localScale.y;
            plate.transform.SetParent(transform, false);

            plate.transform.localScale *= Tile.PIXELS_PER_UNIT;
            plate.transform.localPosition = Vector3.up * reachedHeight * Tile.PIXELS_PER_UNIT + Vector3.down *  Tile.PIXELS_PER_UNIT / 2;
            plate.GetComponent<MeshRenderer>().material.color = Color.Lerp(low, high, reachedHeight / (maxHeight));
            reachedHeight += height + margin;
        }
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
