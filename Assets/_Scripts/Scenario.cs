using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewScenario", order = 3)]
public class Scenario : ScriptableObject
{
    [System.Serializable]
    public class ScenarioUnit
    {
        public GameObject unit;
        public Vector2Int spawnPos;
        public int team;
        public void Init(GameObject unit, Vector2Int spawnPos, int team)
        {
            this.unit = unit;
            this.spawnPos = spawnPos;
            this.team = team;
        }
    }
    public List<ScenarioUnit> scenarioUnits = new List<ScenarioUnit>();

    public void SaveScenario()
    {
        scenarioUnits.Clear();
        Unit[,] activeUnits = Chessboard.Instance.GetUnits();
        for (int x = 0; x < activeUnits.GetLength(0); x++)
        {
            for (int y = 0; y < activeUnits.GetLength(1); y++)
            {
                if (activeUnits[x, y] != null)
                {
                    var obj = ScenarioBuilder.Instance.GetOriginalUnitType_From_InstantiatedUnitObject(activeUnits[x, y].gameObject);
                    var clone = new ScenarioUnit();
                    clone.Init(obj, new Vector2Int(x, y), activeUnits[x, y].team);
                    scenarioUnits.Add(clone);
                }
            }
        }
    }
}
