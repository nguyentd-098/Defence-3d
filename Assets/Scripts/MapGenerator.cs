using UnityEngine;
using System.Collections.Generic;

public class MapGenerator
{
    public MapData Generate(int width, int height)
    {
        MapData map = new MapData(width, height);

        List<Vector2Int> path = GeneratePath(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (path.Contains(pos))
                {
                    map.tiles[x, y] = new TileData(TileType.Road);
                }
                else
                {
                    float biome = Mathf.PerlinNoise(x * 0.08f, y * 0.08f);

                    TileData tile;

                    if (biome < 0.25f)
                    {
                        tile = new TileData(TileType.Water);
                    }
                    else
                    {
                        tile = new TileData(TileType.Grass);
                        if (biome > 0.45f)
                            tile.hasTree = true;
                        if (biome < 0.5f)
                            tile.hasRock = true;
                    }

                    map.tiles[x, y] = tile;
                }
            }
        }

        return map;
    }

    List<Vector2Int> GeneratePath(int width, int height)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        Vector2Int current = new Vector2Int(0, height / 2);
        Vector2Int end = new Vector2Int(width - 1, height / 2);

        path.Add(current);

        while (current != end)
        {
            if (Random.value < 0.7f && current.x < end.x)
                current.x++;
            else
                current.y += Random.Range(-1, 2);

            current.y = Mathf.Clamp(current.y, 0, height - 1);

            path.Add(current);
        }

        return path;
    }
}