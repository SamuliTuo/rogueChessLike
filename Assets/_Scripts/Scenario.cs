using JetBrains.Annotations;
using System.Collections.Generic;
using System.Drawing.Printing;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewScenario", order = 3)]
public class Scenario : ScriptableObject
{
    public string saveName;
    public int sizeX;
    public int sizeY;
    public List<ScenarioNode> scenarioNodes = new List<ScenarioNode>();
    public List<ScenarioUnit> scenarioUnits = new List<ScenarioUnit>();


    [System.Serializable]
    public class ScenarioNode
    {
        public int x;
        public int y;
        public int walkable;
        public string terrainLayer;
        public void Init(int x, int y, int walkable, string layerName)
        {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
            this.terrainLayer = layerName;
        }
    }

    [System.Serializable]
    public class ScenarioUnit
    {
        //public GameObject unit;
        public string unit;
        //public Vector2Int spawnPos;
        public int spawnPosX;
        public int spawnPosY;
        public int team;
        public void Init(string unit, int spawnPosX, int spawnPosY, int team)
        {
            this.unit = unit;
            this.spawnPosX = spawnPosX;
            this.spawnPosY = spawnPosY;
            this.team = team;
        }
    }

    public void SaveScenario(string saveName)
    {
        this.saveName = saveName;
        if (scenarioUnits == null)
            scenarioUnits = new List<ScenarioUnit>();
        else
            scenarioUnits.Clear();

        if (scenarioNodes == null)  
            scenarioNodes = new List<ScenarioNode>(); 
        else
            scenarioNodes.Clear();

        sizeX = Chessboard.Instance.GetBoardSize().x;
        sizeY = Chessboard.Instance.GetBoardSize().y;
        Unit[,] activeUnits = Chessboard.Instance.GetUnits();
        Node[,] nodes = Chessboard.Instance.nodes;
        

        // Add the units to the scenario file
        for (int x = 0; x < activeUnits.GetLength(0); x++) {
            for (int y = 0; y < activeUnits.GetLength(1); y++) {
                if (activeUnits[x, y] != null && x < sizeX && y < sizeY) {
                    var obj = ScenarioBuilder.Instance.GetOriginalUnitType_From_InstantiatedUnitObject(activeUnits[x, y].gameObject);
                    var clone = new ScenarioUnit();
                    clone.Init(obj.name, x, y, activeUnits[x, y].team);
                    scenarioUnits.Add(clone);
                }
            }
        }

        // Add nodes..
        for (int x = 0; x < nodes.GetLength(0); x++) {
            for (int y = 0; y < nodes.GetLength(1); y++) {
                if (nodes[x,y] != null && x < sizeX && y < sizeY) {
                    var clone = new ScenarioNode();
                    var walkable = nodes[x, y].walkable ? 1 : 0;
                    clone.Init(nodes[x,y].x, nodes[x,y].y, walkable, nodes[x,y].tileTypeLayerName);
                    scenarioNodes.Add(clone);
                }
            }
        }
    }

    public void RemoveUnit(int x, int y)
    {
        foreach (var unit in scenarioUnits)
        {
            if (unit.spawnPosX == x && unit.spawnPosY == y)
            {
                scenarioUnits.Remove(unit);
                break;
            }
        }
    }
}