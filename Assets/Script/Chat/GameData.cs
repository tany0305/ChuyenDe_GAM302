using System;
using System.IO;
using UnityEngine;


//A. lưu offline( lưu trên máy người chơi)
//-playerprefs
//--chõ lưu không giống nhau tùy theo hệ điều hành
//--lưu được các kiểu dữ liệu cơ bản như int, string, float
//-Lưu file txt, json
//--lưu được các kiểu dũ liệu phực tạp như List, Dictionary
//--chỗ lưu là địa chỉ quy định
public class GameData
{
    public static void SavePlayerData1(PlayerData playerData)
    {
        var json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("PlayerData", json);
    }

    public static PlayerData Load1()
    {
        var json = PlayerPrefs.GetString("PlayerData");
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonUtility.FromJson<PlayerData>(json);
    }

    /*public static void SavePlayerData2(PlayerData playerData)
    {
        try
        {
            var json = JsonUtility.ToJson(playerData);
            var path = Application.persistentDataPath + "?playerData.json";
            System.IO.File.WriteAllText(path, json);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }*/
}

[SerializeField]
public class PlayerData
{
    public string PlayerName;
    public string PlayerClass;
    public int Health;
}