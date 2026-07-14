using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnomalyDatabase", menuName = "Game/Anomaly Database")]
public class AnomalyDatabase : ScriptableObject
{
    public List<string> anomalyKeys = new List<string>();
}