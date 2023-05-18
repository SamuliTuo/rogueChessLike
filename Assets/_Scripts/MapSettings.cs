using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMapSettings", menuName = "ScriptableObjects/newMapSettings", order = 1)]
public class MapSettings : ScriptableObject
{
    //[System.Serializable]
    //public class Encounter
    //{
    //    public List<MapNodeType> type = new List<MapNodeType>();
    //}
    [Header("These are the floors between StartNode and EndNode")]
    public List<MapFloor> encountersByRow = new List<MapFloor>();
    public Encounter startEncounter;
    public List<Encounter> lastEncounters = new List<Encounter>();

    public Vector2 randomRangeForSplitsAtFirstNode = new Vector2(2, 4);
    public int maximumNodesWideness = 6;
    public float stepDistance_first = 0.5f;
    public float stepDistance = 1.5f;
    public float stepDistance_last = 0.5f;
    public float positionSideOffset = 1.5f;
    public float positionRandomFactor = 0.4f;
    public float splitChance = 0.3f;
    public float mergeChance = 0.5f;
}
