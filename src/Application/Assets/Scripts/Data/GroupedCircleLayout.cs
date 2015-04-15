using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GroupedCircleLayout : CircleLayout {
    private List<Tile> _tiles;

    [HideInInspector]
    public override List<Tile> tiles
    {
        get { return _tiles; }
        set {
            transform.DetachChildren();

            _tiles = value;

            foreach (var tile in _tiles)
                tile.transform.SetParent(transform, false);
        }
    }

    private Dictionary<string, List<GameObject>> _items = new Dictionary<string, List<GameObject>>();

    public Dictionary<string, List<GameObject>> Items { 
        get {
            return _items;
         }
        set
        {
            _items = value;
            CreateGroups();
        }
    }

    protected void Awake()
    {
        tiles = new List<Tile>();
    }

    protected override void Start()
    {
        autoLayout = false;
        height = 1;
        base.Start();
    }


    private void CreateGroups()
    {
        // Set segments
        xSegments = _items.Keys.Count;
        ySegments = 1;
        tileScale = radius * 2 * Mathf.Tan(Mathf.PI / xSegments);

        // Create Objects        
        var groups = new List<Tile>();
        foreach(var pair in Items.OrderBy(kv => kv.Key))
        {
            var group = new GameObject().AddComponent<LayoutGroup>();
            groups.Add(group);

            group.Label = pair.Key;
            group.SetElements(pair.Value);
        }

        tiles = groups;
        
    }

    public override GameObject GetTileAtPosition(Vector3 pos)
    {
        //return null;
        var group = base.GetTileAtPosition(pos);
        return group;
    }


}
