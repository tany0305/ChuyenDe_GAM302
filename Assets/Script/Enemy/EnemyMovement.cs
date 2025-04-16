using UnityEngine;
using Fusion;

public class EnemyMovement : NetworkBehaviour
{
    public BoxCollider spawnCollider;

    public bool IsOwner { get; private set; }

    private void Start()
    {
        if (spawnCollider == null)
        {
            // Lấy spawnCollider từ MainManager
            spawnCollider = FindObjectOfType<MainManager>().GetSpawnCollider();
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Chỉ cho phép owner (player) điều khiển chuyển động

        // Di chuyển ngẫu nhiên cho enemy (hoặc bạn có thể thêm logic di chuyển của riêng mình)
        MoveEnemyWithinBounds();
    }

    private void MoveEnemyWithinBounds()
    {
        if (spawnCollider == null) return;

        // Lấy vị trí hiện tại của enemy
        Vector3 currentPosition = transform.position;

        // Tính toán giới hạn của collider
        Vector3 min = spawnCollider.bounds.min;
        Vector3 max = spawnCollider.bounds.max;

        // Giới hạn vị trí của enemy trong phạm vi của collider
        currentPosition.x = Mathf.Clamp(currentPosition.x, min.x, max.x);
        currentPosition.z = Mathf.Clamp(currentPosition.z, min.z, max.z);

        // Nếu cần, giới hạn cả trục Y (có thể bỏ qua nếu không cần di chuyển theo chiều Y)
        currentPosition.y = Mathf.Clamp(currentPosition.y, min.y, max.y);

        // Cập nhật vị trí của enemy
        transform.position = currentPosition;
    }


}
