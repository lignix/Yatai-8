using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Settings")]
    public float scaleFactor = 1.25f;
    public float smoothSpeed = 15f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private bool isHovered = false;
    private bool isSelected = false;

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
        isHovered = false;
        isSelected = false;
        targetScale = originalScale;
        transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData) { isHovered = true; UpdateScale(); }
    public void OnPointerExit(PointerEventData eventData) { isHovered = false; UpdateScale(); }
    public void OnSelect(BaseEventData eventData) { isSelected = true; UpdateScale(); }
    public void OnDeselect(BaseEventData eventData) { isSelected = false; UpdateScale(); }

    private void UpdateScale()
    {
        if (isHovered || isSelected) targetScale = originalScale * scaleFactor;
        else targetScale = originalScale;
    }
}