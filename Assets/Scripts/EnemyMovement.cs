using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    private List<Vector3> _worldPath;
    private int _currentWaypointIndex = 0;
    private bool _isMoving = false;

    // Cập nhật thêm tham số yOffset
    public void InitPath(List<Vector2Int> hexPath, MapRenderer mapRenderer, float yOffset)
    {
        _worldPath = new List<Vector3>();

        foreach (Vector2Int hex in hexPath)
        {
            Vector3 worldPos = mapRenderer.GetWorldPosition(hex.x, hex.y);
            // Cộng thêm offset để nâng quái lên
            worldPos.y += yOffset;
            _worldPath.Add(worldPos);
        }

        if (_worldPath.Count > 0)
        {
            transform.position = _worldPath[0];
            _currentWaypointIndex = 1;
            _isMoving = true;
        }
    }

    void Update()
    {
        if (!_isMoving || _worldPath == null) return;

        Vector3 targetPos = _worldPath[_currentWaypointIndex];

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
        }

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _worldPath.Count)
            {
                _isMoving = false;
                Debug.Log("Quái vật đã chạm Trụ!");
                Destroy(gameObject);
            }
        }
    }
}