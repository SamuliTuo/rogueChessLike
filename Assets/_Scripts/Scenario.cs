using JetBrains.Annotations;
using System.Collections.Generic;
using System.Drawing.Printing;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewScenario", order = 3)]
public class Scenario : ScriptableObject
{
    public string saveName;
    public int sizeX;
    public int sizeY;
    public float cameraRotationX;
    public float cameraRotationY;
    public float cameraRotationZ;
    public List<ScenarioNode> scenarioNodes = new List<ScenarioNode>();
    public List<ScenarioUnit> scenarioUnits = new List<ScenarioUnit>();


    [System.Serializable]
    public class ScenarioNode
    {
        public int x;
        public int y;
        public int walkable;
        public string terrainLayer;
        public int tileVariation;
        public int rotation;
        public void Init(int x, int y, int walkable, string layerName, int tileVariation, int rotation)
        {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
            this.terrainLayer = layerName;
            this.tileVariation = tileVariation;
            this.rotation = rotation;
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
        public int spawnRot;
        public int team;
        public void Init(string unit, int spawnPosX, int spawnPosY, int spawnRot, int team)
        {
            this.unit = unit;
            this.spawnPosX = spawnPosX;
            this.spawnPosY = spawnPosY;
            this.spawnRot = spawnRot;
            this.team = team;
        }
    }

    public void SaveScenario(string saveName, Vector3 cameRot)
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

        // Add board and camera settings to the scenario file.
        sizeX = Chessboard.Instance.GetBoardSize().x;
        sizeY = Chessboard.Instance.GetBoardSize().y;
        cameraRotationX = cameRot.x;
        cameraRotationY = cameRot.y;
        cameraRotationZ = cameRot.z;

        // Add the units...
        Unit[,] activeUnits = Chessboard.Instance.GetUnits();
        for (int x = 0; x < activeUnits.GetLength(0); x++) {
            for (int y = 0; y < activeUnits.GetLength(1); y++) {
                if (activeUnits[x, y] != null && x < sizeX && y < sizeY) {
                    var obj = ScenarioBuilder.Instance.GetOriginalUnitType_From_InstantiatedUnitObject(activeUnits[x, y].gameObject);
                    var clone = new ScenarioUnit();
                    clone.Init(obj.name, x, y, activeUnits[x,y].spawnRotation, activeUnits[x, y].team);
                    scenarioUnits.Add(clone);
                }
            }
        }

        // Add the nodes...
        Node[,] nodes = Chessboard.Instance.nodes;
        for (int x = 0; x < nodes.GetLength(0); x++) {
            for (int y = 0; y < nodes.GetLength(1); y++) {
                if (nodes[x,y] != null && x < sizeX && y < sizeY) {
                    var clone = new ScenarioNode();
                    var walkable = nodes[x, y].walkable ? 1 : 0;
                    clone.Init(nodes[x,y].x, nodes[x,y].y, walkable, nodes[x,y].tileTypeLayerName, nodes[x,y].tileTypeVariation, nodes[x,y].rotation);
                    scenarioNodes.Add(clone);
                }
            }
        }

        EditorUtility.SetDirty(this);
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