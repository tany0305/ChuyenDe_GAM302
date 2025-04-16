using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class MainManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public NetworkPrefabRef malePrefab;
    public NetworkPrefabRef femalePrefab;

    public NetworkRunner _runner;
    public NetworkSceneManagerDefault sceneManager;

    public GameObject spawnArea; // GameObject chứa Collider để xác định vùng spawn
    private BoxCollider spawnCollider;

    public Transform spawnPointA;
    public Transform spawnPointB;

    private bool spawnToA = true;

    void Awake()
    {
        // Lấy collider từ GameObject spawnArea
        if (spawnArea != null)
        {
            spawnCollider = spawnArea.GetComponent<Collider>() as BoxCollider;
            if (spawnCollider == null)
            {
                Debug.LogError("Spawn Area does not have a BoxCollider!");
            }
        }
        else
        {
            Debug.LogError("Spawn Area is not assigned!");
        }

        // Tạo NetworkRunner nếu chưa có
        if (_runner == null)
        {
            GameObject runnerObj = new GameObject("NetworkRunner");
            _runner = runnerObj.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);
            sceneManager = runnerObj.AddComponent<NetworkSceneManagerDefault>();
        }

        ConnectToFusion();
    }

    // Kết nối mạng với Shared Mode
    async void ConnectToFusion()
    {
        Debug.Log("Connecting to Fusion Network...");
        _runner.ProvideInput = true;
        string sessionName = "MyGameSession";

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SceneManager = sceneManager,
            SessionName = sessionName,
            PlayerCount = 5,
            IsVisible = true,
            IsOpen = true,
        };

        var result = await _runner.StartGame(startGameArgs);
        if (result.Ok)
        {
            Debug.Log("Connected to Fusion Network successfully!");
            isConnected = true;
        }
        else
        {
            Debug.LogError($"Failed to connect: {result.ShutdownReason}");
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 2, 2);
    }

    public NetworkPrefabRef[] EnemyPrefabRef;
    //public int maxEnemies = 10;
    private bool isConnected = false;
    private int currentEnemyCount = 0;

    //private List<NetworkObject> spawnedEnemies = new List<NetworkObject>();

    public void SpawnEnemy()
    {
        if (!isConnected || EnemyPrefabRef == null || EnemyPrefabRef.Length == 0 || spawnCollider == null)
        {
            Debug.LogWarning("Không thể spawn enemy: chưa kết nối hoặc EnemyPrefabRef bị null hoặc Collider không tồn tại.");
            return;
        }

        if (currentEnemyCount >= 5)
        {
            Debug.Log("Đã đạt giới hạn spawn tối đa 5 enemy.");
            return;
        }


        // Tính toán vị trí spawn ngẫu nhiên trong collider
        Vector3 spawnPosition = GetRandomPositionInCollider(spawnCollider);

        // Tạo rotation ngẫu nhiên
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        var enemyPrefab = EnemyPrefabRef[Random.Range(0, EnemyPrefabRef.Length)];
        var enemy = _runner.Spawn(
            enemyPrefab,
            spawnPosition,
            rotation,
            null,
            (r, o) =>
            {
                Debug.Log("Enemy Spawmed: " + o);
                currentEnemyCount++;

                o.GetComponent<EnemyMovement>().enabled = true;  // Bật hoặc gắn script nếu chưa có
            }
        );

        //Invoke(nameof(DeSpawnEnemy), 5);
    }

    // Lấy vị trí ngẫu nhiên trong vùng collider
    private Vector3 GetRandomPositionInCollider(BoxCollider collider)
    {
        Vector3 center = collider.transform.position;
        Vector3 size = collider.size;

        // Random hóa tọa độ trong phạm vi của collider
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        // Giữ y ở cùng mức độ (hoặc tính toán thêm nếu cần)
        float y = center.y;

        return new Vector3(x, y, z);
    }

    /*void DeSpawnEnemy(NetworkObject enemy)
    {
        if (enemy != null)
        {
            _runner.Despawn(enemy);
        }
    }
*/
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    // Hàm này sẽ gọi sau khi kết nối thành công
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("player:" + player);
        if (runner.LocalPlayer != player) { return; }
        // Thực hiện spawn nhân vật
        var playerClass = PlayerPrefs.GetString("PlayerClass");
        var playerName = PlayerPrefs.GetString("PlayerName");

        var prefab = playerClass.Equals("Male") ? malePrefab : femalePrefab;
        // Chọn vị trí spawn dựa trên lượt
        Vector3 spawnPosition = spawnToA && spawnPointA != null ? spawnPointA.position :
                                !spawnToA && spawnPointB != null ? spawnPointB.position :
                                Vector3.zero;

        runner.Spawn(
            prefab,
            spawnPosition,
            Quaternion.identity,
            player,
            (r, o) =>
            {
                Debug.Log($"Player spawned: " + o);
            }
        );
        // Đổi lượt cho người chơi tiếp theo
        spawnToA = !spawnToA;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public BoxCollider GetSpawnCollider()
    {
        return spawnCollider;
    }
}
