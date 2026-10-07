using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ArcadeAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Renderer screenRenderer;
    public Material screenOffMaterial;
    public Light roomSpotlight;
    public AudioSource scareAudioSource;

    [Header("Settings")]
    public float triggerDistance = 2f;
    public float scareSoundDelay = 0.2f;

    private AudioSource audioSource;
    private bool hasTriggered = false;
    private Material originalScreenMaterial;
    private float sqrTriggerDistance;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (screenRenderer != null) originalScreenMaterial = screenRenderer.material;
        sqrTriggerDistance = triggerDistance * triggerDistance;
    }

    private void OnEnable()
    {
        hasTriggered = false;
        if (screenRenderer != null && originalScreenMaterial != null) screenRenderer.material = originalScreenMaterial;
        if (roomSpotlight != null) roomSpotlight.enabled = true;

        ArcadeScreenEffect flickerEffect = GetComponent<ArcadeScreenEffect>();
        if (flickerEffect != null) flickerEffect.enabled = true;
        if (audioSource != null && !audioSource.isPlaying) audioSource.Play();
        if (scareAudioSource != null) scareAudioSource.Stop();

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
            TurnOffArcade();
        }
    }

    private void TurnOffArcade()
    {
        hasTriggered = true;
        if (audioSource != null) audioSource.Stop();
        if (screenRenderer != null && screenOffMaterial != null) screenRenderer.material = screenOffMaterial;
        if (roomSpotlight != null) roomSpotlight.enabled = false;

        ArcadeScreenEffect flickerEffect = GetComponent<ArcadeScreenEffect>();
        if (flickerEffect != null) flickerEffect.enabled = false;
        if (scareAudioSource != null) StartCoroutine(PlayScareSoundRoutine());
    }

    private IEnumerator PlayScareSoundRoutine()
    {
        yield return new WaitForSeconds(scareSoundDelay);
        scareAudioSource.Play();
    }
}