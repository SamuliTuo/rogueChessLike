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
    public Vector3 position;
    public Scenario battleScenario;
    public List<MapNode> connections;

    public void Init(MapNodeType _type, Vector3 _position, Scenario _scenario)
    {
        type = _type;
        position = _position;
        battleScenario = _scenario;
    }
}
