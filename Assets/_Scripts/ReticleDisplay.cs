using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ReticleDisplay : MonoBehaviour
{
    private Image reticleImage;

    private void Awake()
    {
        reticleImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        SettingsManager.OnReticleChanged += UpdateReticle;
        UpdateReticle(PlayerPrefs.GetInt("ReticlePref", 0) == 1); 
    }

    private void OnDisable()
    {
        SettingsManager.OnReticleChanged -= UpdateReticle;
    }

    private void UpdateReticle(bool show)
    {
        if (reticleImage != null) reticleImage.enabled = show;
    }
}