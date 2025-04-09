using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _malePlayerPrefab;
    [SerializeField] private NetworkPrefabRef _femalePlayerPrefab;
    [SerializeField] private Transform[] _spawnPoints;

    [SerializeField] private NetworkRunner _runner;
    [SerializeField] private NetworkSceneManagerDefault _sceneManager;

    private void Awake()
    {
        // Tạo NetworkRunner nếu chưa có
        if (_runner == null)
        {
            GameObject runnerObj = new GameObject("NetworkRunner");
            _runner = runnerObj.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);
            _sceneManager = runnerObj.AddComponent<NetworkSceneManagerDefault>();
        }

        ConnectToFusion();
    }

    // Kết nối mạng với Shared Mode
    async void ConnectToFusion()
    {
        Debug.Log("Connecting to Fusion Network...");
        _runner.ProvideInput = true;
        // Tên phiên cố định cho tất cả client
        string sessionName = "MyGameSession";

        // Cấu hình kết nối chung
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SceneManager = _sceneManager,
            SessionName = sessionName,
            PlayerCount = 5, //số lượng người chơi
            IsVisible = true, //hiển thị phiên hay không
            IsOpen = true, //cho nhiều người chơi hay không
        };

        // Kết nối
        var result = await _runner.StartGame(startGameArgs);
        if (result.Ok)
        {
            Debug.Log("Connected to Fusion Network successfully!");
        }
        else
        {
            Debug.LogError($"Failed to connect: {result.ShutdownReason}");
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player joined: " + player);
        Debug.Log(": " + _runner.IsSharedModeMasterClient);
        Debug.Log(": " + _runner.LocalPlayer);
        Debug.Log(": " + player);
        if (_runner.LocalPlayer != player)
            return;

        // Chọn vị trí spawn ngẫu nhiên
        Transform spawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];

        // Lấy thông tin người chơi
        string playerClass = PlayerPrefs.GetString("PlayerClass");
        string playerName = PlayerPrefs.GetString("PlayerName");

        // Chọn prefab dựa trên class
        NetworkPrefabRef playerPrefab = playerClass == "Male" ? _malePlayerPrefab : _femalePlayerPrefab;

        // Spawn player
        var playerObject = _runner.Spawn(
            playerPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            player,
            // Khởi tạo player với tên
            (r, obj) =>
            {
                Debug.Log($"Player {playerName} spawned!");
                obj.GetComponent<PlayerPro>().PlayerName =
                    playerName;
                obj.GetComponent <MainSceneManager>()._runner = _runner;
            }
        );
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}