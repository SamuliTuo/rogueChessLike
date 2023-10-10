using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTextEncounter", menuName = "ScriptableObjects/newTextEncounter", order = 1)]
public class TextEncounter : ScriptableObject
{
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
    [HideInInspector] public UnitData attemptingUnit;
}
[System.Serializable]
public class ResponseRewards
{
    public int money;
    public UnitData unit;
    [Tooltip("Experience is given to the units that succeeded in a die-roll.")]
    public float experience;
}   