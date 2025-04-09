using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class MainManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public NetworkPrefabRef malePrefab;
    public NetworkPrefabRef femalePrefab;

    public NetworkRunner runner;
    public NetworkSceneManagerDefault sceneManager;

    void Awake()
    {
        // Tạo NetworkRunner nếu chưa có
        if (runner == null)
        {
            GameObject runnerObj = new GameObject("NetworkRunner");
            runner = runnerObj.AddComponent<NetworkRunner>();
            runner.AddCallbacks(this);
            sceneManager = runnerObj.AddComponent<NetworkSceneManagerDefault>();
        }

        ConnectToFusion();
    }

    // Kết nối mạng với Shared Mode
    async void ConnectToFusion()
    {
        Debug.Log("Connecting to Fusion Network...");
        runner.ProvideInput = true;
        // Tên phiên cố định cho tất cả client
        string sessionName = "MyGameSession";

        // Cấu hình kết nối chung
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SceneManager = sceneManager,
            SessionName = sessionName,
            PlayerCount = 5, //số lượng người chơi
            IsVisible = true, //hiển thị phiên hay không
            IsOpen = true, //cho nhiều người chơi hay không
        };

        // Kết nối
        var result = await runner.StartGame(startGameArgs);
        if (result.Ok)
        {
            Debug.Log("Connected to Fusion Network successfully!");
        }
        else
        {
            Debug.LogError($"Failed to connect: {result.ShutdownReason}");
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
      
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
       
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
       
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
      
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
       
    }

    //hàm ày sẽ gọi sau khi kết nối thành công
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("player:" + player);
        if(runner.LocalPlayer != player) { return; }
        //thực hiện spawn nhân vật
        var playerClass = PlayerPrefs.GetString("PlayerClass");
        var playerName = PlayerPrefs.GetString("PlayerName");

        var prefab = playerClass.Equals("Male") ? malePrefab : femalePrefab;
        var positon = Vector3.zero;

        runner.Spawn(
            prefab, 
            positon,
            Quaternion.identity,
            player,
            (r, o) =>
            {
                Debug.Log($"Player spawned: " + o);
            }
            
        );
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
      
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
      
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
      
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
       
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
