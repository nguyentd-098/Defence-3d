using UnityEngine;
using System.Collections.Generic;

public class MapGenerator
{
    public MapData Generate(int width, int height)
    {
        MapData map = new MapData(width, height);

        // 🎯 PATH
        List<Vector2Int> basePath = GeneratePath(width, height);
        HashSet<Vector2Int> path = ExpandPath(basePath);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                TileData tile;

                // 🚧 PATH ƯU TIÊN
                if (path.Contains(pos))
                {
                    tile = new TileData(TileType.Road);
                    tile.height = 0;
                    map.tiles[x, y] = tile;
                    continue;
                }

                // 🌊 HEIGHT (multi noise → có sóng)
                float noise1 = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                float noise2 = Mathf.PerlinNoise(x * 0.15f + 50, y * 0.15f + 50);
                float heightNoise = noise1 * 0.7f + noise2 * 0.3f;

                int baseHeight = Mathf.FloorToInt(heightNoise * 2);

                // 🏔️ MOUNTAIN CLUSTER
                float mountainNoise = Mathf.PerlinNoise(x * 0.08f + 200, y * 0.08f + 200);

                // 🌱 BIOME
                float biome = Mathf.PerlinNoise(x * 0.1f + 100, y * 0.1f + 100);

                // 🌊 WATER
                if (biome < 0.28f)
                {
                    tile = new TileData(TileType.Water);
                    tile.height = 0;
                    map.tiles[x, y] = tile;
                    continue;
                }

                // 🌱 GRASS
                tile = new TileData(TileType.Grass);

                // 🏔️ TẠO CỤM NÚI
                if (mountainNoise > 0.6f)
                {
                    float t = (mountainNoise - 0.6f) / 0.4f;
                    int mountainHeight = Mathf.RoundToInt(t * 3);
                    tile.height = baseHeight + mountainHeight;
                }
                else
                {
                    tile.height = baseHeight;
                }

                // ❌ DỌN GẦN ĐƯỜNG (rất quan trọng)
                if (IsNearPath(pos, path))
                {
                    tile.hasTree = false;
                    tile.hasRock = false;
                }
                else
                {
                    // 🌲 TREE
                    if (biome > 0.6f && tile.height < 2)
                        tile.hasTree = true;

                    // 🪨 ROCK
                    else if (biome < 0.4f && tile.height < 2)
                        tile.hasRock = true;
                }

                map.tiles[x, y] = tile;
            }
        }

        return map;
    }

    // ================= PATH =================

    List<Vector2Int> GeneratePath(int width, int height)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        // choose random start/end rows (avoid edges)
        int margin = 2;
        int startY = Random.Range(margin, Mathf.Max(margin + 1, height - margin));
        int endY = Random.Range(margin, Mathf.Max(margin + 1, height - margin));

        // start at left edge, end at right edge (randomized vertical positions)
        List<Vector2Int> points = new List<Vector2Int>();
        points.Add(new Vector2Int(0, startY));

        int segments = Random.Range(4, 8);
        for (int i = 1; i < segments; i++)
        {
            // ensure x increases so the path flows left->right
            int x = (width * i) / segments;
            int y = Random.Range(margin, Mathf.Max(margin + 1, height - margin));
            points.Add(new Vector2Int(x, y));
        }

        points.Add(new Vector2Int(width - 1, endY));

        for (int i = 0; i < points.Count - 1; i++)
        {
            var segment = DrawLine(points[i], points[i + 1]);
            path.AddRange(segment);
        }

        return path;
    }
    List<Vector2Int> DrawLine(Vector2Int a, Vector2Int b)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int dx = Mathf.Abs(b.x - a.x);
        int dy = Mathf.Abs(b.y - a.y);
        int sx = a.x < b.x ? 1 : -1;
        int sy = a.y < b.y ? 1 : -1;
        int err = dx - dy;
        Vector2Int current = a;
        while (true)
        {
            result.Add(current);
            if (current == b)
                break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                current.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                current.y += sy;
            }
        }
        return result;
    }

    // ================= PATH WIDTH =================

    HashSet<Vector2Int> ExpandPath(List<Vector2Int> path)
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();

        foreach (var p in path)
        {
            result.Add(p);
            result.Add(new Vector2Int(p.x, p.y + 1));
            result.Add(new Vector2Int(p.x, p.y - 1));
        }

        return result;
    }

    // ================= CLEAR AREA =================

    bool IsNearPath(Vector2Int pos, HashSet<Vector2Int> path)
    {
        foreach (var p in path)
        {
            if (Vector2Int.Distance(p, pos) <= 2)
                return true;
        }
        return false;
    }
}