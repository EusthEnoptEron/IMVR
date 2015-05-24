using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent(typeof(AudioSource))]
public class PlayMusic : MonoBehaviour {

    public string path;

    private AudioSource _source;
    private CSCAudioClip _clip;

	// Use this for initialization
	void Start () {
        _source = GetComponent<AudioSource>();

        if (File.Exists(path))
        {
            _clip = new CSCAudioClip(path);
            _source.clip = _clip.Clip;

            if (!_source.isPlaying)
                _source.Play();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnApplicationQuit()
    {
        Debug.Log("DISPOSE");
        if(_clip != null)
            _clip.Dispose();
    }
}
