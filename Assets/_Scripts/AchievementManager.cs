using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        while (!SteamManager.Initialized)
        {
            yield return null;
        }

        CheckAndSyncSaveAchievements();
    }

    public void CheckAndSyncSaveAchievements()
    {
        if (!SteamManager.Initialized) return;

        List<int> unlockedList = SaveManager.Load();
        int count = unlockedList.Count;

        SteamUserStats.SetStat("stat_anomalies_found", count);

        if (count >= 10) UnlockAchievement("ano_10");
        if (count >= 20) UnlockAchievement("ano_20");
        if (count >= 30) UnlockAchievement("ano_30");
        if (count >= 40) UnlockAchievement("ano_40");

        SteamUserStats.StoreStats();
    }

    public void UnlockAchievement(string achievementID)
    {
        if (!SteamManager.Initialized) return;

        bool isUnlocked;
        if (SteamUserStats.GetAchievement(achievementID, out isUnlocked) && !isUnlocked)
        {
            SteamUserStats.SetAchievement(achievementID);
            SteamUserStats.StoreStats();
            Debug.Log($"[Steam] Succès débloqué : {achievementID}");
        }
    }
}