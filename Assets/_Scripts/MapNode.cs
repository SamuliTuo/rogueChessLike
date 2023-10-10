using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapNodeType
{
    NONE,
    START_POS,
    END_POS,
    BATTLE,
    BATTLE_ELITE,
    BATTLE_BOSS,
    TREASURE,
    SHOP,
    RANDOM_ENCOUNTER,
    CARAVAN,
}

public class MapNode : MonoBehaviour
{
    public MapNodeType type;
    public Encounter encounter;
    public List<MapNode> nextNodeConnections;
    public int row;
    public int index;
    public bool splitting;
    public bool mergingRight;

    public void Init(int _row, int _index, bool _splitting, bool _mergingRight, Encounter _encounter, MapNodeType type = MapNodeType.NONE)
    {
        encounter = _encounter;
        if (type == MapNodeType.NONE)
        {
            this.type = DetermineNodeType(_encounter);
        }
        else
        {
            this.type = type;
        }
        row = _row;
        index = _index;
        splitting = _splitting;
        mergingRight = _mergingRight;
        SetImage();
    }

    public MapNodeType DetermineNodeType(Encounter encounter)
    {
        if (encounter.possibleBattleScenarios.Count > 0 && encounter.possibleTextScenarios.Count > 0)
        {
            return MapNodeType.RANDOM_ENCOUNTER;
        }
        else if (encounter.possibleBattleScenarios.Count > 0 && encounter.possibleTextScenarios.Count == 0)
        {
            return MapNodeType.BATTLE;
        }
        else if (encounter.possibleBattleScenarios.Count == 0 && encounter.possibleTextScenarios.Count > 0)
        {
            return MapNodeType.RANDOM_ENCOUNTER;
        }
        return MapNodeType.START_POS;
    }

    public void AddConnection(MapNode connection)
    {
        nextNodeConnections.Add(connection);
    }

    //checks what type the encounter is from Encounter-argument of Init(), then sets the according in MapController-class to the image from to the child's renderer's material
    public void SetImage()
    {
        if (type == MapNodeType.BATTLE)
            transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameManager.Instance.MapController.mapImage_battle.texture;

        else if (type == MapNodeType.BATTLE_ELITE)
            transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameManager.Instance.MapController.mapImage_battleElite.texture;

        else if (type == MapNodeType.BATTLE_BOSS)
            transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameManager.Instance.MapController.mapImage_battleBoss.texture;

        else if (type == MapNodeType.TREASURE)
            transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameManager.Instance.MapController.mapImage_treasure.texture;

        else if (type == MapNodeType.SHOP)
            transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameManager.Instance.MapController.mapImage_shop.texture;

        else if (type == MapNodeType.CARAVAN)
            transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameManager.Instance.MapController.mapImage_caravan.texture;

        else if (type == MapNodeType.RANDOM_ENCOUNTER)
            transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameManager.Instance.MapController.mapImage_mystery.texture;
    }
}
