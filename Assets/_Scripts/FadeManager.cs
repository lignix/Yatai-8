using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("References")]
    public Image fadeImage;

    [Header("Settings")]
    public float fadeOutDuration = 1.5f;
    public float fadeInDuration = 0.1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = Color.black;
        }

        StartCoroutine(StartGameFadeSequence());
    }

    private IEnumerator StartGameFadeSequence()
    {
        float timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
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

    public void FadeAndRestart()
    {
        StartCoroutine(FadeToSceneSequence(SceneManager.GetActiveScene().name, fadeInDuration));
    }

    public void FadeAndLoadScene(string sceneName, float duration)
    {
        StartCoroutine(FadeToSceneSequence(sceneName, duration));
    }

    private IEnumerator FadeToSceneSequence(string targetScene, float duration)
    {
        if (fadeImage != null) fadeImage.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / duration);
            
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }
            yield return null;
        }

        SceneManager.LoadScene(targetScene);
    }
}