using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class RamenAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    
    [Header("Settings")]
    public float interactDistance = 2.5f;

    private AudioSource audioSource;
    private bool hasBeenEaten = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; 
    }

    private void OnEnable()
    {
        hasBeenEaten = false;
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (hasBeenEaten || player == null) return;

        if (Vector3.Distance(player.position, transform.position) <= interactDistance)
        {
            bool interactPressed = false;
            
            if (Keyboard.current != null)
            {
                if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame)
                {
                    interactPressed = true;
                }
            }
            
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    interactPressed = true;
                }
            }

            if (interactPressed)
            {
                EatRamen();
            }
        }
    }

    private void EatRamen()
    {
        hasBeenEaten = true;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.UnlockAchievement("eat");
        }
    }
}