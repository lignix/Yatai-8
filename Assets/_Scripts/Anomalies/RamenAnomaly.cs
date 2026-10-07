using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class RamenAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    [Tooltip("L'objet visuel des nouilles/bouillon à cacher quand on mange")]
    public GameObject ramenContentsToHide;

    [Header("Settings")]
    public float interactDistance = 2.5f;
    public float lookAngleThreshold = 20f;

    private AudioSource audioSource;
    private bool hasBeenEaten = false;
    private Transform mainCamera;

    public InteractableIndicator indicator;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        hasBeenEaten = false;

        if (ramenContentsToHide != null)
        {
            ramenContentsToHide.SetActive(true);
        }

        if (Camera.main != null) mainCamera = Camera.main.transform;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (hasBeenEaten || player == null || mainCamera == null) return;

        if (Vector3.Distance(player.position, transform.position) <= interactDistance)
        {
            Vector3 dirToTarget = (transform.position - mainCamera.position).normalized;
            float angle = Vector3.Angle(mainCamera.forward, dirToTarget);

            if (angle <= lookAngleThreshold)
            {
                PlayerInput pInput = player.GetComponent<PlayerInput>();
                bool interactPressed = pInput != null && pInput.actions["Interact"].WasPressedThisFrame();

                if (interactPressed && !PauseManager.Instance.isPaused)
                {
                    EatRamen();
                }
            }
        }
    }

    private void EatRamen()
    {
        hasBeenEaten = true;

        if (indicator != null) indicator.enabled = false;

        if (ramenContentsToHide != null)
        {
            ramenContentsToHide.SetActive(false);
        }

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.UnlockAchievement("eat");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}