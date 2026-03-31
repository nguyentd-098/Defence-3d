using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 30;
    public int height = 30;
    public int seed = 0;
    public MapRenderer mapRenderer;

    [Header("Gameplay Tọa Độ")]
    [Tooltip("Check vào đây để Cổng và Trụ tự động nằm ở 2 góc chéo xa nhất của Map")]
    public bool autoSetPoints = true;
    public Vector2Int spawnPoint;
    public Vector2Int playerBase;

    [Header("Căn Chỉnh Chiều Cao (Tránh chìm đất)")]
    public float vfxHeightOffset = 0.5f;   // Tăng số này nếu Cổng/Trụ bị chìm
    public float enemyHeightOffset = 0.5f; // Tăng số này nếu Quái bị chìm

    [Header("Prefabs & VFX")]
    public GameObject spawnVfxPrefab;
    public GameObject baseVfxPrefab;
    public GameObject tempEnemyPrefab;

    MapGenerator _generator;
    MapData _currentMap;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // TỰ ĐỘNG GÁN TỌA ĐỘ XA NHẤT (Cách viền 2 ô để không dính vách núi)
        if (autoSetPoints)
        {
            spawnPoint = new Vector2Int(2, height - 3);        // Góc trên cùng bên trái
            playerBase = new Vector2Int(width - 3, 2);         // Góc dưới cùng bên phải
        }

        _generator = new MapGenerator();
        _generator.seed = seed; // Lấy seed từ GameManager truyền sang
        _currentMap = _generator.Generate(width, height);

        // San phẳng điểm đầu/cuối
        _currentMap.tiles[spawnPoint.x, spawnPoint.y].height = 0;
        _currentMap.tiles[playerBase.x, playerBase.y].height = 0;

        mapRenderer.Render(_currentMap);

        SpawnVFX();
        CalculateEnemyPathAndSpawn();
    }

    void SpawnVFX()
    {
        Vector3 spawnWorldPos = mapRenderer.GetWorldPosition(spawnPoint.x, spawnPoint.y);
        Vector3 baseWorldPos = mapRenderer.GetWorldPosition(playerBase.x, playerBase.y);

        // Nâng cao lên theo trục Y để không bị chìm
        spawnWorldPos.y += vfxHeightOffset;
        baseWorldPos.y += vfxHeightOffset;

        if (spawnVfxPrefab != null)
            Instantiate(spawnVfxPrefab, spawnWorldPos, Quaternion.identity);

        if (baseVfxPrefab != null)
            Instantiate(baseVfxPrefab, baseWorldPos, Quaternion.identity);
    }

    public void CalculateEnemyPathAndSpawn()
    {
        List<Vector2Int> enemyPath = _generator.FindPath(_currentMap, spawnPoint, playerBase);

        if (enemyPath != null && enemyPath.Count > 0)
        {
            Debug.Log($"[Pathfinding] Đường đi dài {enemyPath.Count} bước.");

            if (tempEnemyPrefab != null)
            {
                GameObject enemyObj = Instantiate(tempEnemyPrefab);
                EnemyMovement movement = enemyObj.GetComponent<EnemyMovement>();
                if (movement == null) movement = enemyObj.AddComponent<EnemyMovement>();

                // Truyền thêm biến enemyHeightOffset vào cho quái
                movement.InitPath(enemyPath, mapRenderer, enemyHeightOffset);
            }
        }
        else
        {
            Debug.LogWarning("[Pathfinding] Bị núi bít kín, không có đường tới Trụ!");
        }
    }
}