using UnityEngine;
using System.Collections;
using IMVR.Commons;
using System.Collections.Generic;
using VirtualHands.Data.Image;
using System.Linq;

public class ImageAtlas {
    public static bool IsLoading
    {
        get
        {
            return Progress != 1;
        }
    }

    public static float Progress
    {
        get
        {
            return totalJobs == 0
                ? 1
                : (float)(totalJobs-runningJobs) / totalJobs;
        }
    }

    private static Dictionary<string, ImageAtlas> _atlasDictionary = new Dictionary<string,ImageAtlas>();
    
    public string Path { get; private set; }
    public int TileSize { get; private set; }

    private Texture2D texture;
    private int tilesPerRow;

    private static int totalJobs = 0;
    private static int runningJobs = 0;

    public ImageAtlas(Atlas atlas)
    {
        Path = atlas.Path;
        TileSize = atlas.TileSize;

        // Load sprites
        texture = DeferredLoader.Instance.LoadTexture(
            System.IO.Path.Combine(System.IO.Path.GetTempPath(), 
            System.IO.Path.Combine("IMVR", Path)),
            delegate {
                runningJobs--;
            }
        );

        // Initialize if need be
        if (runningJobs == 0)
            totalJobs = 0;

        runningJobs++;
        totalJobs++;

        //Debug.Log(System.IO.Path.Combine(System.IO.Path.GetTempPath(),
        //    System.IO.Path.Combine("IMVR", Path)));
        tilesPerRow = texture.width / TileSize;
    }

    public Sprite GetSprite(AtlasTicket ticket)
    {
        return GetSpriteAt(ticket.Position);
    }

    public Sprite GetSpriteAt(int i)
    {
        // ORIGIN: bottom left
        /*
         |
         |
         |
         -------------> x
         |
         |
         |
         v
         y 
         
         
         HOWEVER: sprites are defined with an origin at top left!
         */


        int x = (i % tilesPerRow) * TileSize;
        int y = texture.height - TileSize - (i / tilesPerRow) * TileSize;

        //Debug.LogFormat("{0} => {1}|{2} ({3})", i, x, y, Path);

        return Sprite.Create(
            texture,
            new Rect(x, y, TileSize, TileSize),
            new Vector2(0.5f, 0.5f),
            TileSize / 100,
            0,
            SpriteMeshType.FullRect
        );
    }


    public static Sprite LoadSprite(AtlasTicket ticket)
    {
        if (!_atlasDictionary.ContainsKey(ticket.Atlas.Path))
        {
            _atlasDictionary.Add(ticket.Atlas.Path, new ImageAtlas(ticket.Atlas));
        }

        return _atlasDictionary[ticket.Atlas.Path].GetSprite(ticket);
    }


}
