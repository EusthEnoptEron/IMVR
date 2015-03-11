using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VirtualHands.Data;
using VirtualHands.Data.Image;
using System.Collections.Generic;



//[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Image))]
public class ImageTile : Tile {

    private Sprite _sprite;
    private static Material SpriteMaterial = Resources.Load("Materials/Sprites-Diffuse") as Material;
    //private static Material SpriteMaterial = new Material(Shader.Find("Sprites/Diffuse Fog"));
    
    public Sprite sprite
    {
        get { return _sprite; }
        set { 
            _sprite = value;
            GetComponent<Image>().sprite = _sprite;
        }
    }

    protected void Start()
    {

        //GetComponent<SpriteRenderer>().sharedMaterial = SpriteMaterial;
        GetComponent<Image>().rectTransform.sizeDelta = new Vector2(1, 1);
        OnEnable();
    }


    protected void OnEnable()
    {
        if (File != null)
        {
            //var texture = DeferredLoader.Instance.LoadTexture(File.Path);
            //sprite = Sprite.Create(texture,
            //                       new Rect(0, 0, texture.width, texture.width),
            //                       new Vector2(0.5f, 0.5f), texture.width);

            
            string path = File.Path;
            sprite = DeferredLoader.Instance.LoadSprite(path);
        }

    }

    protected void OnDisable()
    {
        //var loader = DeferredLoader.Instance;
        //if (loader != null)
        //{
        //    DeferredLoader.Instance.UnloadTexture(File.Path);
        //}
        ////Destroy(sprite.texture);
        //sprite = null;

    }

    protected override void OnDestroy()
    {
        // Free image memory
    }
}
