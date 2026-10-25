using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown languageDropdown;

    [Header("Slider Info Panel (Keyboard / Gamepad Guide)")]
    public GameObject sliderInfoPanel;
    private bool isUsingKeyboardOrGamepad = false;

    [Header("Volume")]
    public Slider volumeSlider;
    public TMP_Text volumeValueText;

    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    public TMP_Dropdown fpsDropdown;

    [Header("Graphics")]
    public TMP_Dropdown aaDropdown;

    [Header("QoL & Accessibility")]
    public Slider fovSlider;
    public TMP_Text fovValueText;
    public Toggle reticleToggle;
    public Toggle sprintToggle;

    public delegate void FOVChangedEvent(float newValue);
    public static event FOVChangedEvent OnFOVChanged;

    public delegate void ReticleChangedEvent(bool newValue);
    public static event ReticleChangedEvent OnReticleChanged;

    public delegate void SprintToggleChangedEvent(bool newValue);
    public static event SprintToggleChangedEvent OnSprintToggleChanged;

    [Header("Sensitivity")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityValueText;
    public delegate void SensitivityChangedEvent(float newValue);
    public static event SensitivityChangedEvent OnSensitivityChanged;

    private List<Resolution> filteredResolutions;
    private readonly int[] fpsLimits = { -1, 30, 60, 120, 144 };

    private void Update()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (
            UnityEngine.InputSystem.Keyboard.current != null
            && UnityEngine.InputSystem.Keyboard.current.f12Key.wasPressedThisFrame
        )
        {
            PlayerPrefs.DeleteAll();

            Debug.Log("[DevTool] Reloading game...");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
#endif

        DetectInputDevice();
        HandleSelectedSliderInput();
        UpdateSliderInfoPanel();
    }

    private void DetectInputDevice()
    {
        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f)
        {
            isUsingKeyboardOrGamepad = false;
        }

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            isUsingKeyboardOrGamepad = true;
        }

        if (Gamepad.current != null)
        {
            if (
                Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.1f
                || Gamepad.current.dpad.ReadValue().sqrMagnitude > 0.1f
                || Gamepad.current.buttonSouth.wasPressedThisFrame
                || Gamepad.current.buttonEast.wasPressedThisFrame
            )
            {
                isUsingKeyboardOrGamepad = true;
            }
        }
    }

    private void HandleSelectedSliderInput()
    {
        GameObject selectedObj =
            EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selectedObj == null)
            return;

        Slider slider = selectedObj.GetComponent<Slider>();
        if (slider == null)
            return;

        float step = (slider.maxValue - slider.minValue) * 0.05f;
        if (step <= 0f)
            step = 0.1f;

        float direction = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
                direction = -1f;
            else if (Keyboard.current.rKey.wasPressedThisFrame)
                direction = 1f;
        }

        if (Gamepad.current != null && direction == 0f)
        {
            if (
                Gamepad.current.leftShoulder.wasPressedThisFrame
                || Gamepad.current.leftTrigger.wasPressedThisFrame
            )
                direction = -1f;
            else if (
                Gamepad.current.rightShoulder.wasPressedThisFrame
                || Gamepad.current.rightTrigger.wasPressedThisFrame
            )
                direction = 1f;
        }

        if (direction != 0f)
        {
            slider.value = Mathf.Clamp(
                slider.value + direction * step,
                slider.minValue,
                slider.maxValue
            );
        }
    }

    private void UpdateSliderInfoPanel()
    {
        if (sliderInfoPanel == null)
            return;

        GameObject selectedObj =
            EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        bool isSliderSelected = selectedObj != null && selectedObj.GetComponent<Slider>() != null;

        bool shouldShow = isSliderSelected && isUsingKeyboardOrGamepad;
        if (sliderInfoPanel.activeSelf != shouldShow)
        {
            sliderInfoPanel.SetActive(shouldShow);
        }
    }

    private void Start()
    {
        SetupFullscreen();
        SetupResolutions();
        SetupVolume();
        SetupLanguage();
        SetupVSync();
        SetupFPS();
        SetupAA();
        SetupSensitivity();
        SetupQoL();
        if (sliderInfoPanel != null)
            sliderInfoPanel.SetActive(false);
    }

    private void SetupQoL()
    {
        if (fovSlider != null)
        {
            float savedFov = PlayerPrefs.GetFloat("FOVPref", 75f);
            fovSlider.SetValueWithoutNotify(savedFov);
            UpdateFOVText(savedFov);
        }

        if (reticleToggle != null)
        {
            bool isReticleOn = PlayerPrefs.GetInt("ReticlePref", 0) == 1;
            reticleToggle.SetIsOnWithoutNotify(isReticleOn);
        }

        if (sprintToggle != null)
        {
            bool isSprintToggleOn = PlayerPrefs.GetInt("SprintTogglePref", 0) == 1;
            sprintToggle.SetIsOnWithoutNotify(isSprintToggleOn);
        }
    }

    public void SetFOV(float value)
    {
        PlayerPrefs.SetFloat("FOVPref", value);
        UpdateFOVText(value);
        OnFOVChanged?.Invoke(value);
    }

    private void UpdateFOVText(float value)
    {
        if (fovValueText != null)
            fovValueText.text = Mathf.RoundToInt(value).ToString();
    }

    public void SetReticle(bool isOn)
    {
        PlayerPrefs.SetInt("ReticlePref", isOn ? 1 : 0);
        OnReticleChanged?.Invoke(isOn);
    }

    public void SetSprintToggle(bool isOn)
    {
        PlayerPrefs.SetInt("SprintTogglePref", isOn ? 1 : 0);
        OnSprintToggleChanged?.Invoke(isOn);
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

            float aspectRatio = (float)res.width / res.height;
            if (Mathf.Abs(aspectRatio - (16f / 9f)) > 0.05f)
            {
                continue;
            }

            string option = res.width + " x " + res.height;

            if (!options.Contains(option))
            {
                filteredResolutions.Add(res);
                options.Add(option);

                if (
                    res.width == Screen.currentResolution.width
                    && res.height == Screen.currentResolution.height
                )
                {
                    currentResIndex = filteredResolutions.Count - 1;
                }
            }
        }

        if (options.Count == 0)
        {
            filteredResolutions.Add(new Resolution { width = 1920, height = 1080 });
            options.Add("1920 x 1080");
            currentResIndex = 0;
        }

        resolutionDropdown.AddOptions(options);
        int defaultResIndex = filteredResolutions.Count - 1;

        int savedRes = PlayerPrefs.GetInt("ResolutionPref", defaultResIndex);

        if (savedRes >= filteredResolutions.Count)
            savedRes = defaultResIndex;

        resolutionDropdown.SetValueWithoutNotify(savedRes);
        resolutionDropdown.RefreshShownValue();
        SetResolution(savedRes);
    }

    public void SetResolution(int index)
    {
        Resolution res = filteredResolutions[index];
        bool isFullscreen = PlayerPrefs.GetInt("FullscreenPref", 1) == 1;

        Screen.SetResolution(res.width, res.height, isFullscreen);
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
        PlayerPrefs.SetInt("FullscreenPref", isFullscreen ? 1 : 0);
        Screen.fullScreen = isFullscreen;

        if (filteredResolutions != null && filteredResolutions.Count > 0)
        {
            int savedRes = PlayerPrefs.GetInt("ResolutionPref", filteredResolutions.Count - 1);
            Resolution res = filteredResolutions[savedRes];
            Screen.SetResolution(res.width, res.height, isFullscreen);
        }
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
        if (
            fpsDropdown != null
            && fpsDropdown.options.Count > 0
            && LocalizationManager.Instance != null
        )
        {
            fpsDropdown.options[0].text = LocalizationManager.Instance.GetTranslation(
                "ui_unlimited"
            );
            if (fpsDropdown.value == 0)
                fpsDropdown.captionText.text = fpsDropdown.options[0].text;
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
            UniversalAdditionalCameraData camData =
                targetCam.GetComponent<UniversalAdditionalCameraData>();
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
