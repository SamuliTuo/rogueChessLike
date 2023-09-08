using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTextEncounter", menuName = "ScriptableObjects/newTextEncounter", order = 1)]
public class TextEncounter : ScriptableObject
{
    public Sprite image;
    public string textPrompt;

    public List<ResponseAndReward> responsesWithRewards = new List<ResponseAndReward>();
}


[System.Serializable]
public class ResponseAndReward
{
    public string response;
    public TextScenarioReward reward;
}

[System.Serializable]
public class TextScenarioReward
{

}