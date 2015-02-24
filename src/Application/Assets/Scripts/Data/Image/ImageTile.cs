using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VirtualHands.Data;
using VirtualHands.Data.Image;

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

        OnEnable();
    }

    protected void OnEnable()
    {
        if (File != null)
        {
            var texture = DeferredLoader.Instance.LoadTexture(File.Path);
            sprite = Sprite.Create(texture,
                                   new Rect(0, 0, texture.width, texture.width),
                                   new Vector2(0.5f, 0.5f), texture.width);

        }

    }

    protected void OnDisable()
    {
        var loader = DeferredLoader.Instance;
        if (loader != null)
        {
            DeferredLoader.Instance.UnloadTexture(File.Path);
        }
        //Destroy(sprite.texture);
        sprite = null;

    }

    protected override void OnDestroy()
    {
        // Free image memory
    }
}
