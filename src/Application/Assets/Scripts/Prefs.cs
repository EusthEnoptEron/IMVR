using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Prefs : Singleton<Prefs> {
    
    public string DBPath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "IMDB.bin");
        }
    }


    public void Load()
    {

    }

    public void Save()
    {

    }


    private void Awake()
    {
        Load();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        Save();
    }

}


