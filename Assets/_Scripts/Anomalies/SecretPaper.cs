using UnityEngine;
using UnityEngine.InputSystem;

public class SecretPaper : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject paperUIPanel;
    public PlayerController playerController;
    public PauseManager pauseManager;
    public CreditsManager creditsManager;

    [Header("Audio")]
    public AudioSource pickupSound;

    [Header("Settings")]
    public float interactDistance = 1f;

    private bool isReading = false;
    private bool hasBeenRead = false;

    private void OnEnable()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) 
            {
                player = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController>();
            }
        }
        
        if (pauseManager == null)
        {
            pauseManager = FindAnyObjectByType<PauseManager>();
        }
        
        if (creditsManager == null)
        {
            creditsManager = FindAnyObjectByType<CreditsManager>();
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (!isReading && !hasBeenRead) 
        {
            if (Vector3.Distance(player.position, transform.position) <= interactDistance)
            {
                bool interactPressed = false;
                
                if (Keyboard.current != null)
                {
                    if (!pauseManager.isPaused && (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame))
                    {
                        interactPressed = true;
                    }
                }
                
                if (!pauseManager.isPaused && Mouse.current != null) 
                {
                    if (Mouse.current.leftButton.wasPressedThisFrame) 
                    {
                        interactPressed = true; 
                    }
                }

                if (interactPressed) 
                {
                    ReadPaper();
                }
            }
        }
        else if (isReading)
        {
            bool exitPressed = false;

            if (!pauseManager.isPaused && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                exitPressed = true;
            }
            
            if (!pauseManager.isPaused && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                exitPressed = true;
            }

            if (exitPressed)
            {
                CloseAndRollCredits();
            }
        }
    }

    private void ReadPaper()
    {
        isReading = true;
        hasBeenRead = true;

        if (pickupSound != null)
        {
            pickupSound.Play();
        }

        if (paperUIPanel != null) paperUIPanel.SetActive(true);

        if (playerController != null) playerController.enabled = false;
        if (pauseManager != null) pauseManager.enabled = false;
    }

    private void CloseAndRollCredits()
    {
        isReading = false;
        if (paperUIPanel != null) paperUIPanel.SetActive(false);
        
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.UnlockAchievement("secret_end");
        }

        EndgameFade endgameFade = FindAnyObjectByType<EndgameFade>();
        if (endgameFade != null)
        {
            endgameFade.enabled = false;
        }

        if (creditsManager != null)
        {
            creditsManager.StartCreditsSequence();
        }
    }
}