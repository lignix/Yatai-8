using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class TrainPassingByAnomaly : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip trainSound;

    [Header("Camera Shake Settings")]
    public float shakeDuration = 3.0f;
    public float shakeIntensity = 0.15f;
    public float shakeSpeed = 25.0f;

    private AudioSource audioSource;
    private bool hasTriggered = false;
    private Transform mainCameraTransform;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        hasTriggered = false;
        
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (audioSource != null && trainSound != null)
            {
                audioSource.PlayOneShot(trainSound);
            }

            if (mainCameraTransform != null)
            {
                StartCoroutine(ShakeCamera());
            }
        }
    }

    private IEnumerator ShakeCamera()
    {
        float timer = 0f;
        Vector3 originalCamLocalPos = mainCameraTransform.localPosition;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float progress = timer / shakeDuration;
            float fadeFactor = Mathf.Sin(progress * Mathf.PI);

            float offsetX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f * shakeIntensity * fadeFactor;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f * shakeIntensity * fadeFactor;

            mainCameraTransform.localPosition = originalCamLocalPos + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        mainCameraTransform.localPosition = originalCamLocalPos;
    }

    private void OnDisable()
    {
        hasTriggered = false;
    }
}