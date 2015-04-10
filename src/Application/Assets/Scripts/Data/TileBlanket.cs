using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TileBlanket : MonoBehaviour {
    public int tilesPerCanvas = 256;
    public List<Tile> tiles = new List<Tile>();
    private List<Canvas> canvasList = new List<Canvas>();
    private GameObject trashBin;

    public void Start()
    {
        trashBin = new GameObject();
        trashBin.transform.SetParent(transform);

    }

    public void SetTiles(List<Tile> tiles)
    {
        foreach (var canvas in canvasList)
        {
            //foreach (var child in canvas.transform.Children())
            //    child.SetParent(trashBin.transform);
            canvas.transform.DetachChildren();
            Destroy(canvas.gameObject, 1);
        }
        canvasList.Clear();
        this.tiles = tiles;

        CreateCanvas();

        int i = 0;
        foreach (var tile in tiles)
        {
            var canvas = ChooseCanvas(i++, tile);
            tile.transform.SetParent(canvas.transform);
            tile.gameObject.layer = gameObject.layer;
        }

        // Clean trash bin
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
            canvas.gameObject.layer = gameObject.layer;
            //canvas.worldCamera = GameObject.FindGameObjectWithTag("ForegroundCamera").GetComponentInChildren<Camera>();
            //canvas.transform.localPosition = Vector3.zero;
            //canvas.transform.localRotation = Quaternion.identity;
            //canvas.transform.localScale = Vector3.one;

            canvasList.Add(canvas);
        }
    }
}
