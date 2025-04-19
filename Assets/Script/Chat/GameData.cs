using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class GameData
{
    public static void SavePlayerData(PlayerData newPlayerData)
    {
        string path = @"G:\tany\hoctap\ky5\Game Online\playerData.json";

        List<PlayerData> dataList = new List<PlayerData>();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            try
            {
                PlayerDataListWrapper wrapper = JsonUtility.FromJson<PlayerDataListWrapper>(json);
                if (wrapper != null && wrapper.PlayerList != null)
                    dataList = wrapper.PlayerList;
            }
            catch (Exception e)
            {
                Debug.LogError("Lỗi đọc JSON: " + e.Message);
            }
        }

        dataList.Add(newPlayerData);

        PlayerDataListWrapper newWrapper = new PlayerDataListWrapper() { PlayerList = dataList };
        string newJson = JsonUtility.ToJson(newWrapper, true);
        File.WriteAllText(path, newJson);

        Debug.Log("Đã lưu dữ liệu mới. Tổng: " + dataList.Count);
    }

    public static List<PlayerData> LoadPlayerData()
    {
        var path = @"G:\tany\hoctap\ky5\Game Online\playerData.json"; // sửa lại cho đúng tên file lưu
        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                var wrapper = JsonUtility.FromJson<PlayerDataListWrapper>(json);
                return wrapper.PlayerList;
            }
            catch (Exception e)
            {
                Debug.LogError("Lỗi tải dữ liệu: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("File dữ liệu người chơi không tồn tại.");
        }

        return new List<PlayerData>();
    }
}

[Serializable]
public class PlayerDataListWrapper
{
    public List<PlayerData> PlayerList = new List<PlayerData>();
}

[Serializable] // sửa lại chỗ này nè chồng
public class PlayerData
{
    public string PlayerName;
    public string PlayerClass;
    public int Health;
}
