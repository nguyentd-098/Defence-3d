using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject grassPrefab;
    public GameObject roadPrefab;
    public GameObject waterPrefab;
    public GameObject rockHexPrefab; // 🏔️ hex đá (núi)
    public GameObject cliffPrefab;   // new: vertical face / cliff segment

    [Header("Decoration")]
    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs; // 🪨 đá nhỏ

    [Header("Hex Size")]
    public float hexWidth = 1f;
    public float hexHeight = 0.86f;

    void Start()
    {
        var renderer = grassPrefab.GetComponentInChildren<Renderer>();
        hexWidth = renderer.bounds.size.x;
        hexHeight = renderer.bounds.size.z;
    }

    public void Render(MapData map)
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                TileData tile = map.tiles[x, y];
                Vector3 basePos = GetHexPosition(x, y);

                // compute layer height from rock prefab so stacked rockHex sit flush
                float layerHeight = 1.5f;
                if (rockHexPrefab != null)
                {
                    var r = rockHexPrefab.GetComponentInChildren<Renderer>();
                    if (r != null)
                        layerHeight = r.bounds.size.y;
                }

                // Spawn stacked rock layers when tile has height > 0
                for (int h = 0; h < tile.height; h++)
                {
                    Vector3 layerPos = basePos + new Vector3(0f, h * layerHeight, 0f);
                    Instantiate(rockHexPrefab, layerPos, Quaternion.identity, transform);
                }

                // Top position (surface) for ground tile and decorations
                Vector3 topPos = basePos + new Vector3(0f, tile.height * layerHeight, 0f);

                // 🎯 Spawn surface tile (road/water/grass). If hill (height>0) we keep top rock as mountain cap.
                switch (tile.type)
                {
                    case TileType.Road:
                        Instantiate(roadPrefab, topPos, Quaternion.identity, transform);
                        break;

                    case TileType.Water:
                        Instantiate(waterPrefab, topPos, Quaternion.identity, transform);
                        break;

                    case TileType.Grass:
                        if (tile.height >= 1)
                        {
                            // top is rock (mountain cap)
                            Instantiate(rockHexPrefab, topPos, Quaternion.identity, transform);
                        }
                        else
                        {
                            Instantiate(grassPrefab, topPos, Quaternion.identity, transform);
                        }
                        break;
                }

                // 🌲 Decoration CHỈ ở vùng thấp (use topPos)
                if (tile.type == TileType.Grass && tile.height < 2)
                {
                    if (tile.hasTree)
                        SpawnTreeCluster(topPos);

                    if (tile.hasRock)
                        SpawnRock(topPos);
                }
            }
        }
    }

    // 🌲 CỤM CÂY
    void SpawnTreeCluster(Vector3 center)
    {
        int count = Random.Range(4, 8);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];

            Vector2 circle = Random.insideUnitCircle * 0.6f;
            Vector3 pos = center + new Vector3(circle.x, 0.5f, circle.y);

            Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
        }
    }

    // 🪨 ĐÁ NHỎ
    void SpawnRock(Vector3 center)
    {
        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

        Vector2 circle = Random.insideUnitCircle * 0.2f;
        Vector3 pos = center + new Vector3(circle.x, 0.1f, circle.y);

        Instantiate(prefab, pos, Quaternion.identity, transform);
    }

    // 📍 TỌA ĐỘ HEX
    Vector3 GetHexPosition(int x, int y)
    {
        float xPos = hexWidth * (x + y * 0.5f - y / 2);
        float zPos = hexHeight * (y * 0.75f);

        return new Vector3(xPos, 0, zPos);
    }
}