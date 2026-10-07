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

    [Header("Gamepad Navigation")]
    public GameObject defaultPauseButton;
    public GameObject defaultOptionsButton;

    private GameObject lastSelectedPauseButton;

    [Header("Progress UI")]
    public TMP_Text progressText;
    public AnomalyDatabase database;

    [Header("UI to Hide")]
    public GameObject deleteSaveButton;

    public bool isPaused = false;

    private InputAction pauseAction;
    private InputAction cancelAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        pauseAction = new InputAction("Pause", binding: "<Keyboard>/escape");
        pauseAction.AddBinding("<Gamepad>/start");

        cancelAction = new InputAction("Cancel", binding: "<Keyboard>/escape");
        cancelAction.AddBinding("<Gamepad>/buttonEast");
    }

    private void OnEnable()
    {
        LocalizationManager.LanguageChangedEvent += UpdateProgressText;
        pauseAction.Enable();
        cancelAction.Enable();
    }

    private void OnDisable()
    {
        LocalizationManager.LanguageChangedEvent -= UpdateProgressText;
        pauseAction.Disable();
        cancelAction.Disable();
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
        if (pauseAction.WasPressedThisFrame() || cancelAction.WasPressedThisFrame())
        {
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                CloseOptions();
            }
            else if (pauseAction.WasPressedThisFrame() && !isPaused)
            {
                TogglePause();
            }
            else if ((pauseAction.WasPressedThisFrame() || cancelAction.WasPressedThisFrame()) && isPaused)
            {
                TogglePause();
            }
        }

        if (isPaused)
        {
            if (optionsPanel != null && optionsPanel.activeSelf) HandleSmartSelection(defaultOptionsButton);
            else HandleSmartSelection(lastSelectedPauseButton != null ? lastSelectedPauseButton : defaultPauseButton);
        }
    }

    private void HandleSmartSelection(GameObject defaultButton)
    {
        if (defaultButton == null) return;

        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
        {
            bool uiInput = false;

            if (Gamepad.current != null)
            {
                if (Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                    Gamepad.current.dpad.ReadValue().sqrMagnitude > 0.1f ||
                    Gamepad.current.buttonSouth.wasPressedThisFrame ||
                    Gamepad.current.buttonEast.wasPressedThisFrame ||
                    Gamepad.current.buttonWest.wasPressedThisFrame ||
                    Gamepad.current.buttonNorth.wasPressedThisFrame)
                {
                    uiInput = true;
                }
            }

            if (Keyboard.current != null && (Keyboard.current.upArrowKey.wasPressedThisFrame ||
                Keyboard.current.downArrowKey.wasPressedThisFrame ||
                Keyboard.current.leftArrowKey.wasPressedThisFrame ||
                Keyboard.current.rightArrowKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame))
            {
                uiInput = true;
            }

            if (uiInput)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(defaultButton);
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;

        SetCursorState(!isPaused);

        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null) playerController.enabled = !isPaused;

        if (isPaused)
        {
            UpdateProgressText();
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OpenOptions()
    {
        lastSelectedPauseButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        pausePanel.SetActive(true);

        if (lastSelectedPauseButton != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(lastSelectedPauseButton);
        }
        else
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 0f;
        AudioListener.pause = false;
        SetCursorState(false);

        if (FadeManager.Instance != null) FadeManager.Instance.FadeAndLoadScene("Menu", 0.5f);
        else SceneManager.LoadScene("Menu");
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
        string progressFormat = LocalizationManager.Instance != null ? LocalizationManager.Instance.GetTranslation("ui_progress") : "{0} / {1}";
        progressText.text = string.Format(progressFormat, unlockedAnomalies.Count, totalAnomalies);
    }
}