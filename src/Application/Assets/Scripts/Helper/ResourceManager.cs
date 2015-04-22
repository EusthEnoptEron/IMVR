using UnityEngine;
using System.Collections;

public static class ResourceManager {

    private static Font _arial;
    public static Font Arial
    {
        get
        {
            if(_arial == null)
                _arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return _arial;
        }
    }
}
