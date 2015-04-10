using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DebugSpriteMaterial : MonoBehaviour {
    Sprite sprite;
	// Use this for initialization
	void Start () {
        var material = GetComponent<Image>().material;

        Debug.Log(material.shader.name);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}