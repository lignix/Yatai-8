using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool hasAnomaly = false;
    public int currentLevel = 0;
    
    private string currentAnomalyName = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GenerateNextLoop();
    }

    public void CheckPlayerChoice(bool wentForward)
    {
        bool correctChoice = (wentForward && !hasAnomaly) || (!wentForward && hasAnomaly);

        if (correctChoice)
        {
            currentLevel++;
            Debug.Log($"<color=green>Correct choice!</color> Moving to level {currentLevel}.");
            
            if (hasAnomaly) 
            {
                AnomalyController.Instance.UnlockCurrentAnomaly();
            }
        }
        else
        {
            currentLevel = 0;
            Debug.Log($"<color=red>Wrong choice!</color> Back to zero.");
        }

        GenerateNextLoop();
    }

    private void GenerateNextLoop()
    {
        if (currentLevel == 0)
        {
            hasAnomaly = false;
        }
        else
        {
            hasAnomaly = Random.value > 0.5f;
        }

        currentAnomalyName = AnomalyController.Instance.SetupLoop(hasAnomaly);
        
        Debug.Log($"Next loop generated. Anomaly: {hasAnomaly} ({currentAnomalyName}). Level: {currentLevel}");
    }
}