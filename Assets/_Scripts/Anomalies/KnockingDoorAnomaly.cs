using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class KnockingDoorAnomaly : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip[] knockSounds;

    [Header("Timings")]
    public float knockInterval = 3.0f;

    [Header("Visual Shake Settings (X Axis)")]
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.25f;

    private AudioSource audioSource;
    private Vector3 initialLocalPosition;
    private Coroutine knockLoopRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        initialLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        transform.localPosition = initialLocalPosition;
        knockLoopRoutine = StartCoroutine(KnockLoop());
    }

    private void OnDisable()
    {
        if (knockLoopRoutine != null)
        {
            StopCoroutine(knockLoopRoutine);
        }

        transform.localPosition = initialLocalPosition;
    }

    private IEnumerator KnockLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(knockInterval);

            PlayRandomKnockSound();

            yield return StartCoroutine(ShakeOnX());
        }
    }

    private void PlayRandomKnockSound()
    {
        if (audioSource != null && knockSounds != null && knockSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, knockSounds.Length);
            AudioClip selectedClip = knockSounds[randomIndex];

            if (selectedClip != null)
            {
                audioSource.PlayOneShot(selectedClip);
            }
        }
    }

    private IEnumerator ShakeOnX()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float xOffset = Random.Range(-1f, 1f) * shakeIntensity;
            transform.localPosition = initialLocalPosition + new Vector3(xOffset, 0f, 0f);

            yield return null;
        }

        transform.localPosition = initialLocalPosition;
    }
}