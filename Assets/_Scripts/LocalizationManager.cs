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