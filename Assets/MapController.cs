using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public float minDistance;
    public Vector2 nodeCountMinMax = new Vector2(33, 40);
    public Vector3 startPos, endPos;
    public float xMargin, yMargin;

    private GameObject mapNode;
    private List<MapNode> mapNodes = new List<MapNode>();


    void Start()
    {
        mapNode = Resources.Load<GameObject>("map/mapNode");
        ClearMapNodes();
        GenerateMapNodes();
    }

    public void MapUpdate()
    {
        //implementing later. . .
    }

    void GenerateMapNodes()
    {
        CreateMapNode(startPos, MapNodeType.START_POS);
        CreateMapNode(endPos, MapNodeType.END_POS);

        for (int i = 0; i < Random.Range(nodeCountMinMax.x, nodeCountMinMax.y); i++)
        {
            tries = 0;
            Vector3 spawnPosition = GetRandomPosition();
            CreateMapNode(spawnPosition, MapNodeType.NONE);
            GameObject unit = Instantiate(mapNode, spawnPosition, Quaternion.identity);
        }
    }

    void CreateMapNode(Vector3 pos, MapNodeType type, Scenario scenario = null)
    {
        GameObject obj = Instantiate(mapNode, pos, Quaternion.identity, transform);
        var _node = obj.GetComponent<MapNode>();
        _node.Init(type, scenario);
        mapNodes.Add(_node);
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
}
