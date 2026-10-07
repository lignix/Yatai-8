using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DigicodeManager : MonoBehaviour
{
    public static DigicodeManager Instance;

    [Header("UI References")]
    public GameObject keypadPanel;
    public TMP_Text displayCode;

    [Header("Gamepad Navigation")]
    public GameObject defaultKeypadButton;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip[] buttonBeepClips;
    public AudioClip errorClip;
    public AudioClip successClip;

    [Header("Door Settings")]
    public string secretCode = "260315";
    public string placeholderKey = "ui_keypad_format";
    public Transform secretDoor;
    public Vector3 openRotation = new Vector3(-90f, 0f, 30f);
    public float openSpeed = 100f;
    public AudioSource doorAudioSource;
    public AudioClip doorOpenClip;

    private string currentInput = "";
    private PlayerController playerController;
    private PauseManager pauseManager;
    public bool isSolved = false;

    public InteractableIndicator indicator;

    private InputAction cancelAction;

    private void Awake()
    {
        Instance = this;
        if (keypadPanel != null) keypadPanel.SetActive(false);

        cancelAction = new InputAction("Cancel", binding: "<Keyboard>/escape");
        cancelAction.AddBinding("<Gamepad>/buttonEast");
    }

    private void OnEnable()
    {
        cancelAction.Enable();
    }

    private void OnDisable()
    {
        cancelAction.Disable();
    }

    private void Start()
    {
        pauseManager = FindAnyObjectByType<PauseManager>();
    }

    private void Update()
    {
        if (keypadPanel != null && keypadPanel.activeSelf && !isSolved)
        {
            if (cancelAction.WasPressedThisFrame()) CloseKeypad();
            HandleSmartSelection(defaultKeypadButton);
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

    public void OpenKeypad(PlayerController player)
    {
        if (isSolved) return;

        playerController = player;
        if (playerController != null) playerController.enabled = false;
        if (pauseManager != null) pauseManager.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearInputSilent();
        if (keypadPanel != null) keypadPanel.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void CloseKeypad()
    {
        if (keypadPanel != null) keypadPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null) playerController.enabled = true;
        if (pauseManager != null) pauseManager.enabled = true;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void AddDigit(string digit)
    {
        PlayRandomBeep();
        if (currentInput.Length < 6)
        {
            if (currentInput.Length == 0) displayCode.color = Color.white;
            currentInput += digit;
            displayCode.text = currentInput;
        }
    }

    public void ClearInput()
    {
        PlayRandomBeep();
        ClearInputSilent();
    }

    private void ClearInputSilent()
    {
        currentInput = "";
        displayCode.text = LocalizationManager.Instance != null ? LocalizationManager.Instance.GetTranslation(placeholderKey) : "AA/MM/JJ";
        displayCode.color = Color.gray;
    }

    public void ValidateOrClose()
    {
        if (string.IsNullOrEmpty(currentInput))
        {
            PlayRandomBeep();
            CloseKeypad();
            return;
        }

        if (currentInput == secretCode)
        {
            isSolved = true;
            if (indicator != null) indicator.enabled = false;
            displayCode.text = "OK";
            displayCode.color = Color.green;
            StartCoroutine(UnlockSequence());
        }
        else
        {
            if (audioSource != null && errorClip != null) audioSource.PlayOneShot(errorClip);
            CloseKeypad();
        }
    }

    private void PlayRandomBeep()
    {
        if (audioSource != null && buttonBeepClips != null && buttonBeepClips.Length > 0)
        {
            int randomIndex = Random.Range(0, buttonBeepClips.Length);
            audioSource.PlayOneShot(buttonBeepClips[randomIndex]);
        }
    }

    private IEnumerator UnlockSequence()
    {
        if (audioSource != null && successClip != null) audioSource.PlayOneShot(successClip);
        yield return new WaitForSeconds(0.5f);
        CloseKeypad();

        if (doorAudioSource != null && doorOpenClip != null) doorAudioSource.PlayOneShot(doorOpenClip);
        if (AchievementManager.Instance != null) AchievementManager.Instance.UnlockAchievement("secret_end");

        if (secretDoor != null)
        {
            Quaternion startRot = secretDoor.localRotation;
            Quaternion targetRot = Quaternion.Euler(openRotation);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                secretDoor.localRotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            secretDoor.localRotation = targetRot;
        }
    }
}