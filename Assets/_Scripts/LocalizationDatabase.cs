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
        public string spanish;
        public string german;
        public string japanese;
        public string simplifiedChinese;
    }

    public List<Translation> translations = new List<Translation>();
}