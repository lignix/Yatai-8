using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("References")]
    public Image fadeImage;
    public GameObject loadingIcon;

    [Header("Settings")]
    public float fadeOutDuration = 1.5f;
    public float fadeInDuration = 0.1f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (loadingIcon != null) loadingIcon.SetActive(false);

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = Color.black;
            fadeImage.raycastTarget = false;
            StartCoroutine(InitialFadeInRoutine());
        }
    }

    private IEnumerator InitialFadeInRoutine()
    {
        yield return null;
        yield return null;
        yield return null;

        float timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }
            yield return null;
        }

        if (fadeImage != null) fadeImage.gameObject.SetActive(false);
    }

    public void FadeAndRestart(float blackScreenPause = 0f)
    {
        if (isTransitioning) return;

        StopAllCoroutines();
        StartCoroutine(RealLoadingSequence(SceneManager.GetActiveScene().name, fadeInDuration, blackScreenPause));
    }

    public void FadeAndLoadScene(string sceneName, float duration, float blackScreenPause = 0f)
    {
        if (isTransitioning) return;

        StopAllCoroutines();
        StartCoroutine(RealLoadingSequence(sceneName, duration, blackScreenPause));
    }

    private IEnumerator RealLoadingSequence(string targetScene, float fadeDuration, float blackScreenPause)
    {
        isTransitioning = true;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.raycastTarget = true;
        }

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }
            yield return null;
        }

        if (fadeImage != null) fadeImage.color = Color.black;

        if (blackScreenPause > 0f)
        {
            yield return new WaitForSecondsRealtime(blackScreenPause);
        }

        if (loadingIcon != null) loadingIcon.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return null;
        yield return null;
        yield return null;

        if (loadingIcon != null) loadingIcon.SetActive(false);

        isTransitioning = false;
        if (fadeImage != null) fadeImage.raycastTarget = false;

        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }
            yield return null;
        }

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }
    }
}