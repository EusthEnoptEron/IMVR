using UnityEngine;
using System.Collections;

public static class ResourceManager {

    private static Font _arial;
    private static Font _roboto;

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
            if (_arial == null)
                _arial = Resources.Load<Font>("Fonts/Roboto-Regular.ttf");
            return _arial;
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
