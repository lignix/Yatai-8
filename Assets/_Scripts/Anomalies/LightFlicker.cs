using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LightFlicker : MonoBehaviour
{
    [Header("Material References")]
    public Material lightOnMaterial;
    public Material lightOffMaterial;

    [Header("Flicker Settings")]
    public float minDelay = 0.05f;
    public float maxDelay = 0.3f;

    private Renderer targetRenderer;
    private bool isLightOn = true;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(FlickerRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Calculate a random time interval before the next state change
            float currentDelay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(currentDelay);

            // Toggle state and apply material
            isLightOn = !isLightOn;
            targetRenderer.material = isLightOn ? lightOnMaterial : lightOffMaterial;
        }
    }
}