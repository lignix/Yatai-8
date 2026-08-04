using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TrashBagCollisionSound : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip impactSound;
    public float sliceDuration = 0.4f;
    
    [Header("Optimization")]
    public float minImpactForce = 2f;
    public float soundCooldown = 0.2f;

    private AudioSource audioSource;
    private float lastSoundTime;
    private Coroutine stopRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.clip = impactSound;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastSoundTime < soundCooldown) return;

        float impactForce = collision.relativeVelocity.magnitude;
        if (impactForce >= minImpactForce)
        {
            PlayImpactSound(impactForce);
        }
    }

    private void PlayImpactSound(float impactForce)
    {
        if (impactSound == null) return;

        if (stopRoutine != null)
        {
            StopCoroutine(stopRoutine);
        }

        audioSource.pitch = Random.Range(0.85f, 1.15f);
        audioSource.volume = Mathf.Clamp01(impactForce / 10f);

        // Ensure the random start time leaves enough room to play the full slice duration
        float maxStartTime = Mathf.Max(0, impactSound.length - sliceDuration);
        audioSource.time = Random.Range(0f, maxStartTime);

        audioSource.Play();
        lastSoundTime = Time.time;

        stopRoutine = StartCoroutine(StopAudioAfterSlice());
    }

    private IEnumerator StopAudioAfterSlice()
    {
        yield return new WaitForSeconds(sliceDuration);
        audioSource.Stop();
    }
}