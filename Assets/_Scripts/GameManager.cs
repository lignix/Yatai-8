using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Debug UI")]
    public TMP_Text textAnomaly;
    public TMP_Text textLevel;

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
            
            if (hasAnomaly) 
            {
                AnomalyController.Instance.UnlockCurrentAnomaly();
            }
        }
        else
        {
            currentLevel = 0;
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

        // Resets scene + Generate the anomaly for this loop if hasAnomaly
        currentAnomalyName = AnomalyController.Instance.SetupLoop(hasAnomaly);

        UpdateDebugUI();
    }

    private void UpdateDebugUI()
    {
        if (textAnomaly != null)
        {
            if (hasAnomaly)
            {
                textAnomaly.text = $"Anomaly? : <color=red>YES ({currentAnomalyName})</color>";
            }
            else
            {
                textAnomaly.text = "Anomaly? : <color=green>NO</color>";
            }
        }
        
        if (textLevel != null)
        {
            textLevel.text = "Level : " + currentLevel;
        }
    }
}