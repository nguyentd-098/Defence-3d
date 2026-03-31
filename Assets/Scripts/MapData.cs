using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public int width;
    public int height;
    public TileData[,] tiles;
    public int seed;
    public List<Vector2Int> roadPath; // <-- Add this line

    public MapData(int w, int h, int seed = 0)
    {
        width = w;
        height = h;
        tiles = new TileData[w, h];
        this.seed = seed;
    }

    // Tiện ích kiểm tra bounds
    public bool InBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    public TileData Get(int x, int y) => InBounds(x, y) ? tiles[x, y] : null;
}