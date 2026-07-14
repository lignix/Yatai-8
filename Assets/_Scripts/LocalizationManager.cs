using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public LocalizationDatabase database;
    public int currentLanguageIndex = 0; // 0 = English, 1 = French

    public delegate void OnLanguageChanged();
    public static event OnLanguageChanged LanguageChangedEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentLanguageIndex = PlayerPrefs.GetInt("LanguagePref", 0); 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(int index)
    {
        currentLanguageIndex = index;
        LanguageChangedEvent?.Invoke();
    }

    public string GetTranslation(string key)
    {
        if (database == null) return key;

        foreach (var t in database.translations)
        {
            if (t.key == key)
            {
                return currentLanguageIndex == 0 ? t.english : t.french;
            }
        }
        return key; 
    }
}