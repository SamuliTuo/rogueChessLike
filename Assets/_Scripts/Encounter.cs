using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "newEncounter", menuName = "ScriptableObjects/newEncounter", order = 1)]
public class Encounter : ScriptableObject
{
    public List<TextEncounter> possibleTextScenarios;
    public List<Scenario> possibleBattleScenarios;
}
