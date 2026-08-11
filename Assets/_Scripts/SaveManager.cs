using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<int> unlockedAnomalies = new List<int>();
}

public static class SaveManager
{
    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(List<int> unlockedList)
    {
        SaveData data = new SaveData { unlockedAnomalies = unlockedList };
        string json = JsonUtility.ToJson(data, true);

        try
        {
            File.WriteAllText(SaveFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game to JSON: {e.Message}");
        }
    }

    public static List<int> Load()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                return data != null ? data.unlockedAnomalies : new List<int>();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading game from JSON: {e.Message}");
                return new List<int>();
            }
        }
        return new List<int>();
    }

    public static void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                File.Delete(SaveFilePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error deleting save file: {e.Message}");
            }
        }
    }
}