using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("References")]
    public Image fullScreenImage;
    public RectTransform creditsPanel;
    
    [Header("UI Elements")]
    public GameObject skipButton; 
    private CanvasGroup skipButtonCanvasGroup;

    [Header("Timings")]
    public float fadeToBlackDuration = 2f;
    public float waitBeforeCredits = 1f;
    public float delayAfterScroll = 3f;

    [Header("Scrolling Settings")]
    public float scrollSpeed = 100f;
    public float endPositionY = 2500f;

    private float initialVolume;

    private void Start()
    {
        if (skipButton != null)
        {
            skipButtonCanvasGroup = skipButton.GetComponent<CanvasGroup>();
            
            if (skipButtonCanvasGroup == null)
            {
                skipButtonCanvasGroup = skipButton.AddComponent<CanvasGroup>();
            }

            skipButtonCanvasGroup.alpha = 0f;
            skipButtonCanvasGroup.interactable = false;
            skipButtonCanvasGroup.blocksRaycasts = false;
            
            skipButton.SetActive(false);
        }
    }

    public void StartCreditsSequence()
    {
        initialVolume = AudioListener.volume;
        StartCoroutine(CreditsRoutine());
    }

    private IEnumerator CreditsRoutine()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.UnlockAchievement("end");
        }
        
        float timer = 0f;
        Color startColor = fullScreenImage.color;
        Color targetColor = Color.black;

        if (skipButton != null)
        {
            skipButton.SetActive(true);
        }

        while (timer < fadeToBlackDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeToBlackDuration;
            
            fullScreenImage.color = Color.Lerp(startColor, targetColor, progress);
            AudioListener.volume = Mathf.Lerp(initialVolume, 0f, progress);

            if (skipButtonCanvasGroup != null)
            {
                skipButtonCanvasGroup.alpha = progress;
            }

            yield return null;
        }
        
        fullScreenImage.color = targetColor;
        AudioListener.volume = 0f;

        if (skipButtonCanvasGroup != null)
        {
            skipButtonCanvasGroup.alpha = 1f;
            skipButtonCanvasGroup.interactable = true;
            skipButtonCanvasGroup.blocksRaycasts = true;
        }

        yield return new WaitForSeconds(waitBeforeCredits);

        if (creditsPanel != null)
        {
            creditsPanel.gameObject.SetActive(true);

            while (creditsPanel.anchoredPosition.y < endPositionY)
            {
                creditsPanel.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);
                yield return null;
            }
        }

        yield return new WaitForSeconds(delayAfterScroll);

        LoadMenu();
    }

    public void SkipCredits()
    {
        StopAllCoroutines();
        LoadMenu();
    }

    private void LoadMenu()
    {
        AudioListener.volume = initialVolume;
        SceneManager.LoadScene("Menu");
    }
}