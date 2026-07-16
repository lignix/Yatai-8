using UnityEngine;
using UnityEngine.UI;

public class EndgameFade : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform fadeTarget;
    public Image whiteFadeImage;

    [Header("Fade Settings")]
    public float startFadeDistance = 15f;
    public float endFadeDistance = 2f;


    public CreditsManager creditsManager;
    private bool isFinished = false;

    private void Start()
    {
        if (whiteFadeImage != null)
        {
            whiteFadeImage.gameObject.SetActive(true);
            Color c = whiteFadeImage.color;
            c.a = 0f;
            whiteFadeImage.color = c;
        }
    }

    private void Update()
    {
        if (isFinished || player == null || fadeTarget == null || whiteFadeImage == null) return;

        float distance = Vector3.Distance(player.position, fadeTarget.position);

        float linearAlpha = Mathf.InverseLerp(startFadeDistance, endFadeDistance, distance);
        float alpha = Mathf.Pow(linearAlpha, 3f);

        Color c = whiteFadeImage.color;
        c.a = alpha;
        whiteFadeImage.color = c;

        if (alpha >= 1f)
        {
            isFinished = true;
            TriggerCredits();
        }
    }

    private void TriggerCredits()
    {
        if (creditsManager != null) creditsManager.StartCreditsSequence();
    }
}