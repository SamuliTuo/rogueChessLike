using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapNodeType
{
    NONE,
    START_POS,
    END_POS,
    BATTLE,
}

public class MapNode : MonoBehaviour
{
    public MapNodeType type;

    // Having a scenario is optional
    public Scenario scenario;

    public void Init(MapNodeType type, Scenario scenario)
    {
        this.type = type;
        this.scenario = scenario;
    }
}
