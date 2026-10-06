using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Database")]
    public AnomalyDatabase database;

    [Header("Panels")]
    public GameObject warningPanel;
    public CanvasGroup warningTextGroup;
    public TMP_Text continuePromptText;
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject anomaliesPanel;
    public GameObject howToPlayPanel;

    private bool isFadingWarning = false;

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
        if (PlayerPrefs.GetInt("HasSeenWarning", 0) == 0)
        {
            ShowPanel(warningPanel);
        }
        else
        {
            ShowPanel(mainPanel);
        }
    }

    private void Update()
    {
        if (warningPanel != null && warningPanel.activeSelf)
        {
            bool acceptPressed = false;
            
            if (UnityEngine.InputSystem.Keyboard.current != null && 
                UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                acceptPressed = true;
            }
            
            if (UnityEngine.InputSystem.Mouse.current != null && 
                UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                acceptPressed = true;
            }

            if (acceptPressed)
            {
                AcceptWarning();
            }
        }
    }

    public void ShowPanel(GameObject panelToShow)
    {
        if (warningPanel != null) warningPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (anomaliesPanel != null) anomaliesPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);

        if (panelToShow != null) panelToShow.SetActive(true);
        ResetDeleteButton();

        if (panelToShow == anomaliesPanel)
        {
            RefreshAnomalyList();
        }
    }

    public void AcceptWarning()
    {
        if (isFadingWarning) return;
        
        PlayerPrefs.SetInt("HasSeenWarning", 1);
        PlayerPrefs.Save();
        
        StartCoroutine(FadeOutWarningRoutine());
    }

    private IEnumerator FadeOutWarningRoutine()
    {
        isFadingWarning = true;
        
        if (mainPanel != null) mainPanel.SetActive(true);

        float textFadeDuration = 1.0f;
        float pauseDuration = 0.5f;
        float bgFadeDuration = 1.0f;
        float timer = 0f;

        if (warningTextGroup != null)
        {
            while (timer < textFadeDuration)
            {
                timer += Time.deltaTime;
                warningTextGroup.alpha = Mathf.Lerp(1f, 0f, timer / textFadeDuration);
                yield return null;
            }
            warningTextGroup.alpha = 0f;
        }

        yield return new WaitForSeconds(pauseDuration);

        CanvasGroup panelGroup = warningPanel.GetComponent<CanvasGroup>();
        if (panelGroup == null) panelGroup = warningPanel.AddComponent<CanvasGroup>();

        timer = 0f;
        while (timer < bgFadeDuration)
        {
            timer += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(1f, 0f, timer / bgFadeDuration);
            yield return null;
        }

        if (warningPanel != null) warningPanel.SetActive(false);
        
        panelGroup.alpha = 1f;
        if (warningTextGroup != null) warningTextGroup.alpha = 1f;
        isFadingWarning = false;
    }
    public void OpenHowToPlay()
    {
        ShowPanel(howToPlayPanel);
    }

    public void OpenOptions()
    {
        ShowPanel(optionsPanel);
    }

    public void OpenAnomalies()
    {
        ShowPanel(anomaliesPanel);
    }

    public void BackToMainMenu()
    {
        ShowPanel(mainPanel);
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
        if (anomaliesPanel != null && anomaliesPanel.activeSelf)
        {
            RefreshAnomalyList();
        }
    }
}