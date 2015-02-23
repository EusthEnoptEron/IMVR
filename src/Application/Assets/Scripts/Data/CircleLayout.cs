using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleLayout : MonoBehaviour {
    public List<Tile> tiles = new List<Tile>();
    public int tileCountVertical = 3;

    void Update()
    {
        // update tile positions
        int tileCount = tiles.Count;
        int imagesPerRevolution = tileCount / tileCountVertical;
        float radius = imagesPerRevolution / (Mathf.PI * 2);

        for (int i = 0; i < tiles.Count; i++) 
        {
            Vector2 pos = new Vector2(((i) / tileCountVertical) * (360f / imagesPerRevolution) * Mathf.PI / 180f, (i) % tileCountVertical);

            if (pos.x >= 2 * Mathf.PI) return;

            tiles[i].transform.parent = transform;
            var endPosition = new Vector3(Mathf.Cos(pos.x) * radius, pos.y, Mathf.Sin(pos.x) * radius);
            var endRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-endPosition, transform.up).normalized);

            tiles[i].transform.localPosition = Vector3.Lerp(tiles[i].transform.localPosition, endPosition, Time.deltaTime);
            tiles[i].transform.localRotation = Quaternion.Slerp(tiles[i].transform.localRotation, endRotation, Time.deltaTime);
        }
    }

    void LateUpdate() {

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                tiles.Shuffle();
            }
            else { 
                tiles.Reverse();
            }
        }
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