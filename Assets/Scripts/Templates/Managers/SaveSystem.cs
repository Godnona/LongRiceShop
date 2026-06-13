using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static readonly string savePath = Path.Combine(Application.persistentDataPath, "gameData.json");

    // Lưu dữ liệu vào file JSON
    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        Debug.Log("Game Saved!");
    }

    // Tải dữ liệu từ file JSON. Nếu không có, tạo mới.
    public static GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game Loaded!");
            return data;
        }
        else
        {
            Debug.Log("No save file found. Creating new game data.");
            return new GameData();
        }
    }

    // for cheat, reset data
    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted: " + savePath);
        }
    }
}