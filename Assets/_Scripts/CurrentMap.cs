using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentMap : MonoBehaviour
{
    public Map currentMap { get; private set; }
    public void SetCurrentMap(Map map)
    {
        currentMap = map;
    }
    public void ClearMap()
    {
        currentMap = null;
        ClearCurrentPath();
    }

    private List<MapNode> currentPath = new List<MapNode>();
    public MapNode GetCurrentNode()
    {
        return currentPath[currentPath.Count - 1];
    }
    public void ClearCurrentPath()
    {
        currentPath.Clear();
    }
    public void AddNextNodeOnPath(MapNode node)
    {
        currentPath.Add(node);
    }
}
