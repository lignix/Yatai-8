using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnomalyData
{
    public string anomalyName;
    public GameObject anomalyObject;
    public GameObject normalObjectToHide;
    public bool isUnlocked;
}

public class AnomalyController : MonoBehaviour
{
    public static AnomalyController Instance;

    [Header("Anomalies list")]
    public List<AnomalyData> anomalies = new List<AnomalyData>();

    private AnomalyData currentAnomaly = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Called by GameManager on each loop
    public string SetupLoop(bool hasAnomaly)
    {
        ResetCurrentAnomaly();
        if (!hasAnomaly) return "";

        // Chooses an anomaly
        List<AnomalyData> availableAnomalies = anomalies.FindAll(a => !a.isUnlocked);
        
        if (availableAnomalies.Count == 0)
        {
            Debug.Log("All anomalies unlocked. Picking from all anomalies.");
            availableAnomalies = anomalies; 
        }

        currentAnomaly = availableAnomalies[Random.Range(0, availableAnomalies.Count)];

        if (currentAnomaly.anomalyObject != null) currentAnomaly.anomalyObject.SetActive(true);
        if (currentAnomaly.normalObjectToHide != null) currentAnomaly.normalObjectToHide.SetActive(false);

        Debug.Log(currentAnomaly.anomalyName);

        return currentAnomaly.anomalyName;
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
        if (currentAnomaly != null)
        {
            currentAnomaly.isUnlocked = true;
        }
    }
}