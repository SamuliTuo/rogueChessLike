using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{
    public int pathLength = 10;
    public float minDistance;
    public Vector2 nodeCountMinMax = new Vector2(33, 40);
    public Vector3 startPos, endPos;
    public float xMargin, yMargin;

    private GameObject mapNode;
    private List<MapNode> mapNodes = new List<MapNode>();

    private MapNode startNode;
    private MapNode endNode;

    void Start()
    {
        mapNode = Resources.Load<GameObject>("map/mapNode");
        ClearMapNodes();
        GenerateMapNodes();
        ConnectNodes();
    }

    public void MapUpdate()
    {
        //implementing later. . .
    }

    void GenerateMapNodes()
    {
        startNode = CreateMapNode(startPos, MapNodeType.START_POS);
        endNode = CreateMapNode(endPos, MapNodeType.END_POS);

        for (int i = 0; i < Random.Range(nodeCountMinMax.x, nodeCountMinMax.y); i++)
        {
            tries = 0;
            Vector3 spawnPosition = GetRandomPosition();
            CreateMapNode(spawnPosition, MapNodeType.NONE);
            GameObject unit = Instantiate(mapNode, spawnPosition, Quaternion.identity);
        }
    }

    MapNode CreateMapNode(Vector3 pos, MapNodeType type, Scenario scenario = null)
    {
        GameObject obj = Instantiate(mapNode, pos, Quaternion.identity, transform);
        var _node = obj.GetComponent<MapNode>();
        _node.Init(type, pos, scenario);
        mapNodes.Add(_node);
        return _node;
    }

    Vector3 GetRandomPosition()
    {
        if (tries > 99)
            return Vector3.zero;

        tries++;
        Bounds bounds = transform.GetChild(0).GetComponent<Renderer>().bounds;
        Vector3 position = new (
            Random.Range(bounds.min.x + xMargin, bounds.max.x - xMargin),
            0,
            Random.Range(bounds.min.z + yMargin, bounds.max.z - yMargin)
        );
        if (IsTooClose(position))
        {
            return GetRandomPosition();
        }
        return position;
    }

    int tries = 0;
    bool IsTooClose(Vector3 position)
    {
        foreach (MapNode unit in mapNodes)
        {
            if (Vector3.Distance(position, unit.transform.position) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    void ClearMapNodes()
    {
        foreach (var node in mapNodes)
        {
            Destroy(node);
        }
        mapNodes.Clear();
    }



    //private void ConnectNodes()
    //{
    //    // connect nodes to form paths
    //    for (int i = 0; i < mapNodes.Count; i++)
    //    {
    //        for (int j = i + 1; j < mapNodes.Count; j++)
    //        {
    //            float distance = Vector3.Distance(mapNodes[i].position, mapNodes[j].position);
    //            if (distance < overlap)
    //            {
    //                nodes[i].connections.Add(nodes[j]);
    //                nodes[j].connections.Add(nodes[i]);
    //            }
    //        }
    //    }
    //}

    void GeneratePaths()
    {
        mapNodes = mapNodes.OrderBy(p => p.position.z).ToList();

        TryCreatePath();
    }
    
    void TryCreatePath()
    {
        List<MapNode> path = new List<MapNode>();
        path.Add(mapNodes[0]);
        int currentNode = 0;
        for (int i = 0; i < pathLength; i++)
        {
            var nextNode = currentNode + Random.Range(1, 4);
            if (mapNodes[nextNode] == null)
            {
                if (mapNodes[currentNode + 1] == null)
                {
                    break;
                }
            }
        }
    }


    //private void ConnectNodes()
    //{
    //    List<List<MapNode>> paths = new List<List<MapNode>>();
    //    // reorder along Z
    //    mapNodes = mapNodes.OrderBy(p => p.position.z).ToList();
    //    startNode.connections.Add(mapNodes[1]);
    //    startNode.connections.Add(mapNodes[2]);
    //    startNode.connections.Add(mapNodes[3]);

    //    // connect nodes to form paths using depth-first search
    //    Stack<MapNode> stack = new Stack<MapNode>();
    //    HashSet<MapNode> visited = new HashSet<MapNode>();
    //    stack.Push(startNode);

    //    while (stack.Count > 0)
    //    {
    //        MapNode node = stack.Pop();

    //        // mark node as visited
    //        visited.Add(node);

    //        // check if path is valid
    //        if (node != startNode && node != endNode && node.position.y <= node.connections[0].position.y)
    //        {
    //            node.connections.Remove(node.connections[0]);
    //            continue;
    //        }
    //        if (node.connections[0] != null)
    //        {
    //            if (node.position.y <= node.connections[0].position.y)
    //            {
    //                node.connections.Remove(node.connections[0]);
    //                continue;
    //            }
    //        }

            

    //        // add connections to stack
    //        foreach (MapNode connection in node.connections)
    //        {
    //            if (!visited.Contains(connection))
    //            {
    //                stack.Push(connection);

    //                // add path if it reaches end node and has correct number of steps
    //                if (connection == endNode && stack.Count == 11)
    //                {
    //                    List<MapNode> path = new List<MapNode>();
    //                    paths.Add(path);
    //                }
    //            }
    //        }
    //    }
    //}

}
