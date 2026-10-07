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
    public float lookAngleThreshold = 20f;

    private bool isReading = false;
    private bool hasBeenRead = false;
    private Transform mainCamera;
    private float sqrInteractDistance;

    public InteractableIndicator indicator;

    private void Awake()
    {
        sqrInteractDistance = interactDistance * interactDistance;
    }

    private void OnEnable()
    {
        if (Camera.main != null) mainCamera = Camera.main.transform;

        if (player == null && PlayerController.InstanceTransform != null)
        {
            player = PlayerController.InstanceTransform;
            playerController = PlayerController.Instance;
        }

        pauseManager = PauseManager.Instance;

        if (creditsManager == null)
        {
            creditsManager = Object.FindAnyObjectByType<CreditsManager>();
        }
    }

    private void Update()
    {
        if (player == null || mainCamera == null) return;

        if (!isReading && !hasBeenRead)
        {
            if ((player.position - transform.position).sqrMagnitude <= sqrInteractDistance)
            {
                Vector3 dirToTarget = (transform.position - mainCamera.position).normalized;
                float angle = Vector3.Angle(mainCamera.forward, dirToTarget);

                if (angle <= lookAngleThreshold)
                {
                    PlayerInput pInput = player.GetComponent<PlayerInput>();
                    bool interactPressed = pInput != null && pInput.actions["Interact"].WasPressedThisFrame();

                    if (interactPressed && !pauseManager.isPaused)
                    {
                        ReadPaper();
                    }
                }
            }
        }
        else if (isReading)
        {
            PlayerInput pInput = player.GetComponent<PlayerInput>();
            bool exitPressed = pInput != null && pInput.actions["Cancel"].WasPressedThisFrame();

            if (exitPressed && !pauseManager.isPaused)
            {
                CloseAndRollCredits();
            }
        }
    }

    private void ReadPaper()
    {
        isReading = true;
        hasBeenRead = true;
        if (indicator != null) indicator.enabled = false;
        if (pickupSound != null) pickupSound.Play();
        if (paperUIPanel != null) paperUIPanel.SetActive(true);
        if (playerController != null) playerController.enabled = false;
        if (pauseManager != null) pauseManager.enabled = false;
    }

    private void CloseAndRollCredits()
    {
        isReading = false;
        if (paperUIPanel != null) paperUIPanel.SetActive(false);
        if (AchievementManager.Instance != null) AchievementManager.Instance.UnlockAchievement("secret_end");

        EndgameFade endgameFade = Object.FindAnyObjectByType<EndgameFade>();
        if (endgameFade != null) endgameFade.enabled = false;

        if (creditsManager != null) creditsManager.StartCreditsSequence();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}