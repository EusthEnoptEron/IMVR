using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class CircleLayout : MonoBehaviour {

    private List<Tile> _tiles = new List<Tile>();

    [HideInInspector]
    public List<Tile> tiles {
        get {
            return _tiles;
        }
        set {
            _tiles = value;
            UpdateLayout();
        }
    }

    public float height = 5;
    public float radius = 5;

    private bool changing;

    private int xSegments = 1;
    private int ySegments = 1;
    private float tileScale = 1;


    void Update()
    {
        if (changing) return;
        //Debug.Log("iüdate");
        UpdatePositions(Time.deltaTime * 5);
    }

    struct BestResult { public int sy; public int sx; public float scale; }

    void UpdatePositions(float progress)
    {
        //return;

        //float area = 2 * Mathf.PI * radius * height * 0.5f;
        //float tileScale = area / (tileCount / 4f);

        int tileCountVertical = ySegments;
        //Debug.LogFormat("{0} {1} {2} {3}", area, radius, height, tileScale);

        int rowsPerRevolution = Mathf.Max(1, xSegments);
        int tileCount = tiles.Count;

        //float radius = imagesPerRevolution / (Mathf.PI * 2);


        int i = 0;
        for (; i < tileCount; i++)
        {
            tiles[i].transform.SetParent(transform);

            Vector2 pos = new Vector2(
                (i / tileCountVertical) * (2f / rowsPerRevolution) * Mathf.PI,  //  latitude
                (i % tileCountVertical) / (float)tileCountVertical); // # of vertical position

            if (pos.x >= 2 * Mathf.PI)
            {
                tiles[i].gameObject.SetActive(false);
            }
            else
            {
                tiles[i].gameObject.SetActive(true);

                var endPosition = new Vector3(Mathf.Cos(pos.x) * radius, pos.y * height - height / 2, Mathf.Sin(pos.x) * radius);
                var endRotation = Quaternion.LookRotation(-Vector3.ProjectOnPlane(-endPosition, transform.up).normalized);

                tiles[i].targetPosition = endPosition;
                tiles[i].targetRotation = endRotation;
                tiles[i].targetScale = tileScale * Vector3.one;
                //tiles[i].transform.DOLocalMove(endPosition, 0.5f);
                //tiles[i].transform.DOLocalRotate(endRotation.eulerAngles, 0.5f);
                //tiles[i].transform.DOScale(tileScale * Vector3.one, 0.5f);

                tiles[i].transform.localPosition = Vector3.Lerp(tiles[i].transform.localPosition, endPosition, progress);
                tiles[i].transform.localRotation = Quaternion.Slerp(tiles[i].transform.localRotation, endRotation, progress);
                tiles[i].transform.localScale = Vector3.one * tileScale;
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
                Mathf.Abs(tileCount - (sy * sx)))
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