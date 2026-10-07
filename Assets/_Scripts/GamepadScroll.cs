using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ScrollRect))]
public class GamepadScroll : MonoBehaviour
{
    public float scrollSpeed = 1.5f;
    
    private ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        if (Gamepad.current != null)
        {
            float scrollInput = Gamepad.current.rightStick.y.ReadValue();
            
            if (Mathf.Abs(scrollInput) > 0.1f)
            {
                scrollRect.verticalNormalizedPosition += scrollInput * scrollSpeed * Time.unscaledDeltaTime;
                
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
            }
        }
    }
}