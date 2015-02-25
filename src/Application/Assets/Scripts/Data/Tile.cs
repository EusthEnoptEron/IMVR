using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using VirtualHands.Data;

public abstract class Tile : UIBehaviour {
    public int width = 1;
    public int height = 1;

    public File File { get; set; }

    protected abstract void OnDestroy();
}
