using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GroupedCircleLayout<T> : CylinderLayout where T : TileGroup {
    private List<Tile> _tiles;
    
    //[HideInInspector]
    //protected override List<Tile> tiles
    //{
    //    get { return _tiles; }
    //    set {
    //        transform.DetachChildren();

    //        _tiles = value;

    //        foreach (var tile in _tiles)
    //            tile.transform.SetParent(transform, false);

    //        BuildTileMatrix();
    //    }
    //}

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
        tiles = new List<GameObject>();
        autoLayout = false;

    }

    protected override void Start()
    {
        height = 1;
        scale = 1f / Tile.PIXELS_PER_UNIT;
        base.Start();
    }


    private void CreateGroups()
    {
        // Set segments
        xSegments = _items.Keys.Count;
        ySegments = 1;
        tileScale = radius * 2 * Mathf.Tan(Mathf.PI / xSegments);

        // Create Objects        
        var groups = new List<GameObject>();
        foreach(var pair in Items.OrderBy(kv => kv.Key))
        {
            var group = new GameObject().AddComponent<T>();
            groups.Add(group.gameObject);
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


public class SimpleGroupedCircleLayout : GroupedCircleLayout<TileGroup>
{

}

public class ListGroupedCircleLayout : GroupedCircleLayout<LayoutGroup>
{

}