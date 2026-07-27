using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown languageDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    public TMP_Dropdown fpsDropdown;

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

            // Filter out duplicate width x height entries
            if (!options.Contains(option))
            {
                filteredResolutions.Add(res);
                options.Add(option);

                if (res.width == Screen.currentResolution.width &&
                    res.height == Screen.currentResolution.height)
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
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("VolumePref", volume);
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

            // Refreshes the dropdown text when the language changes
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
            bool isVSync = PlayerPrefs.GetInt("VSyncPref", 0) == 1;
            vsyncToggle.SetIsOnWithoutNotify(isVSync);
            SetVSync(isVSync);
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
            // Replaces the first option's text with the localized string
            fpsDropdown.options[0].text = LocalizationManager.Instance.GetTranslation("ui_unlimited");

            // Forces the displayed text to update immediately if "Unlimited" is currently selected
            if (fpsDropdown.value == 0)
            {
                fpsDropdown.captionText.text = fpsDropdown.options[0].text;
            }
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
}