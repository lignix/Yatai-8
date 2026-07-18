using UnityEngine;

public class ArcadeScreenEffect : MonoBehaviour
{
    [Header("References")]
    public Renderer screenRenderer;
    public Light screenLight;

    [Header("Flicker Settings")]
    public float minIntensity = 0.6f;
    public float maxIntensity = 1.4f;
    public float flickerSpeed = 15f;

    private Material screenMaterial;
    private Color originalEmissionColor;
    private float baseLightIntensity;

    private void Start()
    {
        if (screenRenderer != null)
        {
            // Instantiates the material to avoid modifying the project asset globally
            screenMaterial = screenRenderer.material;

            if (screenMaterial.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = screenMaterial.GetColor("_EmissionColor");
            }
        }

        if (screenLight != null)
        {
            baseLightIntensity = screenLight.intensity;
        }
    }

    private void Update()
    {
        // Generates organic noise over time
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);

        // Maps the noise from 0-1 to the chosen multiplier range
        float currentMultiplier = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // Applies multiplier to the physical light
        if (screenLight != null)
        {
            screenLight.intensity = baseLightIntensity * currentMultiplier;
        }

        // Applies multiplier to the material's emission intensity
        if (screenMaterial != null && screenMaterial.HasProperty("_EmissionColor"))
        {
            screenMaterial.SetColor("_EmissionColor", originalEmissionColor * currentMultiplier);
        }
    }
}