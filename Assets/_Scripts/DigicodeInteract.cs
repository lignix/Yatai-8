using UnityEngine;
using UnityEngine.InputSystem;

public class DigicodeInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 2.5f;
    public float lookAngleThreshold = 20f;

    private Transform player;
    private PlayerController playerController;
    private Transform mainCamera;

    private void OnEnable()
    {
        if (Camera.main != null) mainCamera = Camera.main.transform;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                playerController = p.GetComponent<PlayerController>();
            }
        }
    }

    private void Update()
    {
        if (player == null || mainCamera == null || DigicodeManager.Instance == null) return;
        if (DigicodeManager.Instance.isSolved) return;
        if (DigicodeManager.Instance.keypadPanel.activeSelf) return;

        if (Vector3.Distance(player.position, transform.position) <= interactDistance)
        {
            Vector3 dirToTarget = (transform.position - mainCamera.position).normalized;
            float angle = Vector3.Angle(mainCamera.forward, dirToTarget);

            if (angle <= lookAngleThreshold)
            {
                bool interactPressed = false;

                if (!PauseManager.Instance.isPaused && Keyboard.current != null && (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame))
                    interactPressed = true;
                    
                if (!PauseManager.Instance.isPaused && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                    interactPressed = true;

                if (interactPressed)
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