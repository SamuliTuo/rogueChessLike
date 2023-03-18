using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenarioData
{
    public string scriptedScenarioName;
    public List<Scenario.ScenarioUnit> unitList = new List<Scenario.ScenarioUnit>();

    public ScenarioData(Scenario scenario)
    {
        scriptedScenarioName = scenario.name;
        unitList = scenario.scenarioUnits;
    }
}