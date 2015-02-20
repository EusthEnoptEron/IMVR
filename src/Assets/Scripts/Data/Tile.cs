using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour {
    public int width = 1;
    public int height = 1;


    protected abstract void OnDestroy();
}
