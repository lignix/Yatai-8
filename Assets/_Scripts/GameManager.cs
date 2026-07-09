using UnityEngine;
using TMPro; // Requis pour les textes TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI de Debug")]
    public TMP_Text textAnomaly;
    public TMP_Text textLevel;

    [Header("État du Jeu")]
    public bool hasAnomaly = false;
    public int currentLevel = 0;

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
            Debug.Log($"<color=green>Bon choix !</color> Passage au niveau {currentLevel}.");
        }
        else
        {
            currentLevel = 0;
            Debug.Log($"<color=red>Mauvais choix !</color> Retour à zéro.");
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

        UpdateDebugUI();

        //TODO add anomalycontroller to choose an anomaly to play
    }

    private void UpdateDebugUI()
    {
        if (textAnomaly != null)
        {
            textAnomaly.text = "Anomalie : " + (hasAnomaly ? "<color=red>OUI</color>" : "<color=green>NON</color>");
        }
        
        if (textLevel != null)
        {
            textLevel.text = "Niveau : " + currentLevel;
        }
    }
}