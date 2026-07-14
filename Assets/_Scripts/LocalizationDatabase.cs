using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationDatabase", menuName = "Game/Localization Database")]
public class LocalizationDatabase : ScriptableObject
{
    [System.Serializable]
    public class Translation
    {
        public string key;
        public string english;
        public string french;
    }

    public List<Translation> translations = new List<Translation>();
}