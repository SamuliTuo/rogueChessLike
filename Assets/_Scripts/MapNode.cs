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
    public Scenario battleScenario;
    public List<MapNode> nextNodeConnections;
    public int row;
    public int index;
    public bool splitting;
    public bool mergingRight;

    public void Init(MapNodeType _type, Scenario _scenario, int _row, int _index, bool _splitting, bool _mergingRight)
    {
        type = _type;
        battleScenario = _scenario;
        row = _row;
        index = _index;
        splitting = _splitting;
        mergingRight = _mergingRight;
    }

    public void AddConnection(MapNode connection)
    {
        nextNodeConnections.Add(connection);
    }
}
