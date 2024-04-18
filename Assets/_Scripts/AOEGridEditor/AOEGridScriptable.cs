using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewAOEGrid", menuName = "ScriptableObjects/AOEGrid", order = 1)]
public class AOEGridScriptable : ScriptableObject
{
    public AOEShapes shapeName;

    public List<NodeInfo> rings_north_0;
    public List<NodeInfo> rings_north_1;
    public List<NodeInfo> rings_north_2;
    public List<NodeInfo> rings_north_3;
    public List<NodeInfo> rings_north_4;
    public List<NodeInfo> rings_north_5;
    public List<NodeInfo> rings_north_6;
    public List<NodeInfo> rings_north_7;
    [Space(10)]
    public List<NodeInfo> rings_northEast_0;
    public List<NodeInfo> rings_northEast_1;
    public List<NodeInfo> rings_northEast_2;
    public List<NodeInfo> rings_northEast_3;
    public List<NodeInfo> rings_northEast_4;
    public List<NodeInfo> rings_northEast_5;
    public List<NodeInfo> rings_northEast_6;
    public List<NodeInfo> rings_northEast_7;
    [Space(10)]
    public List<NodeInfo> rings_east_0;
    public List<NodeInfo> rings_east_1;
    public List<NodeInfo> rings_east_2;
    public List<NodeInfo> rings_east_3;
    public List<NodeInfo> rings_east_4;
    public List<NodeInfo> rings_east_5;
    public List<NodeInfo> rings_east_6;
    public List<NodeInfo> rings_east_7;
    [Space(10)]
    public List<NodeInfo> rings_southEast_0;
    public List<NodeInfo> rings_southEast_1;
    public List<NodeInfo> rings_southEast_2;
    public List<NodeInfo> rings_southEast_3;
    public List<NodeInfo> rings_southEast_4;
    public List<NodeInfo> rings_southEast_5;
    public List<NodeInfo> rings_southEast_6;
    public List<NodeInfo> rings_southEast_7;
    [Space(10)]
    public List<NodeInfo> rings_south_0;
    public List<NodeInfo> rings_south_1;
    public List<NodeInfo> rings_south_2;
    public List<NodeInfo> rings_south_3;
    public List<NodeInfo> rings_south_4;
    public List<NodeInfo> rings_south_5;
    public List<NodeInfo> rings_south_6;
    public List<NodeInfo> rings_south_7;
    [Space(10)]
    public List<NodeInfo> rings_southWest_0;
    public List<NodeInfo> rings_southWest_1;
    public List<NodeInfo> rings_southWest_2;
    public List<NodeInfo> rings_southWest_3;
    public List<NodeInfo> rings_southWest_4;
    public List<NodeInfo> rings_southWest_5;
    public List<NodeInfo> rings_southWest_6;
    public List<NodeInfo> rings_southWest_7;
    [Space(10)]
    public List<NodeInfo> rings_west_0;
    public List<NodeInfo> rings_west_1;
    public List<NodeInfo> rings_west_2;
    public List<NodeInfo> rings_west_3;
    public List<NodeInfo> rings_west_4;
    public List<NodeInfo> rings_west_5;
    public List<NodeInfo> rings_west_6;
    public List<NodeInfo> rings_west_7;
    [Space(10)]
    public List<NodeInfo> rings_northWest_0;
    public List<NodeInfo> rings_northWest_1;
    public List<NodeInfo> rings_northWest_2;
    public List<NodeInfo> rings_northWest_3;
    public List<NodeInfo> rings_northWest_4;
    public List<NodeInfo> rings_northWest_5;
    public List<NodeInfo> rings_northWest_6;
    public List<NodeInfo> rings_northWest_7;

    public void AddRings(CompassDir dir, List<List<NodeInfo>> rings)
    {
        Debug.Log(rings);
        switch (dir)
        {
            case CompassDir.NORTH: AddToLists(rings, rings_north_1, rings_north_2, rings_north_3, rings_north_4, rings_north_5, rings_north_6, rings_north_7, rings_north_0); break;
            case CompassDir.NORTH_EAST: AddToLists(rings, rings_northEast_1, rings_northEast_2, rings_northEast_3, rings_northEast_4, rings_northEast_5, rings_northEast_6, rings_northEast_7, rings_northEast_0); break;
            case CompassDir.EAST: AddToLists(rings, rings_east_1, rings_east_2, rings_east_3, rings_east_4, rings_east_5, rings_east_6, rings_east_7, rings_east_0); break;
            case CompassDir.SOUTH_EAST: AddToLists(rings, rings_southEast_1, rings_southEast_2, rings_southEast_3, rings_southEast_4, rings_southEast_5, rings_southEast_6, rings_southEast_7, rings_southEast_0); break;
            case CompassDir.SOUTH: AddToLists(rings, rings_south_1, rings_south_2, rings_south_3, rings_south_4, rings_south_5, rings_south_6, rings_south_7, rings_south_0); break;
            case CompassDir.SOUTH_WEST: AddToLists(rings, rings_southWest_1, rings_southWest_2, rings_southWest_3, rings_southWest_4, rings_southWest_5, rings_southWest_6, rings_southWest_7, rings_southWest_0); break;
            case CompassDir.WEST: AddToLists(rings, rings_west_1, rings_west_2, rings_west_3, rings_west_4, rings_west_5, rings_west_6, rings_west_7, rings_west_0); break;
            case CompassDir.NORTH_WEST: AddToLists(rings, rings_northWest_1, rings_northWest_2, rings_northWest_3, rings_northWest_4, rings_northWest_5, rings_northWest_6, rings_northWest_7, rings_northWest_0); break;
            default: break;
        }
    }
    void AddToLists(
        List<List<NodeInfo>> rings,
        List<NodeInfo> ring1, 
        List<NodeInfo> ring2,
        List<NodeInfo> ring3,
        List<NodeInfo> ring4,
        List<NodeInfo> ring5,
        List<NodeInfo> ring6,
        List<NodeInfo> ring7,
        List<NodeInfo> mid)
    {
        for (int i = 0; i < rings.Count; i++)
        {
            if (i == 7)
                ring7 = rings[i];
            else if (i == 6)
                ring6 = rings[i];
            else if (i == 5)
                ring5 = rings[i];
            else if (i == 4)
                ring4 = rings[i];
            else if (i == 3)
                ring3 = rings[i];
            else if (i == 2)
                ring2 = rings[i];
            else if (i == 1)
                ring1 = rings[i];
            else if (i == 0)
                mid = rings[i];
        }
    }



    [Serializable]
    public class NodeInfo
    {
        public int x;
        public int y;
        public bool push;
        public bool nudge;
        public int extraPushAmount;
        public string pushDirection;

        public NodeInfo(int x, int y, bool push, bool nudge, int extraPushAmount, string pushDirection)
        {
            this.x = x;
            this.y = y;
            this.push = push;
            this.nudge = nudge;
            this.extraPushAmount = extraPushAmount;
            this.pushDirection = pushDirection;
        }
    }
}