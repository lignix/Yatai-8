using UnityEngine;
using StarterAssets; 

[RequireComponent(typeof(FirstPersonController))]
public class SensitivityApplier : MonoBehaviour
{
    private FirstPersonController controller;

    private void Awake()
    {
        controller = GetComponent<FirstPersonController>();
    }

    private void Start()
    {
        ApplySavedSensitivity(PlayerPrefs.GetFloat("SensitivityPref", 1f));
    }

    private void OnEnable()
    {
        SettingsManager.OnSensitivityChanged += ApplySavedSensitivity;
    }

    private void OnDisable()
    {
        SettingsManager.OnSensitivityChanged -= ApplySavedSensitivity;
    }

    private void ApplySavedSensitivity(float newSensitivity)
    {
        if (controller != null)
        {
            controller.RotationSpeed = newSensitivity;
        }
    }
}