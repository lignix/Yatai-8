using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject optionsPanel;

    [Header("Progress UI")]
    public TMP_Text progressText;
    public AnomalyDatabase database;

    [Header("UI to Hide")]
    public GameObject deleteSaveButton;

    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (deleteSaveButton != null) deleteSaveButton.SetActive(false);

        SetCursorState(true);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                CloseOptions();
            }
            else
            {
                TogglePause();
            }
        }
    }

    private void OnEnable()
    {
        LocalizationManager.LanguageChangedEvent += UpdateProgressText;
    }

    private void OnDisable()
    {
        LocalizationManager.LanguageChangedEvent -= UpdateProgressText;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;

        SetCursorState(!isPaused);

        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = !isPaused;
        }

        if (isPaused)
        {
            UpdateProgressText();
        }
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SetCursorState(false);
        
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeAndLoadScene("Menu", 0.5f);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void UpdateProgressText()
    {
        if (progressText == null || database == null) return;

        List<int> unlockedAnomalies = SaveManager.Load();
        int totalAnomalies = database.anomalyKeys.Count;

        string progressFormat = LocalizationManager.Instance != null
            ? LocalizationManager.Instance.GetTranslation("ui_progress")
            : "{0} / {1}";

        progressText.text = string.Format(progressFormat, unlockedAnomalies.Count, totalAnomalies);
    }
}