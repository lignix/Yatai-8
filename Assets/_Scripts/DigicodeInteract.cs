using UnityEngine;
using UnityEngine.InputSystem;

public class DigicodeInteract : MonoBehaviour
{
    public float interactDistance = 2.5f;
    private Transform player;
    private PlayerController playerController;

    private void OnEnable()
    {
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
        if (player == null || DigicodeManager.Instance == null) return;
        if (DigicodeManager.Instance.keypadPanel.activeSelf) return;

        if (Vector3.Distance(player.position, transform.position) <= interactDistance)
        {
            bool interactPressed = false;

            if (Keyboard.current != null && (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame))
                interactPressed = true;
                
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                interactPressed = true;

            if (interactPressed)
            {
                DigicodeManager.Instance.OpenKeypad(playerController);
            }
        }
    }
}