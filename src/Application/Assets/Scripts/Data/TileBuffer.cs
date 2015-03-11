using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TileBuffer : IEnumerable<Tile>, IDisposable {
    private List<Tile> tiles;
    private IEnumerable<System.IO.FileInfo> enumerable;

    public TileBuffer(IEnumerable<Tile> tiles)
    {
        this.tiles = tiles.ToList();
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        return tiles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return tiles.GetEnumerator();
    }

    public void Dispose()
    {
        // Destory unused tiles and free their memory
        foreach (var tile in tiles) GameObject.Destroy(tile);
    }

    public Tile this[int index] {
        get
        {
            return tiles[index];
        }
    }
}
