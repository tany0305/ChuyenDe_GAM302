using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public Terrain terrain;

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsSharedModeMasterClient) // Chỉ MasterClient mới được random vị trí
        {
            Vector3 randomSpawnPoint = GetRandomPositionOnTerrain();

            Runner.Spawn(PlayerPrefab, randomSpawnPoint, Quaternion.identity,
                player, (runner, obj) =>
                {
                    var _player = obj.GetComponent<PlayerSetup>();
                    _player.SetupCamera();
                }
            );
        }
    }

    private Vector3 GetRandomPositionOnTerrain()
    {
        float terrainX = terrain.transform.position.x;
        float terrainZ = terrain.transform.position.z;
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        Vector3 randomSpawnPoint = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            float randomX = Random.Range(terrainX, terrainX + terrainWidth);
            float randomZ = Random.Range(terrainZ, terrainZ + terrainLength);
            float randomY = terrain.SampleHeight(new Vector3(randomX, 0, randomZ))
                            + terrain.transform.position.y;

            randomSpawnPoint = new Vector3(randomX, randomY, randomZ);

            // Kiểm tra xem vị trí có hợp lệ không (tránh spawn ở vực sâu)
            if (randomY > terrain.transform.position.y)
            {
                validPosition = true;
            }
        }

        Debug.Log($"[SPAWN POINT] {randomSpawnPoint}"); // Debug kiểm tra vị trí
        return randomSpawnPoint;
    }
}
