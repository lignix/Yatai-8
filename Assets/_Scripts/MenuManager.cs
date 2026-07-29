using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Database")]
    public AnomalyDatabase database;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject anomaliesPanel;

    [Header("Anomalies Menu")]
    public TMP_Text progressText;
    public Transform anomalyListContent;
    public GameObject anomalyTextPrefab;

    [Header("Save Management")]
    public TMP_Text deleteButtonText;
    public Button deleteButton;
    private int deleteClicks = 0;

    private void Start()
    {
        ShowPanel(mainPanel);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(false);
        anomaliesPanel.SetActive(false);

        panelToShow.SetActive(true);
        ResetDeleteButton();

        if (panelToShow == anomaliesPanel)
        {
            RefreshAnomalyList();
        }
    }

    public void PlayGame()
    {
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeAndLoadScene("Game", 0.5f);
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnDeleteSaveClicked()
    {
        deleteClicks++;

        if (deleteClicks == 1)
        {
            deleteButtonText.text = LocalizationManager.Instance.GetTranslation(
                "ui_delete_confirm"
            );
            deleteButtonText.color = Color.yellow;
        }
        else if (deleteClicks >= 2)
        {
            SaveManager.DeleteSave();
            deleteButtonText.text = LocalizationManager.Instance.GetTranslation(
                "ui_delete_success"
            );
            deleteButtonText.color = Color.red;
            deleteClicks = 0;

            if (deleteButton != null)
            {
                deleteButton.interactable = false;
            }

            if (anomaliesPanel.activeSelf)
            {
                RefreshAnomalyList();
            }
        }
    }

    private void ResetDeleteButton()
    {
        deleteClicks = 0;
        if (deleteButtonText != null)
        {
            string key = "ui_delete_default";
            deleteButtonText.text =
                LocalizationManager.Instance != null
                    ? LocalizationManager.Instance.GetTranslation(key)
                    : "Delete";
            deleteButtonText.color = Color.white;
        }

        if (deleteButton != null)
        {
            deleteButton.interactable = true;
        }
    }

    private void RefreshAnomalyList()
    {
        foreach (Transform child in anomalyListContent)
        {
            Destroy(child.gameObject);
        }

        List<int> unlockedAnomalies = SaveManager.Load();
        int totalAnomalies = database.anomalyKeys.Count;

        string progressFormat =
            LocalizationManager.Instance != null
                ? LocalizationManager.Instance.GetTranslation("ui_progress")
                : "{0} / {1}";

        progressText.text = string.Format(progressFormat, unlockedAnomalies.Count, totalAnomalies);

        for (int i = 0; i < totalAnomalies; i++)
        {
            GameObject newTextObj = Instantiate(anomalyTextPrefab, anomalyListContent);
            TMP_Text tmpText = newTextObj.GetComponent<TMP_Text>();

            string indexString = (i + 1).ToString("00");

            if (unlockedAnomalies.Contains(i))
            {
                string localizedName =
                    LocalizationManager.Instance != null
                        ? LocalizationManager.Instance.GetTranslation(database.anomalyKeys[i])
                        : database.anomalyKeys[i];

                tmpText.text = $"{indexString}. {localizedName}";
                tmpText.color = Color.white;
            }
            else
            {
                tmpText.text = $"{indexString}. ???";
                tmpText.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    private void OnEnable()
    {
        LocalizationManager.LanguageChangedEvent += OnLanguageChanged;
    }

    private void OnDisable()
    {
        LocalizationManager.LanguageChangedEvent -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        ResetDeleteButton();
        if (anomaliesPanel.activeSelf)
        {
            RefreshAnomalyList();
        }
    }
}
