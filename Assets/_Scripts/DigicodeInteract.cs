using UnityEngine;
using UnityEngine.InputSystem;

public class DigicodeInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 2.5f;
    public float lookAngleThreshold = 20f;

    [Header("References")]
    public Transform player;
    public PlayerController playerController;

    private Transform mainCamera;
    private float sqrInteractDistance;

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
    }

    private void Update()
    {
        if (player == null && PlayerController.InstanceTransform != null)
        {
            player = PlayerController.InstanceTransform;
            playerController = PlayerController.Instance;
        }

        if (player == null || mainCamera == null || DigicodeManager.Instance == null) return;
        if (DigicodeManager.Instance.isSolved || DigicodeManager.Instance.keypadPanel.activeSelf) return;

        if ((player.position - transform.position).sqrMagnitude <= sqrInteractDistance)
        {
            Vector3 dirToTarget = (transform.position - mainCamera.position).normalized;
            float angle = Vector3.Angle(mainCamera.forward, dirToTarget);

            if (angle <= lookAngleThreshold)
            {
                PlayerInput pInput = player.GetComponent<PlayerInput>();
                bool interactPressed = pInput != null && pInput.actions["Interact"].WasPressedThisFrame();

                if (interactPressed && !PauseManager.Instance.isPaused)
                {
                    DigicodeManager.Instance.OpenKeypad(playerController);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}