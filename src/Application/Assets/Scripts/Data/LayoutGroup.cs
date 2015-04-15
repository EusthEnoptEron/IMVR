using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class LayoutGroup : Tile, IPointerDownHandler, IDragHandler {
    private VerticalLayoutGroup layout;
    public float heightPerElement = 50;
    private float scrollSpeed = 0;

    private Text text;

	// Use this for initialization
	void Awake () {
        layout = new GameObject().AddComponent<VerticalLayoutGroup>();
        text = new GameObject().AddComponent<Text>();
        text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.alignment = TextAnchor.MiddleCenter;

        layout.transform.SetParent(transform);
        text.transform.SetParent(transform);

        text.transform.localPosition = new Vector3(0, -300, 0);
        text.transform.localRotation = Quaternion.Euler(60, 0, 0);
        text.fontSize = 25;
        //SetRaycaster(false);
	}

    public void SetRaycaster(bool active)
    {
        GetComponent<GraphicRaycaster>().enabled = active;
    }

    public void SetElements(IList<GameObject> gos)
    {
        layout.transform.DetachChildren();
        layout.GetComponent<RectTransform>().sizeDelta = new Vector2(
            100,
            heightPerElement * gos.Count
        );

        foreach (var go in gos)
        {
            go.transform.SetParent(layout.transform);
        }
    }

    protected override void OnDestroy()
    {
    }

    public string Label { 
        get {
            return text.text;
        }
        set
        {
            gameObject.name = value;
            text.text = value;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log(Label);
    }


    public void OnDrag(PointerEventData eventData)
    {
        scrollSpeed += eventData.delta.y;
        //layout.transform.localPosition += Vector3.up * eventData.delta.y;
    }

    void Update()
    {
        if (Mathf.Abs(scrollSpeed) > 0)
        {
            layout.transform.localPosition += Vector3.up * scrollSpeed * Time.deltaTime;
            scrollSpeed = Mathf.MoveTowards(scrollSpeed, 0, 100 * Time.deltaTime);
        }
    }
}
