using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Gestures;

public class LayoutGroup : TileGroup, IPointerEnterHandler, IPointerExitHandler {//, IFingerDownHandler, IFingerUpHandler {
    private DialLayout layout;
    private Mask mask;
    private List<GameObject> children = new List<GameObject>();

    private int m_fingers = 0;
    public float heightPerElement = 50;
    public float scrollSpeed = 0;
    private float currentAngle = 0;
    private float m_alpha = 0.05f;
    private float TargetAngle
    {
        get
        {
            if (layout.transform.childCount <= 1) return 0;
            float step = (layout.maxAngle - layout.minAngle) / (layout.transform.childCount-1);
            int factor = Mathf.RoundToInt(currentAngle / step);
            return Mathf.Clamp(factor * step, layout.minAngle, layout.maxAngle);
        }
    }

    private bool _dragging = false;
    private Vector3 _lastPosition;


	// Use this for initialization
	void Awake () {
        base.Awake();

        mask = new GameObject().AddComponent<Image>().gameObject.AddComponent<LooseGroup>().gameObject.AddComponent<Mask>();
        layout = new GameObject().AddComponent<DialLayout>();

        mask.transform.SetParent(transform);
        layout.transform.SetParent(mask.transform);

        // Configure mask
        mask.transform.localPosition = Vector3.zero;
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(Tile.PIXELS_PER_UNIT * 0.8f, Tile.PIXELS_PER_UNIT * 5);
        
        var outline = mask.gameObject.AddComponent<Outline>();
        outline.useGraphicAlpha = false;
        mask.showMaskGraphic = true;
        mask.GetComponent<Image>().color = new Color(1, 1, 1, m_alpha);
       
        text.transform.localPosition = new Vector3(0, -Tile.PIXELS_PER_UNIT * 3, 0);

        // Configure layout
        var layoutRect = layout.GetComponent<RectTransform>();
        layoutRect.anchorMin = new Vector2(0, 0);
        layoutRect.anchorMax = new Vector2(1, 1);
        layoutRect.sizeDelta = new Vector2(0, 0);
        layout.childAlignment = TextAnchor.MiddleCenter;

	}



    public override void SetElements(List<GameObject> gos)
    {
        layout.transform.DetachChildren();
        layout.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightPerElement * gos.Count);
        //layout.GetComponent<RectTransform>().sizeDelta = new Vector2(
        //    100,
        //    heightPerElement * gos.Count
        //);

        foreach (var go in gos)
        {
            go.transform.SetParent(layout.transform);
            
            if (!go.GetComponent<CanvasGroup>())
            {
                go.AddComponent<CanvasGroup>();
            }
        }

        foreach (var child in layout.transform.Children())
        {
            child.SetAsFirstSibling();
        }

        children = gos;
    }

    protected override void OnDestroy()
    {
    }

    private Accumulator<float> accumulator = new Accumulator<float>();
    private bool dragging = false;


    private float delta;
    private float deadZone = 0;
    public void OnDrag()
    {
        var pos = HandProvider.Instance.GetHand(HandType.Right, NoHandStrategy.Keep).PalmPosition;

        float angleDelta = Vector3.Dot(pos - _lastPosition, Camera.main.transform.up) * 100;
        scrollSpeed = 0;

        if (dragging)
        {
            accumulator.Add(angleDelta);
            currentAngle += angleDelta;
        } else if (Mathf.Abs(delta) > deadZone && !dragging)
        {
            dragging = true;
            currentAngle += delta;
        }
        else
        {
            // Bubble
            //ExecuteEvents.ExecuteHierarchy<IDragHandler>(gameObject.transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
        }

        //layout.transform.localRotation *= Quaternion.Euler(eventData.delta.y * .5f, 0, 0);
        //scrollSpeed += eventData.delta.y;
        //layout.transform.localPosition += Vector3.up * eventData.delta.y;
        delta += angleDelta;

        _lastPosition = pos;
    }


    void Update()
    {
        if (!dragging)
        {
            if (Mathf.Abs(scrollSpeed) > 0)
            {
                currentAngle += scrollSpeed * Time.deltaTime;
                scrollSpeed = Mathf.MoveTowards(scrollSpeed, 0, 100 * Time.deltaTime);

                ApplyRotations();
            }
            else
            {
                currentAngle = Mathf.Lerp(currentAngle, TargetAngle, Time.deltaTime * 5);
                ApplyRotations();
            }
        }
        else
        {
            OnDrag();
        }


        layout.transform.localPosition = new Vector3(layout.transform.localPosition.x,
                                                     layout.transform.localPosition.y,
                                                     layout.Radius);

        // Do culling
        foreach (var child in children)
        {
            if (Vector3.Dot(child.transform.forward, transform.forward) < 0)
            {
                // Disable
                child.SetActive(false);
                //child.GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                // Enable
                child.SetActive(true);
                //child.GetComponent<CanvasGroup>().alpha = 1;
            }
        }

        if (dragging) ApplyRotations();

        ApplyConstraints();
    }


    void ApplyRotations()
    {
        layout.transform.localRotation = Quaternion.Euler(currentAngle, 0, 0);
    }

    void ApplyConstraints()
    {
        float prev = currentAngle;
        currentAngle = Mathf.Clamp(currentAngle, layout.minAngle, layout.maxAngle);
        if(prev != currentAngle) Debug.LogFormat("{0}->{1}", prev, currentAngle);
    }

    private float ClampAngle(float angle, float from, float to)
    {
        if (angle > 180) angle = 360 - angle;
        angle = Mathf.Clamp(angle, from, to);
        if (angle < 0) angle = 360 + angle;

        return angle;
    }


    public void BeginDrag()
    {
        //GetComponent<BaseRaycaster>().enabled = false;
        //dragging = true;
        //_lastPosition = HandProvider.Instance.GetHand(HandType.Right, NoHandStrategy.Keep).PalmPosition;

        ////dragging = true;
        //accumulator.Clear();
        //delta = 0;

        //var image = mask.GetComponent<Image>();
        //image.DOKill();
        //image.DOColor(new Color(0.8f, 1, 0.8f, m_alpha), 0.5f);
        

    }

    public void EndDrag()
    {
        //GetComponent<BaseRaycaster>().enabled = true;

        //scrollSpeed = accumulator.Values.Sum();
        //dragging = false;


        //var image = mask.GetComponent<Image>();
        //image.DOKill();
        //image.DOColor(new Color(1, 1, 1, m_alpha), 0.5f);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
       

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }


    private bool hasIndex = false;
    public void OnFingerUp(FingerEventData eventData, FingerEventData submitFinger)
    {
        m_fingers--;

        if (hasIndex && eventData.finger.Type == FingerType.Index)
            hasIndex = false;

        if (m_fingers == 0 && dragging)
        {
            EndDrag();
        }
    }

    public void OnFingerDown(FingerEventData eventData, FingerEventData submitFinger)
    {
        m_fingers++; 

        if (!hasIndex && eventData.finger.Type == FingerType.Index)
            hasIndex = true;
        
        Debug.LogFormat("{0} ENTER -> {1}", eventData.finger.Type, m_fingers);
        

        if (m_fingers >= 3 && !submitFinger.occupied
            && hasIndex)
        {
            BeginDrag();
        }
    }
}
