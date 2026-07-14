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

    private Resolution[] resolutions;

    private void Start()
    {
        SetupResolutions();
        SetupVolume();
        SetupLanguage();
        SetupFullscreen();
    }

    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        
        int savedRes = PlayerPrefs.GetInt("ResolutionPref", currentResIndex);
        resolutionDropdown.SetValueWithoutNotify(savedRes);
        resolutionDropdown.RefreshShownValue();
        
        SetResolution(savedRes);
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
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
}