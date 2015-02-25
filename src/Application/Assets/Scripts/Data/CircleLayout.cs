using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class CircleLayout : MonoBehaviour {
    public List<Tile> tiles = new List<Tile>();
    public int tileCountVertical = 3;
    private bool changing;

    void Update()
    {
        if (changing) return;

        UpdatePositions(Time.deltaTime);
    }

    void UpdatePositions(float progress)
    {
        // update tile positions
        int tileCount = Mathf.Min(1000, tiles.Count);
        int imagesPerRevolution = tileCount / tileCountVertical;
        float radius = imagesPerRevolution / (Mathf.PI * 2);

        int i = 0;
        for (; i < tileCount; i++)
        {
            tiles[i].gameObject.SetActive(true);

            Vector2 pos = new Vector2(((i) / tileCountVertical) * (360f / imagesPerRevolution) * Mathf.PI / 180f, (i) % tileCountVertical);

            if (pos.x >= 2 * Mathf.PI) return;

            tiles[i].transform.parent = transform;
            var endPosition = new Vector3(Mathf.Cos(pos.x) * radius, pos.y, Mathf.Sin(pos.x) * radius);
            var endRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-endPosition, transform.up).normalized);

            tiles[i].transform.localPosition = Vector3.Lerp(tiles[i].transform.localPosition, endPosition, progress);
            tiles[i].transform.localRotation = Quaternion.Slerp(tiles[i].transform.localRotation, endRotation, progress);
            tiles[i].transform.localScale = Vector3.one;
        }

        for (; i < tiles.Count; i++)
        {
            tiles[i].gameObject.SetActive(false);
            tiles[i].transform.parent = transform;

        }
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