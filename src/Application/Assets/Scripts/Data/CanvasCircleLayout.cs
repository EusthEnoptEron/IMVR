using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasCircleLayout : CylinderLayout
{
    protected override void Start()
    {
        autoLayout = false;
        base.Start();
    }

    /// <summary>
    /// Resizes the internal matrix. This action will remove all tiles.
    /// </summary>
    /// <param name="xTiles"></param>
    /// <param name="yTiles"></param>
    public void Resize(int xTiles, int yTiles)
    {
        ySegments = yTiles;
        xSegments = xTiles;

        tileMat = new GameObject[yTiles, xTiles];
        
        for (int y = 0; y < yTiles; y++)
        {
            for (int x = 0; x < xTiles; x++)
            {
                var rect = new GameObject().AddComponent<RectTransform>();
                tileMat[y, x] = rect.gameObject;
                rect.transform.SetParent(transform, false);
                //rect.sizeDelta = new Vector2(1 / scale * sideWidth , 1 / scale * tileHeight);
            }
        }
    }

    public void SetTile(int x, int y, GameObject obj)
    {
        obj.transform.SetParent(tileMat[y, x].transform, false);
        _dirty = true;
    }

    protected override void Update()
    {
        base.Update();

        float sideWidth = radius * 2 * Mathf.Tan(Mathf.PI / xSegments);
        float tileHeight = height / ySegments;
        for (int y = 0; y < ySegments; y++)
        {
            for (int x = 0; x < xSegments; x++)
            {
                //var rect = new GameObject().AddComponent<RectTransform>();
                //tileMat[y, x] = rect.gameObject;
                //rect.transform.SetParent(transform, false);
                tileMat[y,x].GetComponent<RectTransform>().sizeDelta = new Vector2(1 / scale * sideWidth, 1 / scale * tileHeight);
            }
        }
    }

}
