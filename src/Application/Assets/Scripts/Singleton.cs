﻿using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static bool exiting = false;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null && !exiting)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {

                    string goName = typeof(T).ToString();

                    GameObject go = GameObject.Find(goName);
                    if (go == null)
                    {
                        go = new GameObject();
                        go.name = goName;
                    }

                    _instance = go.AddComponent<T>();
                }

                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    /// <summary>
    /// for garbage collection
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        exiting = true;
        // release reference on exit
        _instance = null;
    }
}
