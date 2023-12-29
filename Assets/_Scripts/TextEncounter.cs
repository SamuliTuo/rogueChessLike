using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTextEncounter", menuName = "ScriptableObjects/newTextEncounter", order = 1)]
public class TextEncounter : ScriptableObject
{
    [Header("Make the lists 10 long,  first unit goes in first slot etc.")]
    [Space(10)]
    [Header("Positions between (-1,-1) and (1,1) - it corresponds to scenario images bounds.")]
    public List<Vector2> playerPositions = new List<Vector2>();
    public List<Vector3> playerForwardVectors = new List<Vector3>();
    public List<Vector3> playerScales = new List<Vector3>();
    [Space(32)]
    public Sprite image;
    public string textPrompt;
    public List<ResponseRequirementsAndReward> responses = new List<ResponseRequirementsAndReward>();
}


[System.Serializable]
public class ResponseRequirementsAndReward
{
    public string response;
    public string successPrompt;
    public string failPrompt;
    
    public ResponseRequirements requirements;
    public ResponseRewards reward;
}


[System.Serializable]
public class ResponseRequirements
{
    public int money;

    [Tooltip("Roll a d20, pass with rolling 'minimumRoll' or above. Leave empty if no die-roll needed")]
    [Range(0, 20)] public int minimumRoll;
    public bool needsUnit;
    [Header("If unit is needed, what stats can give bonus to check?")]
    public UnitTextEncounterCheckableStats statCheck_main;
    public UnitTextEncounterCheckableStats statCheck_minor1;
    public UnitTextEncounterCheckableStats statCheck_minor2;
    public UnitTextEncounterCheckableStats statCheck_minor3;
    [HideInInspector] public UnitData attemptingUnit;
}
[System.Serializable]
public class ResponseRewards
{
    public int money;
    public UnitData unit;
    [Tooltip("Experience is given to the units that succeeded in a die-roll.")]
    public float experience;
    [Tooltip("Use battleOnSuccess when you want to battle on successfull die-roll.")]
    public Scenario battleOnSuccess;
    [Tooltip("Use battleOnFailureOrAnyCase for battle initiated by failed rolls, BUT also if you want to always activate a battle.")]
    public Scenario battleOnFailureOrAnyCase;
}   