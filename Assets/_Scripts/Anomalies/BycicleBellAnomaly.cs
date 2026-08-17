using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BicycleBellAnomaly : MonoBehaviour
{
    [Header("References")]
    public Collider triggerZone;

    [Header("Timing Settings")]
    public float ringInterval = 5.0f;

    [Header("Shake Settings")]
    public float shakeIntensity = 0.02f;
    public float shakeDuration = 1.0f;
    public float rotationShakeAngle = 2.0f;

    private AudioSource audioSource;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private bool hasTriggered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        if (triggerZone != null)
        {
            triggerZone.isTrigger = true;
            TriggerListener listener = triggerZone.gameObject.AddComponent<TriggerListener>();
            listener.onTrigger = HandlePlayerEnter;
        }
        else
        {
            Debug.LogWarning("NO TRIGGER");
        }
    }

    private void OnEnable()
    {
        hasTriggered = false;
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
    }

    private void HandlePlayerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(RingLoopRoutine());
        }
    }

    private IEnumerator RingLoopRoutine()
    {
        while (true)
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }

            yield return StartCoroutine(ShakeRoutine());

            yield return new WaitForSeconds(Mathf.Max(0, ringInterval - shakeDuration));
        }
    }

    private IEnumerator ShakeRoutine()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float xOffset = Random.Range(-1f, 1f) * shakeIntensity;
            float zOffset = Random.Range(-1f, 1f) * shakeIntensity;
            transform.localPosition = initialLocalPosition + new Vector3(xOffset, 0f, zOffset);
            
            float rotOffset = Random.Range(-rotationShakeAngle, rotationShakeAngle);
            transform.localRotation = initialLocalRotation * Quaternion.Euler(0f, 0f, rotOffset);

            yield return null;
        }

        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
    }

    private class TriggerListener : MonoBehaviour
    {
        public System.Action<Collider> onTrigger;

        private void OnTriggerEnter(Collider other)
        {
            onTrigger?.Invoke(other);
        }
    }
}