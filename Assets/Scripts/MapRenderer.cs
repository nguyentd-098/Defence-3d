using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject grassPrefab;
    public GameObject roadPrefab;
    public GameObject waterPrefab;
    public GameObject rockHexPrefab;    // hex dùng để stack height

    [Header("Decoration")]
    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;

    [Header("Hex Size (tự động lấy từ prefab)")]
    public float hexWidth = 1f;
    public float hexHeight = 0.866f;   // √3/2 ≈ 0.866 cho pointy-top

    float _layerHeight = 1.5f;

    // ──────────────────────────────────────────
    void Start()
    {
        // Lấy kích thước thực từ prefab
        if (grassPrefab != null)
        {
            var r = grassPrefab.GetComponentInChildren<Renderer>();
            if (r != null)
            {
                hexWidth = r.bounds.size.x;
                hexHeight = r.bounds.size.z;
            }
        }

        if (rockHexPrefab != null)
        {
            var r = rockHexPrefab.GetComponentInChildren<Renderer>();
            if (r != null) _layerHeight = r.bounds.size.y;
        }
    }

    // ──────────────────────────────────────────
    public void Render(MapData map)
    {
        // Clear map cũ nếu render lại
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                TileData tile = map.tiles[x, y];
                Vector3 basePos = GetHexPosition(x, y);

                // --- Stack đá nền theo height ---
                for (int h = 0; h < tile.height; h++)
                {
                    Vector3 layerPos = basePos + new Vector3(0f, h * _layerHeight, 0f);
                    SafeInstantiate(rockHexPrefab, layerPos);
                }

                Vector3 topPos = basePos + new Vector3(0f, tile.height * _layerHeight, 0f);

                // --- Tile mặt trên ---
                switch (tile.type)
                {
                    case TileType.Road:
                        SafeInstantiate(roadPrefab, topPos);
                        break;

                    case TileType.Water:
                        SafeInstantiate(waterPrefab, topPos);
                        break;

                    case TileType.Grass:
                        // height >= 3 → chỉ dùng rockHex (đỉnh núi)
                        SafeInstantiate(tile.height >= 3 ? rockHexPrefab : grassPrefab, topPos);
                        break;
                }

                // --- Decoration (chỉ Grass thấp) ---
                if (tile.type == TileType.Grass && tile.height < 2)
                {
                    if (tile.hasTree && treePrefabs.Length > 0)
                        SpawnTreeCluster(topPos);

                    if (tile.hasRock && rockPrefabs.Length > 0)
                        SpawnRock(topPos);
                }
            }
        }
    }
    Vector3 GetHexPosition(int col, int row)
    {
        float xPos = hexWidth * col + (row % 2 == 1 ? hexWidth * 0.5f : 0f);
        float zPos = hexHeight * row * 0.75f;
        return new Vector3(xPos, 0f, zPos);
    }

    // ──────────────────────────────────────────
    void SpawnTreeCluster(Vector3 center)
    {
        int count = Random.Range(3, 7);
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            Vector2 circle = Random.insideUnitCircle * 0.55f;
            Vector3 pos = center + new Vector3(circle.x, 0.5f, circle.y);
            Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0, 360f), 0), transform);
        }
    }
    void SpawnRock(Vector3 center)
    {
        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
        Vector2 circle = Random.insideUnitCircle * 0.2f;
        Vector3 pos = center + new Vector3(circle.x, 0.1f, circle.y);
        Instantiate(prefab, pos, Quaternion.identity, transform);
    }

    void SafeInstantiate(GameObject prefab, Vector3 pos)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[MapRenderer] Prefab chưa được gán!");
            return;
        }
        Instantiate(prefab, pos, Quaternion.identity, transform);
    }
    public Vector3 GetWorldPosition(int col, int row)
    {
        float xPos = hexWidth * col + (row % 2 == 1 ? hexWidth * 0.5f : 0f);
        float zPos = hexHeight * row * 0.75f;
        return new Vector3(xPos, 0.5f, zPos);
    }
}