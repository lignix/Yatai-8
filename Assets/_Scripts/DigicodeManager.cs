using System.Collections;
using UnityEngine;
using TMPro;

public class DigicodeManager : MonoBehaviour
{
    public static DigicodeManager Instance;

    [Header("UI References")]
    public GameObject keypadPanel;
    public TMP_Text displayCode;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip[] buttonBeepClips;
    public AudioClip errorClip;
    public AudioClip successClip;

    [Header("Door Settings")]
    public string secretCode = "123456";
    public Transform secretDoor;
    public Vector3 openRotation = new Vector3(-90f, 0f, 30f);
    [Tooltip("Vitesse de rotation de la porte")]
    public float openSpeed = 100f;
    public AudioSource doorAudioSource;
    public AudioClip doorOpenClip;

    private string currentInput = "";
    private PlayerController playerController;
    private bool isSolved = false;

    private void Awake()
    {
        Instance = this;
        if (keypadPanel != null) keypadPanel.SetActive(false);
    }

    public void OpenKeypad(PlayerController player)
    {
        if (isSolved) return;

        playerController = player;
        playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearInputSilent();

        if (keypadPanel != null) keypadPanel.SetActive(true);
    }

    public void CloseKeypad()
    {
        if (keypadPanel != null) keypadPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null) playerController.enabled = true;
    }

    public void AddDigit(string digit)
    {
        PlayRandomBeep();

        if (currentInput.Length < 6)
        {
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
        displayCode.text = currentInput;
        displayCode.color = Color.white;
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
            displayCode.text = "OK";
            displayCode.color = Color.green;
            StartCoroutine(UnlockSequence());
        }
        else
        {
            if (audioSource != null && errorClip != null)
            {
                audioSource.PlayOneShot(errorClip);
            }
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
        if (audioSource != null && successClip != null)
        {
            audioSource.PlayOneShot(successClip);
        }
        yield return new WaitForSeconds(0.5f);
        CloseKeypad();

        if (doorAudioSource != null && doorOpenClip != null)
        {
            doorAudioSource.PlayOneShot(doorOpenClip);
        }

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.UnlockAchievement("secret_end");
        }

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