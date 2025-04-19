using UnityEngine;
using Fusion;
using System.Collections;

public class SpawnVFX : NetworkBehaviour
{
    [Header("VFX Settings")]
    public NetworkPrefabRef vfxPrefab;       // Prefab VFX (phải có NetworkObject)
    public float duration = 30f;             // Thời gian tồn tại
    public Transform vfxSpawnPoint;          // Vị trí spawn VFX (dưới chân nhân vật)

    private float lastSpawnTime = -30f;      // Thời gian spawn lần cuối

    void Update()
    {
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.T))
        {
            if (Time.time - lastSpawnTime >= duration)
            {
                // Nếu đủ thời gian 30s từ lần spawn trước, gọi spawn mới
                Vector3 spawnPos = vfxSpawnPoint != null ? vfxSpawnPoint.position : transform.position;
                RPC_SpawnVFX(spawnPos);
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_SpawnVFX(Vector3 spawnPosition)
    {
        if (Runner.IsServer || Runner.IsSharedModeMasterClient)
        {
            StartCoroutine(SpawnAndDestroy(spawnPosition));
        }
    }

    System.Collections.IEnumerator SpawnAndDestroy(Vector3 position)
    {
        // Spawn VFX
        NetworkObject vfx = Runner.Spawn(vfxPrefab, position, Quaternion.identity);

        // Gắn vào player để VFX đi theo
        if (vfx != null)
        {
            vfx.transform.SetParent(transform);
            vfx.transform.localPosition = Vector3.zero; // Canh đúng giữa chân
        }

        // Cập nhật thời gian spawn mới
        lastSpawnTime = Time.time;

        // Chờ hết thời gian tồn tại VFX
        yield return new WaitForSeconds(duration);

        // Hủy VFX
        if (vfx != null)
        {
            Runner.Despawn(vfx);
        }
    }
}
