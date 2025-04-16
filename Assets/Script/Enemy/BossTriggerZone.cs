using UnityEngine;
using Fusion;

public class BossTriggerZone : NetworkBehaviour
{
    public GameObject bossPrefab;
    public Transform spawnPoint;

    private bool bossSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        if (other.CompareTag("Player") && !bossSpawned)
        {
            bossSpawned = true;
            Runner.Spawn(bossPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
