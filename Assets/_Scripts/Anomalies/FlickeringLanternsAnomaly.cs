using System.Collections.Generic;
using UnityEngine;

public class FlickeringLanternsAnomaly : MonoBehaviour
{
    [Header("References")]
    public List<Light> lanterneLights = new List<Light>();

    [Header("Detection Settings")]
    public float triggerDistance = 2.0f;

    [Header("Flicker Settings")]
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 15f;

    private Transform playerTransform;
    private bool hasTriggered = false;
    private Dictionary<Light, float> baseIntensities = new Dictionary<Light, float>();
    private float sqrTriggerDistance;

    private void Awake()
    {
        foreach (Light l in lanterneLights)
        {
            if (l != null) baseIntensities[l] = l.intensity;
        }
        sqrTriggerDistance = triggerDistance * triggerDistance;
    }

    private void OnEnable()
    {
        hasTriggered = false;
        
        foreach (Light l in lanterneLights)
        {
            if (l != null && baseIntensities.ContainsKey(l)) l.intensity = baseIntensities[l];
        }

        if (playerTransform == null && PlayerController.InstanceTransform != null)
        {
            playerTransform = PlayerController.InstanceTransform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        if (!hasTriggered)
        {
            if ((playerTransform.position - transform.position).sqrMagnitude <= sqrTriggerDistance)
            {
                hasTriggered = true;
            }
        }

        if (hasTriggered) ApplyFlicker();
    }

    private void ApplyFlicker()
    {
        for (int i = 0; i < lanterneLights.Count; i++)
        {
            Light l = lanterneLights[i];
            if (l == null) continue;

            float noise = Mathf.PerlinNoise((Time.time + i * 10f) * flickerSpeed, 0f);
            float currentMultiplier = Mathf.Lerp(minIntensity, maxIntensity, noise);

            if (baseIntensities.TryGetValue(l, out float originalIntensity))
            {
                l.intensity = originalIntensity * currentMultiplier;
            }
        }
    }

    private void OnDisable()
    {
        hasTriggered = false;
    }
}