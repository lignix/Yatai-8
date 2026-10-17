using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool hasAnomaly = false;
    public int currentLevel = 0;
    public int winLevel = 8;
    public bool isEndlessMode = false;

    [Header("Rhythm Settings (Shuffle Bag)")]
    [Range(0f, 1f)]
    public float anomalyProbability = 0.5f;
    public int shuffleBagSize = 10;

    [Header("UI")]
    public TMP_Text levelDisplay;

    private string currentAnomalyName = "";
    private List<bool> shuffleBag = new List<bool>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        anomalyProbability = PlayerPrefs.GetFloat("SelectedAnomalyProbability", 0.5f);
        isEndlessMode = PlayerPrefs.GetInt("IsEndlessMode", 0) == 1;

        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
    }

    private void Start()
    {
        RefillShuffleBag();
        GenerateNextLoop();
        UpdateLevelDisplay();
    }

    public void CheckPlayerChoice(bool wentForward)
    {
        bool correctChoice = (wentForward && !hasAnomaly) || (!wentForward && hasAnomaly);

        if (correctChoice)
        {
            currentLevel++;

            if (hasAnomaly) AnomalyController.Instance.UnlockCurrentAnomaly();

            if (isEndlessMode)
            {
                int currentHighScore = PlayerPrefs.GetInt("EndlessHighScore", 0);
                if (currentLevel > currentHighScore)
                {
                    PlayerPrefs.SetInt("EndlessHighScore", currentLevel);
                    PlayerPrefs.Save();
                }
            }
            else if (currentLevel >= winLevel)
            {
                Debug.Log("Win condition reached. Ready for endgame teleport.");
            }
        }
        else
        {
            currentLevel = 0;
            PlayerPrefs.Save();
            RefillShuffleBag();
        }

        GenerateNextLoop();
        UpdateLevelDisplay();
    }

    private void RefillShuffleBag()
    {
        shuffleBag.Clear();

        // Calculates how many anomalies should be in the bag based on probability
        int anomalyCount = Mathf.RoundToInt(anomalyProbability * shuffleBagSize);

        for (int i = 0; i < shuffleBagSize; i++)
        {
            shuffleBag.Add(i < anomalyCount);
        }

        // Shuffles the bag using the Fisher-Yates algorithm
        for (int i = 0; i < shuffleBag.Count; i++)
        {
            bool temp = shuffleBag[i];
            int randomIndex = Random.Range(i, shuffleBag.Count);
            shuffleBag[i] = shuffleBag[randomIndex];
            shuffleBag[randomIndex] = temp;
        }
    }

    public void RestartFromDeath()
    {
        currentLevel = 0;

        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeAndRestart(1.0f);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private bool PullFromBag()
    {
        if (shuffleBag.Count == 0)
        {
            RefillShuffleBag();
        }

        bool nextState = shuffleBag[0];
        shuffleBag.RemoveAt(0);
        return nextState;
    }

    private void GenerateNextLoop()
    {
        if (!isEndlessMode && currentLevel >= winLevel)
        {
            hasAnomaly = false;
            return;
        }

        // Level 0 is always normal and shouldn't consume a ticket from the bag
        if (currentLevel == 0)
        {
            hasAnomaly = false;
        }
        else
        {
            hasAnomaly = PullFromBag();
        }

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