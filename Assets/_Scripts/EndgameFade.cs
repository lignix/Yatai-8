using UnityEngine;
using UnityEngine.UI;

public class EndgameFade : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform fadeTarget;
    public Image whiteFadeImage;
    public CreditsManager creditsManager;
    private Transform mainCameraTransform;

    [Header("Fade Settings")]
    public float startFadeDistance = 15f;
    public float endFadeDistance = 2f;
    public float fadeSmoothing = 3f; 

    private bool isFinished = false;
    private float currentAlpha = 0f;
    private PlayerController playerController;

    private void Start()
    {
        if (whiteFadeImage != null)
        {
            whiteFadeImage.gameObject.SetActive(true);
            SetImageAlpha(0f);
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (isFinished || player == null || fadeTarget == null || whiteFadeImage == null) return;

        float distance = Vector3.Distance(player.position, fadeTarget.position);

        float baseAlpha = Mathf.InverseLerp(startFadeDistance, endFadeDistance, distance);
        baseAlpha = Mathf.SmoothStep(0f, 1f, baseAlpha);

        float targetAlpha = baseAlpha;

        if (distance > endFadeDistance && mainCameraTransform != null)
        {
            Vector3 directionToTarget = (fadeTarget.position - mainCameraTransform.position).normalized;
            
            float lookDot = Vector3.Dot(mainCameraTransform.forward, directionToTarget);
            
            float lookFactor = Mathf.Clamp01((lookDot + 1f) / 2f);

            lookFactor = Mathf.Pow(lookFactor, 2f);

            targetAlpha = baseAlpha * lookFactor;
        }
        else if (distance <= endFadeDistance)
        {
            targetAlpha = 1f;
        }

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSmoothing);

        SetImageAlpha(currentAlpha);

        if (currentAlpha >= 0.98f && distance <= endFadeDistance)
        {
            isFinished = true;
            SetImageAlpha(1f);
            TriggerEndgame();
        }
    }

    private void SetImageAlpha(float alpha)
    {
        Color c = whiteFadeImage.color;
        c.a = alpha;
        whiteFadeImage.color = c;
    }

    private void TriggerEndgame()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (creditsManager != null) 
        {
            creditsManager.StartCreditsSequence();
        }
    }
}