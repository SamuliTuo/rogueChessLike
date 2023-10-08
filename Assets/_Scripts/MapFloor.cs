using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMapFloor", menuName = "ScriptableObjects/MapFloor", order = 1)]
[System.Serializable]
public class MapFloor : ScriptableObject
{
    public List<Encounter> possibleEncounters;
}
