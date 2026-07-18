using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorSlamAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Settings")]
    public float triggerDistance = 2f;
    public Vector3 closedRotation = new Vector3(-90f, 0f, -90f);
    public float slamSpeed = 25f;

    private AudioSource audioSource;
    private bool hasTriggered = false;
    private Quaternion originalRotation;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Store the initial rotation so it can be reset in future loops
        originalRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        // Reset state every time the AnomalyController activates this object
        hasTriggered = false;
        transform.localRotation = originalRotation;
    }

    private void OnDisable()
    {
        // Prevent the coroutine from running if the anomaly is disabled mid-animation
        StopAllCoroutines();
    }

    private void Update()
    {
        if (hasTriggered || player == null) return;

        if (Vector3.Distance(player.position, transform.position) <= triggerDistance)
        {
            TriggerSlam();
        }
    }

    private void TriggerSlam()
    {
        hasTriggered = true;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        StartCoroutine(SlamRoutine());
    }

    private IEnumerator SlamRoutine()
    {
        Quaternion targetRotation = Quaternion.Euler(closedRotation);

        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * slamSpeed);
            yield return null;
        }

        transform.localRotation = targetRotation;
    }
}