using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

[System.Serializable]
public enum NodeType
{
    NONE,
    GRASS_PURPLE,
    HOLE,
    SWAMP,
    ROAD,
    THORNS,
    VINES,
    WALL,
    WATER
}

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;
    public string tileTypeLayerName;
    public int tileTypeVariation;
    public int rotation;

    public Node(bool walkable, int x, int y, string type, int tileTypeVariation, int rotation)// string tileTypeLayerName)
    {
        this.walkable = walkable;
        this.x = x;
        this.y = y;
        tileTypeLayerName = type;
        this.tileTypeVariation = tileTypeVariation;
        this.rotation = rotation;
    }

    public int fCost {
        get { 
            return gCost + hCost; 
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
