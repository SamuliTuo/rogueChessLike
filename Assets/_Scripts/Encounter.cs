using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "newEncounter", menuName = "ScriptableObjects/newEncounter", order = 1)]
public class Encounter : ScriptableObject
{
    public MapNodeType mapNodeType;

    [Header("If this is a random encounter, set the encounter text and image here")]
    public string encounterText;
    public Sprite encounterImage;

    [Header("If this is a battle encounter, set the battle scenario here")]
    public Scenario battleScenario;
}
