using System.Collections;
using UnityEngine;

public class SecretRoomAmbiance : MonoBehaviour
{
    public AudioSource[] outsideSounds;

    public AudioSource classroomAmbiance;

    [Header("Settings")]
    public float fadeDuration = 2.5f;

    public GameObject blockerCollider;
    private bool hasTriggered = false;

    private void Start()
    {
        if (blockerCollider != null)
        {
            blockerCollider.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(TransitionAmbianceRoutine());
            if (blockerCollider != null)
            {
                blockerCollider.SetActive(true);
            }
        }
    }

    private IEnumerator TransitionAmbianceRoutine()
    {
        float classTargetVol = 0f;
        if (classroomAmbiance != null)
        {
            classTargetVol = classroomAmbiance.volume;
            classroomAmbiance.volume = 0f;
            classroomAmbiance.Play();
        }

        float[] startVolumes = new float[outsideSounds.Length];
        for (int i = 0; i < outsideSounds.Length; i++)
        {
            if (outsideSounds[i] != null) 
            {
                startVolumes[i] = outsideSounds[i].volume;
            }
        }

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            for (int i = 0; i < outsideSounds.Length; i++)
            {
                if (outsideSounds[i] != null)
                {
                    outsideSounds[i].volume = Mathf.Lerp(startVolumes[i], 0f, progress);
                }
            }

            if (classroomAmbiance != null)
            {
                classroomAmbiance.volume = Mathf.Lerp(0f, classTargetVol, progress);
            }

            yield return null;
        }

        foreach (AudioSource audio in outsideSounds)
        {
            if (audio != null) audio.Stop();
        }
    }
}