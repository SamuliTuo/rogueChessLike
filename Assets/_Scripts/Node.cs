using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    NONE,
    EMPTY,
    SWAMP
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

    public Node(bool walkable, int x, int y, string type)// string tileTypeLayerName)
    {
        this.walkable = walkable;
        this.x = x;
        this.y = y;
        tileTypeLayerName = type;
        //if (type == NodeType.NONE) { tileTypeLayerName = "Tile"; }
        //else if (type == NodeType.SWAMP) { tileTypeLayerName = "Swamp"; }
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
