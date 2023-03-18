using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewScenario", order = 3)]
public class Scenario : ScriptableObject
{

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
    public List<ScenarioUnit> scenarioUnits = new List<ScenarioUnit>();

    /// <summary>
    ///  scenarioUnits on jostain syyst� null kun koitetaan chessboardin startissa ladata se scenebuilderilla. Varmaan muissa sceneiss� samana homma
    ///  en tied� mit� sille pit�is tehd� mut nyt pelaan v�lill� Samin kanssa dota2gosta ja kirjotan eka yl�s ni ei unohdu.
    ///  .
    ///  Gl tulevaisuuden samuli, tsekkasin jo kaikenalista mutta en viel� ymm�rr� t�t� savesysteemi� kokonaisuudeessan niin hyvin..
    /// </summary>

    public void SaveScenario()
    {
        scenarioUnits.Clear();
        Unit[,] activeUnits = Chessboard.Instance.GetUnits();
        for (int x = 0; x < activeUnits.GetLength(0); x++)
        {
            // Add the units to the scenario file
            for (int y = 0; y < activeUnits.GetLength(1); y++)
            {
                if (activeUnits[x, y] != null)
                {
                    var obj = ScenarioBuilder.Instance.GetOriginalUnitType_From_InstantiatedUnitObject(activeUnits[x, y].gameObject);
                    var clone = new ScenarioUnit();
                    clone.Init(obj.name, x, y, activeUnits[x, y].team);
                    scenarioUnits.Add(clone);
                }
            }

            // Add the map-obstacles etc later!

        }
    }
}
