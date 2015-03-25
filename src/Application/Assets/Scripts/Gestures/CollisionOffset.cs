using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CollisionOffset : UIBehaviour {
    public enum OffsetSource
    {
        Element,
        Camera
    }

    public OffsetSource source;
    public float offset = 1;
}
