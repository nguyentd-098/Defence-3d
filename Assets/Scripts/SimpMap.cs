//using System.Collections.Generic;
//using UnityEngine;

//public class AdvancedMapGenerator : MonoBehaviour
//{
//    [Header("Prefabs")]
//    public GameObject hexPrefab;
//    public GameObject roadPrefab;
//    public GameObject[] treePrefabs;
//    public GameObject[] rockPrefabs;
//    public GameObject waterPrefab;

//    [Header("Map Size")]
//    public int width = 12;
//    public int height = 12;

//    float hexWidth;
//    float hexHeight;

//    List<Vector2Int> path;

//    void Start()
//    {
//        path = GeneratePath(new Vector2Int(0, 0), new Vector2Int(width - 1, height - 1));
//        Generate();
//    }

//    // ─────────────────────────────────────────────
//    void Generate()
//    {
//        Renderer r = hexPrefab.GetComponentInChildren<Renderer>();
//        hexWidth = r.bounds.size.x;
//        hexHeight = r.bounds.size.z;

//        for (int x = 0; x < width; x++)
//        {
//            for (int y = 0; y < height; y++)
//            {
//                Vector3 pos = GetHexPosition(x, y);
//                Vector2Int coord = new Vector2Int(x, y);

//                bool isPath = path.Contains(coord);
//                Instantiate(isPath ? roadPrefab : hexPrefab, pos, Quaternion.identity, transform);

//                if (!isPath)
//                {
//                    float noise = Mathf.PerlinNoise(x * 0.2f, y * 0.2f);
//                    if (noise < 0.25f)
//                    {
//                        Instantiate(waterPrefab, pos + Vector3.up * 0.1f, Quaternion.identity, transform);
//                    }
//                    else if (noise < 0.4f)
//                    {
//                        SpawnRock(pos);
//                    }  
//                    else if (noise > 0.65f)
//                    {
//                        SpawnTree(pos);
//                    }
//                }
//            }
//        }
//    }

//    // ─────────────────────────────────────────────
//    List<Vector2Int> GeneratePath(Vector2Int start, Vector2Int end)
//    {
//        List<Vector2Int> result = new List<Vector2Int>();
//        Vector2Int current = start;
//        result.Add(current);

//        while (current != end)
//        {
//            if (Random.value < 0.5f)
//            {
//                if (current.x != end.x)
//                    current.x += (end.x > current.x) ? 1 : -1;
//            }
//            else
//            {
//                if (current.y != end.y)
//                    current.y += (end.y > current.y) ? 1 : -1;
//            }

//            result.Add(current);
//        }

//        return result;
//    }

//    // ─────────────────────────────────────────────
//    void SpawnTree(Vector3 pos)
//    {
//        if (treePrefabs.Length == 0) return;

//        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];

//        Vector3 offset = new Vector3(
//            Random.Range(-0.4f, 0.4f),
//            0,
//            Random.Range(-0.4f, 0.4f)
//        );

//        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);

//        float scale = Random.Range(0.8f, 1.2f);

//        GameObject tree = Instantiate(prefab, pos + offset + Vector3.up * 1f, rot, transform);
//        tree.transform.localScale = Vector3.one * scale;
//    }

//    // ─────────────────────────────────────────────
//    void SpawnRock(Vector3 pos)
//    {
//        if (rockPrefabs.Length == 0) return;

//        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

//        Vector3 spawnPos = pos;

//        Vector3 groundPos = GetGroundPosition(pos);

//        Quaternion rot = hexPrefab.transform.rotation;

//        Instantiate(prefab, groundPos, rot, transform);
//    }

//    // ─────────────────────────────────────────────
//    Vector3 GetHexPosition(int x, int y)
//    {
//        float xPos = hexWidth * (x + y * 0.5f - y / 2);
//        float zPos = hexHeight * (y * 0.75f);

//        return new Vector3(xPos, 0, zPos);
//    }
//    Vector3 GetGroundPosition(Vector3 pos)
//    {
//        Ray ray = new Ray(pos + Vector3.up * 5f, Vector3.down);

//        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
//        {
//            return hit.point;
//        }

//        return pos;
//    }
//}