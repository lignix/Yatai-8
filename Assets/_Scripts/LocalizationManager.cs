using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public LocalizationDatabase database;
    public int currentLanguageIndex = 0; 

    public delegate void OnLanguageChanged();
    public static event OnLanguageChanged LanguageChangedEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // if first time running game
            if (!PlayerPrefs.HasKey("LanguagePref"))
            {
                currentLanguageIndex = GetSystemLanguageIndex();
                PlayerPrefs.SetInt("LanguagePref", currentLanguageIndex);
            }
            else
            {
                currentLanguageIndex = PlayerPrefs.GetInt("LanguagePref", 0); 
            }
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int GetSystemLanguageIndex()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.French: return 1;
            case SystemLanguage.Spanish: return 2;
            case SystemLanguage.German: return 3;
            case SystemLanguage.Japanese: return 4;
            case SystemLanguage.ChineseSimplified: return 5;
            case SystemLanguage.Chinese: return 5;
            default: return 0;
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
                switch (currentLanguageIndex)
                {
                    case 0: return t.english;
                    case 1: return t.french;
                    case 2: return string.IsNullOrEmpty(t.spanish) ? t.english : t.spanish;
                    case 3: return string.IsNullOrEmpty(t.german) ? t.english : t.german;
                    case 4: return string.IsNullOrEmpty(t.japanese) ? t.english : t.japanese;
                    case 5: return string.IsNullOrEmpty(t.simplifiedChinese) ? t.english : t.simplifiedChinese;
                    default: return t.english;
                }
            }
        }
        return key; 
    }
}