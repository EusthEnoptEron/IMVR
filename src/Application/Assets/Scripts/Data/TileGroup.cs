using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class TileGroup : Tile {

    protected Text text;

    protected virtual void Awake()
    {
        text = new GameObject().AddComponent<Text>();
        text.font = ResourceManager.Roboto;
        text.alignment = TextAnchor.MiddleCenter;
        text.transform.SetParent(transform);

        // Configure text
        text.transform.localPosition = new Vector3(0, -Tile.PIXELS_PER_UNIT, 0);
        text.transform.localRotation = Quaternion.Euler(60, 0, 0);
        text.fontSize = 25;

        SetRaycaster(false);

    }
    public void SetRaycaster(bool active)
    {
        GetComponent<GraphicRaycaster>().enabled = active;
    }

    public virtual string Label
    {
        get
        {
            return text.text;
        }
        set
        {
            gameObject.name = value;
            text.text = value;
        }
    }

    public virtual void SetElements(List<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            obj.transform.SetParent(transform, false);
        }
    }


    protected override void OnDestroy()
    {
    }
}
