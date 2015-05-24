using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class FlatGroupView : View {

    public Dictionary<string, List<GameObject>> items = new Dictionary<string, List<GameObject>>();

    private static GameObject pref_FlatGroupView = Resources.Load<GameObject>("Prefabs/UI/pref_FlatGroupView");
    
    protected Canvas canvas;
    protected UnityEngine.UI.LayoutGroup layout;
    protected float spacePerTile = 1f;
    protected float width = 1f;
    protected float distance = 1f;
    protected float height = -0.5f;
    protected Text titleElement;
    protected Transform layoutElement;

    protected float baseWidth = 5;


    protected override void Awake()
    {
        base.Awake();

        var content = GameObject.Instantiate<GameObject>(pref_FlatGroupView);
        content.transform.SetParent(transform, false);

        titleElement = content.transform.FindChild("Title").GetComponent<Text>();
        layoutElement = content.transform.FindChild("Layout");

        // Make a layout container
        InitLayoutGroup();
        layout = GetComponentInChildren<UnityEngine.UI.LayoutGroup>();


        transform.localScale = Vector3.one * (width / Tile.PIXELS_PER_UNIT / baseWidth);
        transform.localPosition += Vector3.forward * distance;
        transform.localPosition += Vector3.up * height;
    }

    protected virtual void Start()
    {
        CreateGroups();
    }

    protected virtual void CreateGroups() {
        var groups = new List<Tile>();

        foreach (var pair in items.OrderBy(kv => kv.Key))
        {
            var container = new GameObject().AddComponent<RectTransform>();
            var group = new GameObject().AddComponent<TileGroup>();

            groups.Add(group);
            group.Label = pair.Key;
            group.SetElements(pair.Value);

            container.transform.SetParent(layoutElement, false);

            group.transform.SetParent(container.transform, false);
            group.transform.localScale = Vector3.one * ((1f / items.Count()) / spacePerTile) * baseWidth;
        }
    }

    protected virtual void InitLayoutGroup()
    {
        layoutElement.gameObject.AddComponent<HorizontalLayoutGroup>();
    }

        
}
