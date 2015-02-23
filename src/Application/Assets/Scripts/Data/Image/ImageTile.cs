using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class ImageTile : Tile {

    private Sprite _sprite;
    public Sprite sprite
    {
        get { return _sprite; }
        set { 
            _sprite = value;
            GetComponent<SpriteRenderer>().sprite = _sprite;
        }
    }

    protected void Start()
    {
        GetComponent<SpriteRenderer>().material.shader = Shader.Find("Sprites/Diffuse Fog");
    }

    protected override void OnDestroy()
    {
        // Free image memory
    }
}
