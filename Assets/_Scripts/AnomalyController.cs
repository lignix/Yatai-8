using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnomalySceneData
{
    [HideInInspector] public int databaseIndex;
    public GameObject anomalyObject;
    public GameObject normalObjectToHide;
    [HideInInspector] public bool isUnlocked;
}

public class AnomalyController : MonoBehaviour
{
    public static AnomalyController Instance;

    [Header("Database Reference")]
    public AnomalyDatabase database;

    [Header("Anomalies List in Scene")]
    public List<AnomalySceneData> anomalies = new List<AnomalySceneData>();

    private AnomalySceneData currentAnomaly = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SyncDatabaseIndexes();
    }

    private void OnValidate()
    {
        SyncDatabaseIndexes();
    }

    private void SyncDatabaseIndexes()
    {
        if (anomalies == null) return;

        for (int i = 0; i < anomalies.Count; i++)
        {
            anomalies[i].databaseIndex = i;
        }
    }

    private void Start()
    {
        LoadUnlockedAnomalies();
    }

    private void LoadUnlockedAnomalies()
    {
        List<int> savedUnlocks = SaveManager.Load();
        
        foreach (var anomaly in anomalies)
        {
            if (savedUnlocks.Contains(anomaly.databaseIndex))
            {
                anomaly.isUnlocked = true;
            }
        }
    }

    public string SetupLoop(bool hasAnomaly)
    {
        ResetCurrentAnomaly();
        if (!hasAnomaly || database == null) return "";

        List<AnomalySceneData> availableAnomalies = anomalies.FindAll(a => !a.isUnlocked);
        
        if (availableAnomalies.Count == 0)
        {
            availableAnomalies = anomalies; 
        }

        currentAnomaly = availableAnomalies[Random.Range(0, availableAnomalies.Count)];

        if (currentAnomaly.anomalyObject != null) currentAnomaly.anomalyObject.SetActive(true);
        if (currentAnomaly.normalObjectToHide != null) currentAnomaly.normalObjectToHide.SetActive(false);

        if (currentAnomaly.databaseIndex < database.anomalyKeys.Count)
        {
            string key = database.anomalyKeys[currentAnomaly.databaseIndex];
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetTranslation(key) : key;
        }

        return "";
    }

    private void ResetCurrentAnomaly()
    {
        if (currentAnomaly != null)
        {
            if (currentAnomaly.anomalyObject != null) currentAnomaly.anomalyObject.SetActive(false);
            if (currentAnomaly.normalObjectToHide != null) currentAnomaly.normalObjectToHide.SetActive(true);
            currentAnomaly = null;
        }
    }

    public void UnlockCurrentAnomaly()
    {
        if (currentAnomaly != null && !currentAnomaly.isUnlocked)
        {
            currentAnomaly.isUnlocked = true;
            SaveAllProgress();
        }
    }

    public void SaveAllProgress()
    {
        List<int> unlockedIndexes = new List<int>();
        foreach (var anomaly in anomalies)
        {
            if (anomaly.isUnlocked)
            {
                unlockedIndexes.Add(anomaly.databaseIndex);
            }
        }
        SaveManager.Save(unlockedIndexes);
    }
}