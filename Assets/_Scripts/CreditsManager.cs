using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    [Tooltip("Temps d'attente avant l'apparition du bouton Skip")]
    public float skipButtonDelay = 3f;

    [Header("Scrolling Settings")]
    public float scrollSpeed = 100f;
    [Tooltip("Multiplicateur de vitesse quand on maintient le clic gauche")]
    public float fastScrollMultiplier = 4f;
    [Tooltip("La distance de départ de ton panneau en Y (ex: 1080)")]
    public float startOffsetY = 1080f;
    [Tooltip("Marge ajoutée à la hauteur du panneau pour s'assurer que le texte sort bien de l'écran")]
    public float endPadding = 200f;

    private float initialVolume;
    private bool isSkipping = false;

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

        if (skipButton != null)
        {
            StartCoroutine(ShowSkipButtonRoutine());
        }
    }

    private IEnumerator ShowSkipButtonRoutine()
    {
        yield return new WaitForSeconds(skipButtonDelay);

        if (isSkipping) yield break;

        skipButton.SetActive(true);
        if (skipButtonCanvasGroup != null)
        {
            skipButtonCanvasGroup.alpha = 1f;
            skipButtonCanvasGroup.interactable = true;
            skipButtonCanvasGroup.blocksRaycasts = true;
        }
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

        while (timer < fadeToBlackDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeToBlackDuration;

            if (fullScreenImage != null)
            {
                fullScreenImage.color = Color.Lerp(startColor, targetColor, progress);
            }
            AudioListener.volume = Mathf.Lerp(initialVolume, 0f, progress);

            yield return null;
        }

        if (fullScreenImage != null)
        {
            fullScreenImage.color = targetColor;
        }
        AudioListener.volume = 0f;

        yield return new WaitForSeconds(waitBeforeCredits);

        if (creditsPanel != null)
        {
            creditsPanel.gameObject.SetActive(true);

            float dynamicEndY = creditsPanel.rect.height - startOffsetY + endPadding;

            while (creditsPanel.anchoredPosition.y < dynamicEndY)
            {
                float currentSpeed = scrollSpeed;

                bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

                if (Mouse.current != null && Mouse.current.leftButton.isPressed && !isPointerOverUI)
                {
                    currentSpeed *= fastScrollMultiplier;
                }

                creditsPanel.anchoredPosition += Vector2.up * (currentSpeed * Time.deltaTime);
                yield return null;
            }
        }

        yield return new WaitForSeconds(delayAfterScroll);

        LoadMenu();
    }

    public void SkipCredits()
    {
        LoadMenu();
    }

    private void LoadMenu()
    {
        if (isSkipping) return;
        isSkipping = true;

        StopAllCoroutines();

        AudioListener.volume = 0f;

        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeAndLoadScene("Menu", 1f);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }
}