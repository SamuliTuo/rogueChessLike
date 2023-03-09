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
        //ClearMapNodes();
        //GenerateMapNodes();
        GeneratePaths();
        PositionMapNodes();
    }
        
    public void MapUpdate()
    {
        //implementing later. . .
    }

    //void GenerateMapNodes()
    //{
    //    startNode = CreateMapNode(startPos, MapNodeType.START_POS);
    //    endNode = CreateMapNode(endPos, MapNodeType.END_POS);

    //    for (int i = 0; i < Random.Range(nodeCountMinMax.x, nodeCountMinMax.y); i++)
    //    {
    //        tries = 0;
    //        Vector3 spawnPosition = GetRandomPosition();
    //        CreateMapNode(spawnPosition, MapNodeType.NONE);
    //    }
    //}

    MapNode CreateMapNode(MapNodeType type, int row, int index, bool splitting, bool mergingRight, Scenario scenario = null)
    {
        GameObject obj = Instantiate(mapNode, transform.position, Quaternion.identity, transform);
        var _node = obj.GetComponent<MapNode>();
        _node.Init(type, scenario, row, index, splitting, mergingRight);
        mapNodes.Add(_node);
        return _node;
    }

    //Vector3 GetRandomPosition()
    //{
    //    if (tries > 99)
    //        return Vector3.zero;

    //    tries++;
    //    Bounds bounds = transform.GetChild(0).GetComponent<Renderer>().bounds;
    //    Vector3 position = new (
    //        Random.Range(bounds.min.x + xMargin, bounds.max.x - xMargin),
    //        0,
    //        Random.Range(bounds.min.z + yMargin, bounds.max.z - yMargin)
    //    );
    //    if (IsTooClose(position))
    //    {
    //        return GetRandomPosition();
    //    }
    //    return position;
    //}

    //int tries = 0;
    //bool IsTooClose(Vector3 position)
    //{
    //    foreach (MapNode unit in mapNodes)
    //    {
    //        if (Vector3.Distance(position, unit.transform.position) < minDistance)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //void ClearMapNodes()
    //{
    //    foreach (var node in mapNodes)
    //    {
    //        Destroy(node);
    //    }
    //    mapNodes.Clear();
    //}



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

    [SerializeField] private Vector2 startSplitsRange = new Vector2(2, 5);
    [SerializeField] private int pathSteps = 5;
    [SerializeField] private float stepDistance_first = 4;
    [SerializeField] private float stepDistance = 2;
    [SerializeField] private float stepDistance_last = 4;
    [SerializeField] private float positionSideOffset = 2;
    [SerializeField] private float positionRandomFactor = 2f;
    [SerializeField] private float splitChance = 0.3f;
    [SerializeField] private float mergeChance = 0.3f;

    void GeneratePaths()
    {
        // First node
        startNode = CreateMapNode(MapNodeType.START_POS, 0, 0, false, false);

        List<MapNode> lastRow = new List<MapNode>();
        List<MapNode> nextRow = new List<MapNode>();
        int firstStepSplits = (int)Random.Range(startSplitsRange.x, startSplitsRange.y);

        // Second row of nodes
        for (int i = 0; i < firstStepSplits; i++)
        {
            bool splitting = (Random.Range(0.00f, 1.00f) < splitChance);
            bool mergingRight = (i != firstStepSplits - 1 && Random.Range(0.00f, 1.00f) < mergeChance);

            //var pos = currentStepPos - (Vector3.right * (firstStepSplits - 1) * positionSideOffset * 0.5f) + new Vector3(i * positionSideOffset, 0, 0);
            //pos += new Vector3(Random.Range(-1.00f, 1.00f) * positionRandomFactor, 0, Random.Range(-1.00f, 1.00f) * positionRandomFactor);
            var clone = CreateMapNode(MapNodeType.NONE, 1, i, splitting, mergingRight);
            startNode.AddConnection(clone);
            nextRow.Add(clone);
        }

        // Intermediate rows
        for (int pathStep = 0; pathStep < pathSteps; pathStep++)
        {
            lastRow.Clear();
            mapNodes.AddRange(nextRow);
            lastRow.AddRange(nextRow);
            nextRow.Clear();
            MapNode nextMergesWith = null;
            int rowIndex = 0;
            for (int i = 0; i < lastRow.Count; i++)
            {
                if (lastRow[i].splitting == true)
                {
                    if (nextMergesWith != null)
                    {
                        lastRow[i].AddConnection(nextMergesWith);
                        nextMergesWith = null;
                    }
                    else
                    {
                        bool split = Random.Range(0.00f, 1.00f) < splitChance;
                        var o = CreateMapNode(MapNodeType.NONE, pathStep + 2, rowIndex, split, false);
                        lastRow[i].AddConnection(o);
                        nextRow.Add(o);
                        rowIndex++;
                    }

                    bool splitting = Random.Range(0.00f, 1.00f) < splitChance;
                    bool mergingRight = (i != lastRow.Count - 1 && Random.Range(0.00f, 1.00f) < mergeChance);
                    var obj = CreateMapNode(MapNodeType.NONE, pathStep + 2, rowIndex, splitting, mergingRight);
                    lastRow[i].AddConnection(obj);
                    nextRow.Add(obj);
                    if (mergingRight) 
                        nextMergesWith = obj;

                    rowIndex++;
                }
                else
                {
                    if (nextMergesWith != null)
                    {
                        lastRow[i].AddConnection(nextMergesWith);
                        nextMergesWith = null;
                    }
                    else
                    {
                        bool splitting = Random.Range(0.00f, 1.00f) < splitChance;
                        bool mergingRight = (i != lastRow.Count - 1 && Random.Range(0.00f, 1.00f) < mergeChance);
                        var obj = CreateMapNode(MapNodeType.NONE, pathStep + 2, rowIndex, splitting, mergingRight);
                        lastRow[i].AddConnection(obj);
                        nextRow.Add(obj);
                        if (mergingRight)
                            nextMergesWith = obj;

                        rowIndex++;
                    }
                }
            }


            //var pos = currentStepPos - (Vector3.right * (firstStepSplits - 1) * positionSideOffset * 0.5f) + new Vector3(i * positionSideOffset, 0, 0);
            //pos += new Vector3(Random.Range(-1.00f, 1.00f) * positionRandomFactor, 0, Random.Range(-1.00f, 1.00f) * positionRandomFactor);
            //var clone = CreateMapNode(MapNodeType.NONE, );
            //lastRow[i].AddConnection(clone);
            //nextRow.Add(clone);
        }

        // Last node
        endNode = CreateMapNode(MapNodeType.END_POS, pathSteps + 2, 0, false, false);
        foreach (var node in lastRow)
            node.AddConnection(endNode);
    }

    void PositionMapNodes()
    {
        startNode.transform.position = startPos;

        Vector3 currentStepPos = startPos + Vector3.forward * stepDistance_first;
        int row = 1;
        
        while (mapNodes.Count > 0)
        {
            List<MapNode> rowNodes = new List<MapNode>();
            foreach (var node in mapNodes)
            {
                if (node.row == row)
                {
                    rowNodes.Add(node);
                }
            }
            foreach (var rowNode in rowNodes)
            {
                mapNodes.Remove(rowNode);
            }

            for (int i = 0; i < rowNodes.Count; i++)
            {
                var pos = currentStepPos - (Vector3.right * (rowNodes.Count - 1) * positionSideOffset * 0.5f) + new Vector3(i * positionSideOffset, 0, 0);
                pos += new Vector3(Random.Range(-1.00f, 1.00f) * positionRandomFactor, 0, Random.Range(-1.00f, 1.00f) * positionRandomFactor);

                rowNodes[i].transform.position = pos;
            }

            currentStepPos += Vector3.forward * stepDistance;
            row++;
        }

        endNode.transform.position = currentStepPos + Vector3.forward * stepDistance_last;


    }

    //void GeneratePaths()
    //{
    //    mapNodes = mapNodes.OrderBy(p => p.position.z).ToList();

    //    TryCreatePath();
    //}
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