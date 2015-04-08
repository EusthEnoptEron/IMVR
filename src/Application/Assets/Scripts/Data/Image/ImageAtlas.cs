﻿using UnityEngine;
using System.Collections;
using IMVR.Commons;
using System.Collections.Generic;
using VirtualHands.Data.Image;

public class ImageAtlas {
    private static Dictionary<string, ImageAtlas> _atlasDictionary = new Dictionary<string,ImageAtlas>();
    
    public string Path { get; private set; }
    public int TileSize { get; private set; }

    private Texture2D texture;
    private int tilesPerRow;

    public ImageAtlas(Atlas atlas)
    {
        Path = atlas.Path;
        TileSize = atlas.TileSize;

        // Load sprites
        texture = DeferredLoader.Instance.LoadTexture(
            System.IO.Path.Combine(System.IO.Path.GetTempPath(), 
            System.IO.Path.Combine("IMVR", Path))
        );
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
        int x = (i % tilesPerRow) * TileSize;
        int y = (i / tilesPerRow) * TileSize;

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