using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

//[RequireComponent(typeof(Mask), typeof(Image))]
public class DummyTile : Tile, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    protected Image m_image;
    private Color oldColor;
    private Color targetColor;
    private bool clicked = false;

    public Color Color
    {
        get { return m_image.color; }
        set
        {
            m_image.color = value;
        }
    }

    protected RectTransform rectTransform;

    private static Sprite _maskSprite;
    private static Sprite maskSprite {
        get {
            if(_maskSprite == null) {
                var texture = Resources.Load("Textures/CircleMask") as Texture2D;
                
                _maskSprite = Sprite.Create(texture,
                                            new Rect(0, 0, texture.width, texture.height),
                                            new Vector2(0.5f, 0.5f), texture.width, 0, SpriteMeshType.FullRect);
            }
            return _maskSprite;
        }
    }

    void Start()
    {
        oldColor = Color;
        targetColor = oldColor;
    }

    void Awake()
    {
        //var mask = GetComponent<Image>();
        //mask.sprite = maskSprite;
        
        //m_image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = Vector2.one * 100;

        var component = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/DebugPanel"));
        //m_image = new GameObject().AddComponent<Image>();
        m_image = component.GetComponentInChildren<Image>();

        // Create tile
        component.transform.SetParent(transform, false);
        //component.transform.localPosition = Vector3.zero;
        //component.transform.localRotation = Quaternion.identity;
        //component.transform.localScale = Vector3.one;
        m_image.sprite = maskSprite;
        //m_image.text = Random.Range(0, 100).ToString();
        //m_image.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        //var imageRect = m_image.GetComponent<RectTransform>();
        //{
        //    //imageRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 1);

        //    // Fill rect
        //    imageRect.anchorMin = new Vector2(0, 0);
        //    imageRect.anchorMax = new Vector2(1, 1);
        //    imageRect.offsetMin = new Vector2(0, 0);
        //    imageRect.offsetMax = new Vector2(0, 0);
        //}

    }

	
    //// Update is called once per frame
    //protected override void Update () {
    //    //base.Update();

    //    Color = Color.Lerp(Color, targetColor, Time.deltaTime * 5);
    //}

    protected override void OnDestroy()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        m_image.DOKill();
        m_image.DOColor(Color.red, 0.1f);        
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
            m_image.DOColor(Color.green, 1f);
        }
    }

    public class LightImage : Image
    {
    }
}
