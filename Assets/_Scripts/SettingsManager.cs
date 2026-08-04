using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown languageDropdown;
    
    [Header("Volume")]
    public Slider volumeSlider;
    public TMP_Text volumeValueText;

    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    public TMP_Dropdown fpsDropdown;

    [Header("Graphics")]
    public TMP_Dropdown aaDropdown;
    
    [Header("Sensitivity")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityValueText;
    public delegate void SensitivityChangedEvent(float newValue);
    public static event SensitivityChangedEvent OnSensitivityChanged;

    private List<Resolution> filteredResolutions;
    private readonly int[] fpsLimits = { -1, 30, 60, 120, 144 };

    private void Start()
    {
        SetupResolutions();
        SetupVolume();
        SetupLanguage();
        SetupFullscreen();
        SetupVSync();
        SetupFPS();
        SetupAA();
        SetupSensitivity();
    }

    private void SetupResolutions()
    {
        Resolution[] rawResolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < rawResolutions.Length; i++)
        {
            Resolution res = rawResolutions[i];
            string option = res.width + " x " + res.height;

            if (!options.Contains(option))
            {
                filteredResolutions.Add(res);
                options.Add(option);

                if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                {
                    currentResIndex = filteredResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        int savedRes = PlayerPrefs.GetInt("ResolutionPref", currentResIndex);
        if (savedRes >= filteredResolutions.Count) savedRes = currentResIndex;

        resolutionDropdown.SetValueWithoutNotify(savedRes);
        resolutionDropdown.RefreshShownValue();
        SetResolution(savedRes);
    }

    public void SetResolution(int index)
    {
        Resolution res = filteredResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionPref", index);
    }

    private void SetupVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("VolumePref", 1f);
        volumeSlider.SetValueWithoutNotify(savedVolume);
        AudioListener.volume = savedVolume;
        UpdateVolumeText(savedVolume);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("VolumePref", volume);
        UpdateVolumeText(volume);
    }

    private void UpdateVolumeText(float volume)
    {
        if (volumeValueText != null)
        {
            volumeValueText.text = Mathf.RoundToInt(volume * 100f) + "%";
        }
    }

    private void SetupLanguage()
    {
        int savedLang = PlayerPrefs.GetInt("LanguagePref", 0);
        languageDropdown.SetValueWithoutNotify(savedLang);
        languageDropdown.RefreshShownValue();
    }

    public void SetLanguage(int index)
    {
        PlayerPrefs.SetInt("LanguagePref", index);
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage(index);
            UpdateFPSDropdownText();
        }
    }

    private void SetupFullscreen()
    {
        if (fullscreenToggle != null)
        {
            bool isFullscreen = PlayerPrefs.GetInt("FullscreenPref", 1) == 1;
            fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
            Screen.fullScreen = isFullscreen;
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenPref", isFullscreen ? 1 : 0);
    }

    private void SetupVSync()
    {
        if (vsyncToggle != null)
        {
            bool isSync = PlayerPrefs.GetInt("VSyncPref", 0) == 1;
            vsyncToggle.SetIsOnWithoutNotify(isSync);
            SetVSync(isSync);
        }
    }

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        PlayerPrefs.SetInt("VSyncPref", isVSync ? 1 : 0);
    }

    private void SetupFPS()
    {
        if (fpsDropdown != null)
        {
            UpdateFPSDropdownText();
            int savedFPSIndex = PlayerPrefs.GetInt("FPSPref", 0);
            fpsDropdown.SetValueWithoutNotify(savedFPSIndex);
            fpsDropdown.RefreshShownValue();
            SetFPSLimit(savedFPSIndex);
        }
    }

    private void UpdateFPSDropdownText()
    {
        if (fpsDropdown != null && fpsDropdown.options.Count > 0 && LocalizationManager.Instance != null)
        {
            fpsDropdown.options[0].text = LocalizationManager.Instance.GetTranslation("ui_unlimited");
            if (fpsDropdown.value == 0) fpsDropdown.captionText.text = fpsDropdown.options[0].text;
        }
    }

    public void SetFPSLimit(int index)
    {
        if (index >= 0 && index < fpsLimits.Length)
        {
            Application.targetFrameRate = fpsLimits[index];
            PlayerPrefs.SetInt("FPSPref", index);
        }
    }

    private void SetupAA()
    {
        if (aaDropdown != null)
        {
            int savedAA = PlayerPrefs.GetInt("AAPref", 1); 
            aaDropdown.SetValueWithoutNotify(savedAA);
            aaDropdown.RefreshShownValue();
            SetAA(savedAA);
        }
    }

    public void SetAA(int index)
    {
        PlayerPrefs.SetInt("AAPref", index);
        
        Camera targetCam = Camera.main != null ? Camera.main : FindAnyObjectByType<Camera>();
        if (targetCam != null)
        {
            UniversalAdditionalCameraData camData = targetCam.GetComponent<UniversalAdditionalCameraData>();
            if (camData != null)
            {
                camData.antialiasing = (AntialiasingMode)index;
            }
        }
    }

    private void SetupSensitivity()
    {
        if (sensitivitySlider != null)
        {
            float savedSens = PlayerPrefs.GetFloat("SensitivityPref", 1f); 
            sensitivitySlider.SetValueWithoutNotify(savedSens);
            UpdateSensitivityText(savedSens);
        }
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("SensitivityPref", value);
        UpdateSensitivityText(value);
        OnSensitivityChanged?.Invoke(value);
    }

    private void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = value.ToString("F1");
        }
    }
}