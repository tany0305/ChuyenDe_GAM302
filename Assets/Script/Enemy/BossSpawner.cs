using UnityEngine;
using Fusion;

public class BossSpawner : NetworkBehaviour
{
    [Header("Spawn Settings")]
    public GameObject bossPrefab;  // Prefab của Boss
    public int numberOfBosses = 3;  // Số lượng boss muốn spawn
    public Vector3[] spawnPositions;  // Các vị trí spawn boss

    private int spawnedBossCount = 0;

    // Hàm này sẽ được gọi để spawn các boss
    public void SpawnBosses()
    {
        if (!Object.HasStateAuthority) return;

        // Kiểm tra nếu số lượng spawn đã đạt tối đa
        if (spawnedBossCount >= numberOfBosses)
            return;

        // Chỉ spawn khi còn boss cần spawn
        for (int i = spawnedBossCount; i < numberOfBosses && i < spawnPositions.Length; i++)
        {
            SpawnBossAtPosition(spawnPositions[i]);
            spawnedBossCount++;
        }
    }

    // Hàm spawn một boss tại vị trí cụ thể
    private void SpawnBossAtPosition(Vector3 spawnPosition)
    {
        // Spawn boss và lưu đối tượng trả về là NetworkObject
        NetworkObject bossNetworkObject = Runner.Spawn(bossPrefab, spawnPosition, Quaternion.identity);

        // Lấy gameobject từ NetworkObject
        GameObject boss = bossNetworkObject.gameObject;

        // Lấy BossAI từ boss để thiết lập máu
        BossAI bossAI = boss.GetComponent<BossAI>();
        bossAI.currentHealth = bossAI.maxHealth;
    }

    // Có thể gọi hàm này từ đâu đó trong game để spawn boss
    void Start()
    {
        // Gọi spawn boss khi bắt đầu
        SpawnBosses();
    }
}
