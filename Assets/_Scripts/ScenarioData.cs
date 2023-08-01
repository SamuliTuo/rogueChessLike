using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenarioData
{
    public string scriptableScenarioFileName;
    public string saveName;
    public List<Scenario.ScenarioUnit> unitList = new List<Scenario.ScenarioUnit>();
    public List<Scenario.ScenarioNode> scenarioNodes = new List<Scenario.ScenarioNode>();
    public int boardSizeX;
    public int boardSizeY;

    public ScenarioData(Scenario scenario)
    {
        scriptableScenarioFileName = scenario.name;
        saveName = scenario.saveName;
        unitList = scenario.scenarioUnits;
        boardSizeX = scenario.sizeX;
        boardSizeY = scenario.sizeY;
        scenarioNodes = scenario.scenarioNodes;
    }
}