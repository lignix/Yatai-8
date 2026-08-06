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

    private void Awake()
    {
        foreach (Light l in lanterneLights)
        {
            if (l != null)
            {
                baseIntensities[l] = l.intensity;
            }
        }
    }

    private void OnEnable()
    {
        hasTriggered = false;
        
        foreach (Light l in lanterneLights)
        {
            if (l != null && baseIntensities.ContainsKey(l))
            {
                l.intensity = baseIntensities[l];
            }
        }

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        if (!hasTriggered)
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance <= triggerDistance)
            {
                hasTriggered = true;
            }
        }

        if (hasTriggered)
        {
            ApplyFlicker();
        }
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