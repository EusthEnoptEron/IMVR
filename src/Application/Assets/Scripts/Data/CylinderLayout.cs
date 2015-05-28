using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class CylinderLayout : MonoBehaviour {
    private List<GameObject> _tiles = new List<GameObject>();

    [HideInInspector]
    public virtual List<GameObject> tiles {
        get {
            return _tiles;
        }
        protected set {
            transform.DetachChildren();

            _tiles = value.Select<GameObject, GameObject>(Wrap).ToList();

            foreach (var tile in _tiles)
                tile.transform.SetParent(transform, false);

            UpdateLayout();

            BuildTileMatrix();
            UpdatePositions(1);
        }
    }

    public virtual void SetTiles(IEnumerable<GameObject> tiles)
    {
        this.tiles = tiles.ToList();
    }

    public virtual void SetTile(int x, int y, GameObject tile)
    {
        if (tileMat[y, x])
            tileMat[y, x].transform.parent = null;

        tileMat[y, x] = Wrap(tile);
        tileMat[y, x].transform.SetParent(transform, false);

        _dirty = true;
    }

    protected virtual GameObject Wrap(GameObject obj)
    {
        var wrapper = new GameObject();
        obj.transform.SetParent(wrapper.transform, false);
        return wrapper;
    }

    protected GameObject[,] tileMat;

    public bool ignoreHeight = true;
    public bool autoLayout = true;
    public float height = 5;
    public float radius = 5;

    private bool changing;
    protected bool _dirty = false;

    public int xSegments { get; protected set; }
    public int ySegments { get; protected set; }
    public float tileScale { get; protected set; }

    public float scale = 1f;

    private Transform world;
    protected virtual void Start() {
        tileScale = tileScale == 0 ? 1 : tileScale;

        //world = GameObject.FindGameObjectWithTag("ForegroundCamera").transform;
        world = World.WorldNode.transform;

        xSegments = Mathf.Max(1, xSegments);
        ySegments = Mathf.Max(1, ySegments);

        if(tileMat == null)
            tileMat = new GameObject[ySegments, xSegments];
    }

    protected virtual void Update()
    {
        // Annetuation
        //v = Mathf.MoveTowards(v, 0, Time.deltaTime);
        v *= (1 - Time.deltaTime);

        if (Mathf.Abs(v) > 0.0001f)
        {
            World.WorldNode.transform.localRotation *= Quaternion.Euler(0, -v, 0);
        }

        //Debug.Log("iüdate");
        if (_dirty)
            UpdatePositions(1);
        else
            UpdatePositions(Time.deltaTime * 5);

        _dirty = false;
    }


    public virtual GameObject GetTileAtPosition(Vector3 pos)
    {
        var locP = transform.InverseTransformPoint(pos);
        //Debug.Log(locP.x);
        // WORKAROUND
        //pos = (world.localRotation) * pos;
        // / WORKAROUND
        
        int y = Mathf.RoundToInt((locP.y + height / 2) / tileScale);
        int x = Mathf.RoundToInt(
            (Mathf.Atan2(locP.z, -locP.x) - Mathf.PI / 2)  / (2 * Mathf.PI) * xSegments
        );

        x = (x + xSegments) % xSegments;

        if(ignoreHeight) y = Mathf.Clamp(y, 0, tileMat.GetLength(0)-1);

        if (y < tileMat.GetLength(0) && x < tileMat.GetLength(1) && y >= 0 && x >= 0
            && tileMat[y, x] != null)
            return tileMat[y, x].gameObject;
        else
            return null;
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
        tileScale = radius * 2 * Mathf.Tan(Mathf.PI / xTiles);


        //for (int y = 0; y < yTiles; y++)
        //{
        //    for (int x = 0; x < xTiles; x++)
        //    {
        //        var rect = new GameObject().AddComponent<RectTransform>();
        //        tileMat[y, x] = rect.gameObject;
        //        rect.transform.SetParent(transform, false);
        //        //rect.sizeDelta = new Vector2(1 / scale * sideWidth , 1 / scale * tileHeight);
        //    }
        //}
    }
    struct BestResult { public int sy; public int sx; public float scale; }

    public void UpdatePositions(float progress)
    {
        //tileMat = new GameObject[ySegments, xSegments];
        //return;

        //float area = 2 * Mathf.PI * radius * height * 0.5f;
        //float tileScale = area / (tileCount / 4f);

        //Debug.LogFormat("{0} {1} {2} {3}", area, radius, height, tileScale);

        int rowsPerRevolution = Mathf.Max(1, xSegments);
        int tileCount = tiles.Count;

        //float radius = imagesPerRevolution / (Mathf.PI * 2);
        float ySegmentsF = (float)ySegments;
        float indexToLatitude = (2f / rowsPerRevolution) * Mathf.PI;
        float finalScale = tileScale * scale;

        for (int y = 0; y < tileMat.GetLength(0); y++)
        {
            for (int x = 0; x < tileMat.GetLength(1); x++)
            {
                var tile = tileMat[y, x];
                if (tile == null) continue;

                float latitude = (x * indexToLatitude) - Mathf.PI / 2;
                float verticalProgress = y / ySegmentsF;
                if (y == 0 && ySegments == 1) verticalProgress = 0.5f;

                if (latitude >= 2 * Mathf.PI)
                {
                    if (tile.gameObject.activeSelf)
                        tile.gameObject.SetActive(false);
                }
                else
                {
                    if (!tile.gameObject.activeSelf)
                        tile.gameObject.SetActive(true);

                    var endPosition = new Vector3(Mathf.Cos(latitude) * radius,
                                                  verticalProgress * height - height / 2,
                                                  -Mathf.Sin(latitude) * radius);
                    var endRotation = Quaternion.LookRotation(-Vector3.ProjectOnPlane(-endPosition, transform.up).normalized);

                    //tileMat[y, x].targetPosition = endPosition;
                    //tileMat[y, x].targetRotation = endRotation;
                    //tileMat[y, x].targetScale = tileScale * Vector3.one;
                    //tiles[i].transform.DOLocalMove(endPosition, 0.5f);
                    //tiles[i].transform.DOLocalRotate(endRotation.eulerAngles, 0.5f);
                    //tiles[i].transform.DOScale(tileScale * Vector3.one, 0.5f);

                    tile.transform.localPosition = Vector3.Lerp(tile.transform.localPosition, endPosition, progress);
                    tile.transform.localRotation = Quaternion.Slerp(tile.transform.localRotation, endRotation, progress);
                    tile.transform.localScale = Vector3.one * finalScale;
                }
            }
        }
    }


    protected void BuildTileMatrix()
    {
        tileMat = new GameObject[ySegments, xSegments];

        for (int i = 0; i < tiles.Count; i++)
        {
            //tiles[i].transform.SetParent(transform);
            int x = (i / ySegments);
            int y = (i % ySegments);

            tileMat[y, x] = tiles[i].gameObject;
        }
    }

    void UpdateLayout() {
        if (!autoLayout) return;

        // update tile positions
        //int tileCount = Mathf.Min(1000, tiles.Count);

        int tileCount = tiles.Count;
        //float area = 2 * Mathf.PI * radius * height;

        var bestResult = new BestResult { sx = 1, sy = 1, scale = 1 };

        for (int sx = 1; sx < tileCount; sx++)
        {
            int neededSy = tileCount / sx;
            float scale = radius * 2 * Mathf.Tan(Mathf.PI / sx);
            int sy = Mathf.RoundToInt(height / scale);

            //Debug.LogFormat("count: {3}, x={0}, sy={1}, sx={2}", scale, sy, sx, tileCount);

            //Debug.LogFormat("{0} - {1}", sx, scale);
            if (Mathf.Abs(tileCount - (bestResult.sy * bestResult.sx)) >
                Mathf.Abs(tileCount - (sy * sx)) && sy * sx >= tileCount)
            {
                bestResult = new BestResult
                {
                    sx = sx,
                    sy = sy,
                    scale = scale
                };
            }

        }

        Debug.LogFormat("count: {3}, x={0}, sy={1}, sx={2}", bestResult.scale, bestResult.sy, bestResult.sx, tileCount);

        ySegments = bestResult.sy;
        xSegments = bestResult.sx;
        tileScale = bestResult.scale;

    }

    private void Rotate(float from, float to, float duration, Transform transform)
    {
        float currentRotation = from;
        var oldRotation = transform.localRotation;

        DOTween.To(() => currentRotation, y =>
        {
            transform.localRotation = oldRotation * Quaternion.Euler(0, y, 0); currentRotation = y;
        }, to, duration).SetEase(Ease.Linear);
    }

    IEnumerator SwapTiles()
    {
        changing = true;
        var halfRevolution = Quaternion.Euler(0, 180, 0);
        //int currentRotation = 0;


        float duration = 1;

        foreach (var tile in tiles)
        {
            if (tile.gameObject.activeSelf)
            {
                var oldRotation = tile.transform.localRotation;

                //tile.transform.DOLocalRotate((tile.transform.localRotation * halfRevolution).eulerAngles, duration);
                
                Rotate(0, 180, duration, tile.transform);
                
            }
        }
        

        yield return new WaitForSeconds(duration);

        UpdatePositions(1f);

        foreach (var tile in tiles)
        {
            if (tile.gameObject.activeSelf)
            {
                tile.transform.localRotation *= halfRevolution;
                
                Rotate(0, 180, duration, tile.transform);
            }
        }
        

        yield return new WaitForSeconds(duration);

        changing = false;

    }


    private float v = 0;
    public void AddTorque(float N)
    {
        v += N / radius * Time.deltaTime;
    }
}
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;

            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}