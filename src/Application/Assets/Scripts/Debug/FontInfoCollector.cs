using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Helps collecting infos about the fonts in use
/// </summary>
public class FontInfoCollector : MonoBehaviour {
    private HashSet<string> _infoStrings = new HashSet<string>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            foreach (var text in GameObject.FindObjectsOfType<Text>())
            {
                var infoString = string.Format("{0} - {1} [{2}]", text.font.name, text.fontStyle, text.fontSize);   
                _infoStrings.Add(infoString);
            }
        }
	}

    void OnApplicationQuit()
    {
        Debug.Log("---FONT ANALYSIS---");
        foreach (var infoString in _infoStrings)
        {
            Debug.Log(infoString);
        }

        Debug.Log("-------------------");
    }
}
