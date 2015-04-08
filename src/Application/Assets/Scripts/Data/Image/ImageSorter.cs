using UnityEngine;
using System.Collections;
using System.Linq;
using IMVR.Commons;

[RequireComponent(typeof(CircleLayout))]
public class ImageSorter : MonoBehaviour {
    private CircleLayout layout;

    public void Start()
    {
        layout = GetComponent<CircleLayout>();
    }

    public void Update()
    {
    }


    public void SortBy(string propertyName) {
        Debug.Log("SORT");
        var property = typeof(Image).GetProperty(propertyName);

        layout.tiles = layout.tiles.OfType<ImageTile>().OrderBy(tile =>
        {
            return property.GetValue(tile.Image, null);
        }).Cast<Tile>().ToList();

    }

}
