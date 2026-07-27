using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject optionsPanel;

    [Header("Progress UI")]
    public TMP_Text progressText;
    public AnomalyDatabase database;

    [Header("UI to Hide")]
    public GameObject deleteSaveButton;

    [Header("Player Reference")]
    public PlayerInput playerInput;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (deleteSaveButton != null)
            deleteSaveButton.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        if (playerInput != null)
        {
            playerInput.enabled = !isPaused;
        }

        if (isPaused)
        {
            UpdateProgressText();
        }
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    private void UpdateProgressText()
    {
        if (progressText == null || database == null)
            return;

        List<int> unlockedAnomalies = SaveManager.Load();
        int totalAnomalies = database.anomalyKeys.Count;

        string progressFormat =
            LocalizationManager.Instance != null
                ? LocalizationManager.Instance.GetTranslation("ui_progress")
                : "{0} / {1}";

        progressText.text = string.Format(progressFormat, unlockedAnomalies.Count, totalAnomalies);
    }
}
