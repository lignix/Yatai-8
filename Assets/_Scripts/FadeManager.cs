using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("References")]
    public Image fadeImage;
    [Tooltip("L'objet contenant ton image ou texte de chargement")]
    public GameObject loadingIcon; 

    [Header("Settings")]
    public float fadeOutDuration = 1.5f; 
    public float fadeInDuration = 0.1f; 

    private bool isFading = false;

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

        if (fadeImage != null && !isFading)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = Color.black;
            StartCoroutine(InitialFadeInRoutine());
        }
    }

    private IEnumerator InitialFadeInRoutine()
    {
        isFading = true;
        
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
        isFading = false;
    }

    public void FadeAndRestart()
    {
        if (isFading) return;
        StartCoroutine(RealLoadingSequence(SceneManager.GetActiveScene().name, fadeInDuration));
    }

    public void FadeAndLoadScene(string sceneName, float duration)
    {
        if (isFading) return;
        StartCoroutine(RealLoadingSequence(sceneName, duration));
    }

    private IEnumerator RealLoadingSequence(string targetScene, float fadeDuration)
    {
        isFading = true;
        
        if (fadeImage != null) fadeImage.gameObject.SetActive(true);
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

        isFading = false;
    }
}