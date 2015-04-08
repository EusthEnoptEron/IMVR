using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IMVR.Commons;
using System.IO;
using System.Linq;

public class DummyTileFeeder : MonoBehaviour {
    public int tileCount = 1000;


	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForSeconds(2);
	    // Create tiles

        List<Tile> tiles = new List<Tile>();
        CircleLayout layout = GetComponent<CircleLayout>();

        var db = IMDB.FromFile(
            Path.Combine(Application.dataPath, PlayerPrefs.GetString("DBPath"))
        );


        Debug.Log(Path.Combine(Application.dataPath, PlayerPrefs.GetString("DBPath")));

        foreach (var image in db.Images)
        {
            var tile = new GameObject().AddComponent<ImageTile>();
            tile.Image = image;
            tiles.Add(tile);
        }
        //yield return new WaitForSeconds(6);

        //foreach (var tile in tiles.Cast<ImageTile>())
        //{
        //    tile.Image = tile.Image;
        //}

        //for (int i = 0; i < tileCount; i++)
        //{
        //    var tile = new GameObject().AddComponent<DummyTile>();
        //    tile.Color = HSVToRGB( ((float)i / tileCount) * 1, 0.5f, 0.5f);

        //    tiles.Add(tile);
        //}

        layout.tiles = tiles;
	}
	

    public static Color HSVToRGB(float H, float S, float V)
     {
         if (S == 0f)
             return new Color(V,V,V);
         else if (V == 0f)
             return Color.black;
         else
         {
             Color col = Color.black;
             float Hval = H * 6f;
             int sel = Mathf.FloorToInt(Hval);
             float mod = Hval - sel;
             float v1 = V * (1f - S);
             float v2 = V * (1f - S * mod);
             float v3 = V * (1f - S * (1f - mod));
             switch (sel + 1)
             {
             case 0:
                 col.r = V;
                 col.g = v1;
                 col.b = v2;
                 break;
             case 1:
                 col.r = V;
                 col.g = v3;
                 col.b = v1;
                 break;
             case 2:
                 col.r = v2;
                 col.g = V;
                 col.b = v1;
                 break;
             case 3:
                 col.r = v1;
                 col.g = V;
                 col.b = v3;
                 break;
             case 4:
                 col.r = v1;
                 col.g = v2;
                 col.b = V;
                 break;
             case 5:
                 col.r = v3;
                 col.g = v1;
                 col.b = V;
                 break;
             case 6:
                 col.r = V;
                 col.g = v1;
                 col.b = v2;
                 break;
             case 7:
                 col.r = V;
                 col.g = v3;
                 col.b = v1;
                 break;
             }
             col.r = Mathf.Clamp(col.r, 0f, 1f);
             col.g = Mathf.Clamp(col.g, 0f, 1f);
             col.b = Mathf.Clamp(col.b, 0f, 1f);
             return col;
         }
     }

}
