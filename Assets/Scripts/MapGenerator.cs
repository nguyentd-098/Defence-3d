using UnityEngine;
using System.Collections.Generic;

public class MapGenerator
{
    public int seed = 0;
    [Range(0f, 1f)] public float forestThreshold = 0.60f;
    [Range(0f, 1f)] public float mountainThreshold = 0.65f;
    public float mountainFreq = 0.08f;
    public float biomeFreq = 0.10f;

    // Các hướng di chuyển cho hệ tọa độ Odd-r
    public static readonly Vector2Int[][] HexDirections = new Vector2Int[][]
    {
        new Vector2Int[] { new Vector2Int(+1,  0), new Vector2Int( 0, -1), new Vector2Int(-1, -1), new Vector2Int(-1,  0), new Vector2Int(-1, +1), new Vector2Int( 0, +1) },
        new Vector2Int[] { new Vector2Int(+1,  0), new Vector2Int(+1, -1), new Vector2Int( 0, -1), new Vector2Int(-1,  0), new Vector2Int( 0, +1), new Vector2Int(+1, +1) }
    };

    public MapData Generate(int width, int height)
    {
        int _seed = (seed == 0) ? Random.Range(1, 999999) : seed;
        Random.InitState(_seed);
        float _offsetMtn = (_seed / 7f) % 1000 + 200f;
        float _offsetBiome = (_seed / 3f) % 1000 + 100f;

        MapData map = new MapData(width, height, _seed);

        // 1. CHỈ TẠO ĐỊA HÌNH CƠ BẢN VÀ TRANG TRÍ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float mtnNoise = Mathf.PerlinNoise(x * mountainFreq + _offsetMtn, y * mountainFreq + _offsetMtn);
                TileData tile = new TileData(TileType.Grass, BiomeType.Plain) { height = 0 };

                bool isBorder = (x <= 1 || x >= width - 2 || y <= 1 || y >= height - 2);

                if (isBorder || mtnNoise > mountainThreshold)
                {
                    tile.type = TileType.Mountain;
                    tile.biome = BiomeType.Highland;
                    tile.height = isBorder ? Random.Range(3, 6) : Random.Range(1, 4);
                }
                else
                {
                    // Đất bằng phẳng thì sinh cỏ cây
                    float biomeNoise = Mathf.PerlinNoise(x * biomeFreq + _offsetBiome, y * biomeFreq + _offsetBiome);
                    if (biomeNoise > forestThreshold)
                    {
                        tile.biome = BiomeType.Forest;
                        tile.hasTree = true;
                    }
                    else if (biomeNoise < 0.25f)
                    {
                        tile.biome = BiomeType.Rocky;
                        tile.hasRock = true;
                    }
                }
                map.tiles[x, y] = tile;
            }
        }

        // Trả về map hoàn toàn tự nhiên, không có đường đi cho sẵn
        return map;
    }

    // ──────────────────────────────────────────
    // 2. HÀM TÌM ĐƯỜNG ĐỘNG (Dùng bất cứ lúc nào)
    // ──────────────────────────────────────────
    public List<Vector2Int> FindPath(MapData map, Vector2Int start, Vector2Int target)
    {
        var openSet = new List<Vector2Int> { start };
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, float> { [start] = 0 };
        var fScore = new Dictionary<Vector2Int, float> { [start] = HexDistance(start, target) };

        int emergencyBreak = 0;

        while (openSet.Count > 0)
        {
            emergencyBreak++;
            if (emergencyBreak > 5000) break;

            openSet.Sort((a, b) => fScore.GetValueOrDefault(a, float.MaxValue).CompareTo(fScore.GetValueOrDefault(b, float.MaxValue)));
            Vector2Int current = openSet[0];

            if (current == target) return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (Vector2Int neighbor in GetNeighbors(current, map.width, map.height))
            {
                // Điều kiện đi được: Cột mốc không phải núi cao (hoặc không có tháp chắn của Player)
                TileData nTile = map.tiles[neighbor.x, neighbor.y];
                if (nTile.height > 1 && neighbor != target) continue;

                // Có thể cộng thêm "cost" nếu đi qua rừng cây thì chậm hơn (vd: terrainCost = 5)
                float terrainCost = nTile.hasTree ? 2f : 1f;
                float tentative_gScore = gScore[current] + terrainCost;

                if (tentative_gScore < gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = gScore[neighbor] + HexDistance(neighbor, target);

                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
        return null; // Không tìm thấy đường
    }

    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    List<Vector2Int> GetNeighbors(Vector2Int hex, int width, int height)
    {
        var neighbors = new List<Vector2Int>();
        int parity = hex.y & 1;
        foreach (var dir in HexDirections[parity])
        {
            int nx = hex.x + dir.x;
            int ny = hex.y + dir.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                neighbors.Add(new Vector2Int(nx, ny));
        }
        return neighbors;
    }

    float HexDistance(Vector2Int a, Vector2Int b)
    {
        float qa = a.x - (a.y - (a.y & 1)) / 2f;
        float qb = b.x - (b.y - (b.y & 1)) / 2f;
        Vector3 aCube = new Vector3(qa, -qa - a.y, a.y);
        Vector3 bCube = new Vector3(qb, -qb - b.y, b.y);
        return Mathf.Max(Mathf.Abs(aCube.x - bCube.x), Mathf.Abs(aCube.y - bCube.y), Mathf.Abs(aCube.z - bCube.z));
    }
}