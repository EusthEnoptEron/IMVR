using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(TileBlanket))]
public class CircleLayout : MonoBehaviour {
    [HideInInspector]
    public List<Tile> tiles {
        get {
            return blanket.tiles;
        }
        set {
            blanket.SetTiles(value);
            UpdateLayout();
        }
    }

    public Tile[,] tileMat;


    public float height = 5;
    public float radius = 5;

    private bool changing;

    public int xSegments { get; private set; }
    public int ySegments { get; private set; }
    public float tileScale { get; private set; }

    private TileBlanket blanket;

    private Transform world;
    protected virtual void Start() {
        blanket = GetComponent<TileBlanket>();
        world = GameObject.Find("World").transform;
    }

    void Update()
    {
        // Annetuation
        //v = Mathf.MoveTowards(v, 0, Time.deltaTime);
        v *= (1 - Time.deltaTime);

        if (Mathf.Abs(v) > 0)
            world.localRotation *= Quaternion.Euler(0, -v, 0);

        if (changing) return;
        //Debug.Log("iüdate");
        UpdatePositions(Time.deltaTime * 5);
    }

    public Tile GetTileAtPosition(Vector3 pos)
    {
        var locP = transform.InverseTransformPoint(pos);

        // WORKAROUND
        //pos = (world.localRotation) * pos;
        // / WORKAROUND
        
        int y = Mathf.RoundToInt((locP.y + height / 2) / tileScale);
        int x = Mathf.RoundToInt(
            Mathf.Atan2(pos.z, pos.x) / (2 * Mathf.PI) * xSegments
        );

        x = (x + xSegments) % xSegments;

        if (y < tileMat.GetLength(0) && x < tileMat.GetLength(1) && y >= 0 && x >= 0)
            return tileMat[y, x];
        else
            return null;
    }

    struct BestResult { public int sy; public int sx; public float scale; }

    void UpdatePositions(float progress)
    {
        tileMat = new Tile[ySegments, xSegments];

        //return;

        //float area = 2 * Mathf.PI * radius * height * 0.5f;
        //float tileScale = area / (tileCount / 4f);

        //Debug.LogFormat("{0} {1} {2} {3}", area, radius, height, tileScale);

        int rowsPerRevolution = Mathf.Max(1, xSegments);
        int tileCount = tiles.Count;

        //float radius = imagesPerRevolution / (Mathf.PI * 2);
        float ySegmentsF = (float)ySegments;
        float indexToLatitude = (2f / rowsPerRevolution) * Mathf.PI;
        float scale = tileScale / 100f;

        int i = 0;
        for (; i < tileCount; i++)
        {
            //tiles[i].transform.SetParent(transform);
            int x = (i / ySegments);
            int y = (i % ySegments);
   
            tileMat[y, x] = tiles[i];

            float latitude = x * indexToLatitude;
            float verticalProgress = y / ySegmentsF;

            if (latitude >= 2 * Mathf.PI)
            {
                if(tiles[i].gameObject.activeSelf)
                    tiles[i].gameObject.SetActive(false);
            }
            else
            {
                if (!tiles[i].gameObject.activeSelf)
                    tiles[i].gameObject.SetActive(true);

                var endPosition = new Vector3(Mathf.Cos(latitude) * radius, 
                                              verticalProgress * height - height / 2,
                                              Mathf.Sin(latitude) * radius);
                var endRotation = Quaternion.LookRotation(-Vector3.ProjectOnPlane(-endPosition, transform.up).normalized);

                tiles[i].targetPosition = endPosition;
                tiles[i].targetRotation = endRotation;
                tiles[i].targetScale = tileScale * Vector3.one;

                //tiles[i].transform.DOLocalMove(endPosition, 0.5f);
                //tiles[i].transform.DOLocalRotate(endRotation.eulerAngles, 0.5f);
                //tiles[i].transform.DOScale(tileScale * Vector3.one, 0.5f);

                tiles[i].transform.localPosition = Vector3.Lerp(tiles[i].transform.localPosition, endPosition, progress);
                tiles[i].transform.localRotation = Quaternion.Slerp(tiles[i].transform.localRotation, endRotation, progress);
                tiles[i].transform.localScale = Vector3.one * scale;
            }
        }
    }

    void UpdateLayout() {

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

        UpdatePositions(1);
    }

    void LateUpdate() {
        if (changing) return;

        bool transition = false;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                tiles.Shuffle();
            }
            else
            { 
                tiles.Reverse();
            }
            transition = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            tiles.Sort((x, y) => x.File.ImageStatistics.First().Saturation.Value.CompareTo(y.File.ImageStatistics.First().Saturation.Value));
            transition = true;
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            tiles.Sort((x, y) => x.File.ImageStatistics.First().Hue.Value.CompareTo(y.File.ImageStatistics.First().Hue.Value));
            transition = true;
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            tiles.Sort((x, y) => x.File.ImageStatistics.First().Lightness.Value.CompareTo(y.File.ImageStatistics.First().Lightness.Value));
            transition = true;

        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            tiles.Sort((x, y) => x.File.ImageStatistics.First().Variance.Value.CompareTo(y.File.ImageStatistics.First().Variance.Value));
            transition = true;
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            tiles.Sort((x, y) => x.File.ImageStatistics.First().Mean.Value.CompareTo(y.File.ImageStatistics.First().Mean.Value));
            transition = true;

        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            tiles.Sort((x, y) => x.File.ImageStatistics.First().Entropy.Value.CompareTo(y.File.ImageStatistics.First().Entropy.Value));
            transition = true;
        }
        if (transition)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                StartCoroutine(SwapTiles());
            }
        }
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