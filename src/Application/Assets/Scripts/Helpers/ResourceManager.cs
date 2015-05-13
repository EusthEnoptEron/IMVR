using UnityEngine;
using System.Collections;
using IMVR.Commons;

public static class ResourceManager {

    private static Font _arial;
    private static Font _roboto;
    private static IMDB _db;

    public static Font Arial
    {
        get
        {
            if(_arial == null)
                _arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return _arial;
        }
    }

    public static Font Roboto
    {
        get
        {
            if (_roboto == null)
                _roboto = Resources.Load<Font>("Fonts/Roboto-Regular");
            return _roboto;
        }
    }

    public static IMDB DB
    {
        get
        {
            if (_db == null)
                _db = IMDB.FromFile(Prefs.Instance.DBPath);
            return _db;
        }
    }

}

public static class World
{
    public static GameObject CameraRig
    {
        get
        {
            return GameObject.FindGameObjectWithTag("CameraRig");
        }
    }

    public static GameObject WorldNode
    {
        get
        {
            return GameObject.Find("World");
        }
    }
}
