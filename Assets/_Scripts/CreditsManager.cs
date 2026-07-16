using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("References")]
    public Image fullScreenImage;
    public RectTransform creditsPanel;
    public Behaviour[] componentsToDisable;

    [Header("Timings")]
    public float fadeToBlackDuration = 2f;
    public float waitBeforeCredits = 1f;
    public float delayAfterScroll = 3f;

    [Header("Scrolling Settings")]
    public float scrollSpeed = 100f;
    public float endPositionY = 2500f;

    public void StartCreditsSequence()
    {
        foreach (Behaviour comp in componentsToDisable)
        {
            if (comp != null)
            {
                comp.enabled = false;
            }
        }

        StartCoroutine(CreditsRoutine());
    }

    private IEnumerator CreditsRoutine()
    {
        float timer = 0f;
        Color startColor = fullScreenImage.color;
        Color targetColor = Color.black;

        while (timer < fadeToBlackDuration)
        {
            timer += Time.deltaTime;
            fullScreenImage.color = Color.Lerp(startColor, targetColor, timer / fadeToBlackDuration);
            yield return null;
        }
        fullScreenImage.color = targetColor;

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

        SceneManager.LoadScene("Menu");
    }
}