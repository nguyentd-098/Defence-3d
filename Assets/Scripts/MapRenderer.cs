using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    public GameObject grassPrefab;
    public GameObject roadPrefab;
    public GameObject waterPrefab;

    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;

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
                Vector3 pos = GetHexPosition(x, y);
                TileData tile = map.tiles[x, y];
                switch (tile.type)
                {
                    case TileType.Road:
                        Instantiate(roadPrefab, pos, Quaternion.identity, transform);
                        break;

                    case TileType.Water:
                        Instantiate(waterPrefab, pos, Quaternion.identity, transform);
                        break;

                    case TileType.Grass:
                        Instantiate(grassPrefab, pos, Quaternion.identity, transform);
                        break;
                }
                if (tile.type == TileType.Grass)
                {
                    if (tile.hasTree)
                        SpawnTreeCluster(pos);

                    if (tile.hasRock)
                        SpawnRock(pos);
                }
            }
        }
    }
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
    void SpawnRock(Vector3 center)
    {
        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

        Vector2 circle = Random.insideUnitCircle * 0.15f;
        Vector3 pos = center + new Vector3(circle.x, 0.1f, circle.y);

        Instantiate(prefab, pos, Quaternion.identity, transform);
    }
    Vector3 GetHexPosition(int x, int y)
    {
        float xPos = hexWidth * (x + y * 0.5f - y / 2);
        float zPos = hexHeight * (y * 0.75f);

        return new Vector3(xPos, 0, zPos);
    }
}