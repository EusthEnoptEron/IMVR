using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TileBlanket : MonoBehaviour {
    public int tilesPerCanvas = 256;
    public List<Tile> tiles = new List<Tile>();
    private List<Canvas> canvasList = new List<Canvas>();

    public void SetTiles(List<Tile> tiles)
    {
        foreach (var canvas in canvasList)
        {
            Destroy(canvas.gameObject);
        }

        this.tiles = tiles;

        CreateCanvas();

        int i = 0;
        foreach (var tile in tiles)
        {
            var canvas = ChooseCanvas(i++, tile);
            tile.transform.SetParent(canvas.transform);
        }
    }

    protected virtual Canvas ChooseCanvas(int no, Tile tile)
    {
        var canvasNo = no / tilesPerCanvas;
        return canvasList[canvasNo];
    }

    protected virtual void CreateCanvas()
    {
        int count = Mathf.CeilToInt(tiles.Count() / (float)tilesPerCanvas);

        for (int i = 0; i < count; i++)
        {
            var canvas = new GameObject().AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.SetParent(transform, false);
            //canvas.transform.localPosition = Vector3.zero;
            //canvas.transform.localRotation = Quaternion.identity;
            //canvas.transform.localScale = Vector3.one;

            canvasList.Add(canvas);
        }
    }
}
