using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

public class MapNode : MonoBehaviour
{
    public MapNodeType type;
    public Scenario scenario;
    public List<MapNode> nextNodeConnections;
    public int row;
    public int index;
    public bool splitting;
    public bool mergingRight;

    public void Init(MapNodeType _type, int _row, int _index, bool _splitting, bool _mergingRight)
    {
        type = _type;
        row = _row;
        index = _index;
        splitting = _splitting;
        mergingRight = _mergingRight;
    }

    public void AddConnection(MapNode connection)
    {
        nextNodeConnections.Add(connection);
    }

    public void SetupMapNode(Scenario _scenario, MapNodeType _type = MapNodeType.NONE) //rewards?? etc..
    {
        if (_type != MapNodeType.NONE)
            type = _type;

        scenario = _scenario;
    }
}
