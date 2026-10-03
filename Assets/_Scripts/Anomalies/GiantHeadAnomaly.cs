using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GiantHeadAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform headTransform; 

    [Header("Settings")]
    public float moveDistanceX = 2f;
    public float moveDuration = 0.5f;

    private AudioSource audioSource;
    private Vector3 initialLocalPosition;
    private bool hasTriggered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (headTransform != null)
        {
            initialLocalPosition = headTransform.localPosition;
        }

        // S'assure que ce GameObject a bien un Trigger
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnEnable()
    {
        hasTriggered = false;
        if (headTransform != null)
        {
            headTransform.localPosition = initialLocalPosition;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            
            if (audioSource != null)
            {
                audioSource.Play();
            }

            if (headTransform != null)
            {
                StartCoroutine(LungeHeadRoutine());
            }
        }
    }

    private IEnumerator LungeHeadRoutine()
    {
        Vector3 startPos = headTransform.localPosition;
        Vector3 targetPos = startPos + new Vector3(moveDistanceX, 0f, 0f);
        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / moveDuration;
            
            float easeOutProgress = Mathf.Sin(progress * Mathf.PI * 0.5f);
            
            headTransform.localPosition = Vector3.Lerp(startPos, targetPos, easeOutProgress);
            
            yield return null;
        }

        headTransform.localPosition = targetPos;
    }
}