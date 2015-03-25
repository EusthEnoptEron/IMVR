using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using VirtualHands.Data;
using Foundation;

public class ImageManager : MonoBehaviour {
    ImageSource source;

    public float revolutionTime = 10;

	// Use this for initialization
	void Start () {
        source = new ImageSource(@"C:\Users\meers1\Pictures");
        FillScene();
        //grabber.GrabImages(@"C:\Users\Simon\Pictures");
        //grabber.GrabImages(@"C:");
	}

    void FillScene()
    {
        var tiles = new List<Tile>();

        int height = 3;
        int offset = 0;
        int imageCount = 100;

        using (var ctx = Database.Context)
        {
            var baseTile = new GameObject();
            tiles = (from files in ctx.Files
                     join statistics in ctx.ImageStatistics on files.ID equals statistics.FileID
                     where statistics.Saturation > 0.1
                     select files).ToList()
            .Select(file =>
            {
                var tile = ((GameObject)GameObject.Instantiate(baseTile, Vector3.one, Quaternion.identity)).AddComponent<ImageTile>();
                tile.File = file;

                return (Tile)tile;
            }).ToList();

            ctx.Connection.Close();
        }

        //while (tiles.Count < imageCount)
        //{

        //    yield return 0;
        //    tiles.AddRange(source.ReadForward());
        //    ////yield return new WaitForSeconds(1);

        //    //var buffer = source.ReadForward();
        //    //int imagesPerRevolution = imageCount / height;
        //    //float radius = imagesPerRevolution / (Mathf.PI * 2);

        //    //for (int i = 0; i < buffer.Count(); i++)
        //    //{
        //    //    Vector2 pos = new Vector2(((offset + i) / height) * (360f / imagesPerRevolution) * Mathf.PI / 180f, (offset + i) % height);

        //    //    buffer[i].transform.parent = transform;
        //    //    buffer[i].transform.localPosition = new Vector3(Mathf.Cos(pos.x) * radius, pos.y, Mathf.Sin(pos.x) * radius);
        //    //    buffer[i].transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-buffer[i].transform.position, Vector3.up).normalized); 

        //    //}
        //    //offset += buffer.Count();
        //}

        GetComponent<CircleLayout>().tiles = tiles;
    }

	// Update is called once per frame
	void Update () {
        transform.localRotation *= Quaternion.Euler(0, 360 * Time.deltaTime / revolutionTime, 0);
	}

    void OnApplicationQuit()
    {
        //source.Dispose();
    }

}
