using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<int> unlockedAnomalies = new List<int>();
}

public static class SaveManager
{
    private const string SAVE_KEY = "AnomalyGameSave_V2";

    public static void Save(List<int> unlockedList)
    {
        SaveData data = new SaveData { unlockedAnomalies = unlockedList };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public static List<int> Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.unlockedAnomalies;
        }
        return new List<int>();
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }
}