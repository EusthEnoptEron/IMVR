using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VirtualHands.Data;
using VirtualHands.Data.Image;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;



//[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Image))]
public class ImageTile : Tile, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool clicked = false;

    private Sprite _sprite;
    private Color oldColor;
    private static Color highlightColor = new Color(1, 1, 1, 0.7f);
    private static Color clickColor = new Color(1, 1, 1, 1f);

    private Color targetColor;

    private static Material SpriteMaterial = Resources.Load("Materials/Sprites-Diffuse") as Material;
    //private static Material SpriteMaterial = new Material(Shader.Find("Sprites/Diffuse Fog"));

    private Image m_image;

    private IMVR.Commons.Image _image;
    public IMVR.Commons.Image Image
    {
        get { return _image; }
        set
        {
            _image = value;
            m_image.sprite = ImageAtlas.LoadSprite(value.Atlas);
        }
    }

    //public Sprite sprite
    //{
    //    get { return _sprite; }
    //    set { 
    //        _sprite = value;
    //        GetComponent<Image>().sprite = _sprite;
    //    }
    //}
    protected void Awake()
    {
        m_image = GetComponent<Image>();
   
    }
    protected void Start()
    {
        //GetComponent<SpriteRenderer>().sharedMaterial = SpriteMaterial;
        //GetComponent<Image>().rectTransform.sizeDelta = new Vector2(1, 1);
        OnEnable();
        oldColor = new Color(1, 1, 1, 0.8f);
        m_image.color = oldColor;
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
            //sprite = DeferredLoader.Instance.LoadSprite(path);
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


    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        m_image.DOKill();
        m_image.DOColor(clickColor, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
        m_image.DOKill();
        m_image.DOColor(oldColor, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!clicked)
        {
            m_image.DOKill();
            m_image.DOColor(oldColor, 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!clicked)
        {
            m_image.DOKill();

            m_image.DOColor(highlightColor, 1f);
        }
    }

}
