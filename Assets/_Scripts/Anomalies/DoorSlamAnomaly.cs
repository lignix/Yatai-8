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
    private float sqrTriggerDistance;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        originalRotation = transform.localRotation;
        sqrTriggerDistance = triggerDistance * triggerDistance;
    }

    private void OnEnable()
    {
        hasTriggered = false;
        transform.localRotation = originalRotation;

        if (player == null && PlayerController.InstanceTransform != null)
        {
            player = PlayerController.InstanceTransform;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (hasTriggered || player == null) return;

        if ((player.position - transform.position).sqrMagnitude <= sqrTriggerDistance)
        {
            TriggerSlam();
        }
    }

    private void TriggerSlam()
    {
        hasTriggered = true;
        if (audioSource != null && audioSource.clip != null) audioSource.Play();
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