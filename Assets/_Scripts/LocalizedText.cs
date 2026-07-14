using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string translationKey;
    private TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        UpdateText();
        LocalizationManager.LanguageChangedEvent += UpdateText;
    }

    private void OnDestroy()
    {
        LocalizationManager.LanguageChangedEvent -= UpdateText;
    }

    private void UpdateText()
    {
        if (LocalizationManager.Instance != null && textComponent != null)
        {
            textComponent.text = LocalizationManager.Instance.GetTranslation(translationKey);
        }
    }
}
