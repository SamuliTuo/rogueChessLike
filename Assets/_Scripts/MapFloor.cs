using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMapFloor", menuName = "ScriptableObjects/MapFloor", order = 1)]
[System.Serializable]
public class MapFloor : ScriptableObject
{
    //public List<MapNodeType> type;
    public List<Encounter> encounters;
    public Dictionary<string, int> rewards;
}