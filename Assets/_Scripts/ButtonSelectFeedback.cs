using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Settings")]
    public float scaleFactor = 1.25f;
    public float smoothSpeed = 15f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
    }

    private void OnDisable()
    {
        targetScale = originalScale;
        transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData) => targetScale = originalScale * scaleFactor;
    public void OnPointerExit(PointerEventData eventData) => targetScale = originalScale;
    public void OnSelect(BaseEventData eventData) => targetScale = originalScale * scaleFactor;
    public void OnDeselect(BaseEventData eventData) => targetScale = originalScale;
}