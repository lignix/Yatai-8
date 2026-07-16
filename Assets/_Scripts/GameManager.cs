using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool hasAnomaly = false;
    public int currentLevel = 0;
    public int winLevel = 8;

    [Header("UI")]
    public TMP_Text levelDisplay;

    private string currentAnomalyName = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GenerateNextLoop();
        UpdateLevelDisplay();
    }

    public void CheckPlayerChoice(bool wentForward)
    {
        bool correctChoice = (wentForward && !hasAnomaly) || (!wentForward && hasAnomaly);

        if (correctChoice)
        {
            currentLevel++;
            Debug.Log($"Correct choice. Moving to level {currentLevel}.");

            if (hasAnomaly)
            {
                AnomalyController.Instance.UnlockCurrentAnomaly();
            }

            if (currentLevel >= winLevel)
            {
                Debug.Log("Win condition reached. Ready for endgame teleport.");
            }
        }
        else
        {
            currentLevel = 0;
            Debug.Log("Wrong choice. Reset to level 0.");
        }

        GenerateNextLoop();
        UpdateLevelDisplay();
    }

    private void GenerateNextLoop()
    {
        if (currentLevel >= winLevel)
        {
            hasAnomaly = false;
            return;
        }

        hasAnomaly = currentLevel != 0 && Random.value > 0.5f;
        currentAnomalyName = AnomalyController.Instance.SetupLoop(hasAnomaly);

        Debug.Log($"Next loop generated. Anomaly: {hasAnomaly} ({currentAnomalyName}). Level: {currentLevel}");
    }

    private void UpdateLevelDisplay()
    {
        if (levelDisplay != null)
        {
            levelDisplay.text = currentLevel.ToString("00");
        }
    }
}