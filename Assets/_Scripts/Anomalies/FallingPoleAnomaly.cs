using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FallingPoleAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public AudioSource fallSound;
    
    [Header("Settings")]
    public float triggerDistance = 4f;
    public float fallSpeed = 60f;
    [Tooltip("La rotation exacte qu'aura le poteau une fois au sol")]
    public Vector3 absoluteTargetRotation = new Vector3(0f, 0f, 0f);

    private bool hasTriggered = false;
    private bool isFalling = false;
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private Collider poleCollider;
    private float sqrTriggerDistance;

    private void Awake()
    {
        poleCollider = GetComponent<Collider>();
        originalRotation = transform.localRotation;
        targetRotation = Quaternion.Euler(absoluteTargetRotation);
        sqrTriggerDistance = triggerDistance * triggerDistance;
    }

    private void OnEnable()
    {
        hasTriggered = false;
        isFalling = false;
        transform.localRotation = originalRotation;

        if (poleCollider != null) poleCollider.enabled = true;

        if (player == null && PlayerController.InstanceTransform != null)
        {
            player = PlayerController.InstanceTransform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (!hasTriggered && (player.position - transform.position).sqrMagnitude <= sqrTriggerDistance)
        {
            hasTriggered = true;
            isFalling = true;
            if (fallSound != null) fallSound.Play();
        }

        if (isFalling)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, fallSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
            {
                isFalling = false; 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && isFalling && other.CompareTag("Player"))
        {
            isFalling = false;
            if (poleCollider != null) poleCollider.enabled = false;
            GameManager.Instance.RestartFromDeath();
        }
    }
}