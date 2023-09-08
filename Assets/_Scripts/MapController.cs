using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public MapSettings mapSettings;
    public Material lineMat;
    public Sprite mapImage_battle;
    public Sprite mapImage_battleElite;
    public Sprite mapImage_battleBoss;
    public Sprite mapImage_treasure;
    public Sprite mapImage_shop;
    public Sprite mapImage_mystery;
    public Sprite mapImage_caravan;

    public int pathLength = 10;
    public float minDistance;
    public Vector2 nodeCountMinMax = new Vector2(33, 40);
    public Vector3 startPos, endPos;
    public float xMargin, yMargin;
    public Transform lineTransforms;
    public GameObject playerPrefab;

    private MapNode currentPosition;
    private EncounterManager encounterManager;
    private GameObject mapNode;
    private List<MapNode> mapNodes = new List<MapNode>();
    private Camera cam;
    private MapNode startNode;
    private MapNode endNode;
    private GameObject player;
    private Transform currentMapTransform;
    private bool canMove = false;
    public void SetCanMove(bool canMove) {
        if (!canMove)
        {
            this.canMove = false;
        }
        else
        {
            StartCoroutine(CanMoveDelay());
        }
    }
    IEnumerator CanMoveDelay()
    {
        yield return new WaitForSeconds(0.25f);
        canMove = true;
    }

    void Start()
    {
        encounterManager = GameObject.Find("EncounterManager").GetComponent<EncounterManager>();
        currentMapTransform = GameManager.Instance.CurrentMap.transform;
        var map = GameManager.Instance.CurrentMap.currentMap;
        if (map != null)
        {
            mapNodes = map.mapNodes;
            StartCoroutine("PositionMapNodes");
            StartCoroutine(CanMoveDelay());
            return;
        }

        mapNode = Resources.Load<GameObject>("map/mapNode");
        GeneratePaths();
        StartCoroutine("PositionMapNodes");
        canMove = false;

        if (currentPosition = startNode)
        {
            StartCoroutine(DelayStartNodeActivation(2.5f));
        }
    }

    IEnumerator DelayStartNodeActivation(float time)
    {
        yield return new WaitForSeconds(time);
        encounterManager.ActivateNode(startNode);
    }


    private void Update()
    {
        print(canMove);
        if (currentPosition == null || !canMove)
            return;
        if (!cam)
        {
            cam = Camera.main;
            return;
        }

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("MapNode")))
        {
            if (Input.GetMouseButtonDown(0))
            {
                MoveOnMap(hit.collider.transform.parent.GetComponent<MapNode>());
            }
        }
        //print(currentPosition.name + ", pos: " + currentPosition.transform.position + ", nodeType: " + currentPosition.type);
    }

    public void MoveOnMap(MapNode node)
    {
        if (currentPosition.nextNodeConnections.Contains(node))
        {
            canMove = false;
            print("moved to: " + node.type);
            currentPosition = node;
            player.transform.position = currentPosition.transform.position;
            GameManager.Instance.pathTaken.Add(node);
            encounterManager.ActivateNode(node);
        }
    }





    //     __  __                                                          _     _              
    //    |  \/  |  __ _   _ __     __ _   ___   _ _    ___   _ _   __ _  | |_  (_)  ___   _ _  
    //    | |\/| | / _` | | '_ \   / _` | / -_) | ' \  / -_) | '_| / _` | |  _| | | / _ \ | ' \ 
    //    |_|  |_| \__,_| | .__/   \__, | \___| |_||_| \___| |_|   \__,_|  \__| |_| \___/ |_||_|
    //                    |_|      |___/                                  
    #region Map Generation
    void GeneratePaths()
    {
        // First node
        startNode = CreateMapNode(0, 0, false, false, mapSettings.startEncounter);

        List<MapNode> lastRow = new List<MapNode>();
        List<MapNode> nextRow = new List<MapNode>();
        int firstStepSplits = (int)Random.Range(mapSettings.randomRangeForSplitsAtFirstNode.x, mapSettings.randomRangeForSplitsAtFirstNode.y);

        // Second row of nodes
        for (int i = 0; i < firstStepSplits; i++)
        {
            bool splitting = (Random.Range(0.00f, 1.00f) < mapSettings.splitChance);
            bool mergingRight = (i != firstStepSplits - 1 && Random.Range(0.00f, 1.00f) < mapSettings.mergeChance);
            Encounter e = mapSettings.encountersByRow[0].encounters[Random.Range(0, mapSettings.encountersByRow[0].encounters.Count)];
            var clone = CreateMapNode(0, i, splitting, mergingRight, e);
            startNode.AddConnection(clone);
            nextRow.Add(clone);
        }

        // Middle rows
        for (int row = 1; row < mapSettings.encountersByRow.Count; row++)
        {
            lastRow.Clear();
            lastRow.AddRange(nextRow);
            nextRow.Clear();
            MapNode nextMergesWith = null;
            int rowIndex = 0;
            for (int i = 0; i < lastRow.Count; i++)
            {
                Encounter e = mapSettings.encountersByRow[row].encounters[Random.Range(0, mapSettings.encountersByRow[row].encounters.Count)];
                if ((lastRow[i].splitting == true && lastRow.Count < mapSettings.maximumNodesWideness) || lastRow.Count == 1)
                {
                    if (nextMergesWith != null)
                    {
                        lastRow[i].AddConnection(nextMergesWith);
                        nextMergesWith = null;
                    }
                    else
                    {
                        bool split = Random.Range(0.00f, 1.00f) < mapSettings.splitChance;
                        var o = CreateMapNode(row, rowIndex, split, false, e);
                        lastRow[i].AddConnection(o);
                        nextRow.Add(o);
                        rowIndex++;
                    }

                    bool splitting = (Random.Range(0.00f, 1.00f) < mapSettings.splitChance && lastRow.Count < mapSettings.maximumNodesWideness);
                    bool mergingRight = (i != lastRow.Count - 1 && Random.Range(0.00f, 1.00f) < mapSettings.mergeChance);
                    var obj = CreateMapNode(row, rowIndex, splitting, mergingRight, e);
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
                        bool splitting = Random.Range(0.00f, 1.00f) < mapSettings.splitChance;
                        bool mergingRight = (i != lastRow.Count - 1 && Random.Range(0.00f, 1.00f) < mapSettings.mergeChance);
                        var obj = CreateMapNode(row, rowIndex, splitting, mergingRight, e);
                        lastRow[i].AddConnection(obj);
                        nextRow.Add(obj);
                        if (mergingRight)
                            nextMergesWith = obj;

                        rowIndex++;
                    }
                }
            }
        }

        // Last node
        endNode = CreateMapNode(mapSettings.encountersByRow.Count, 0, false, false, mapSettings.lastEncounters[Random.Range(0, mapSettings.lastEncounters.Count)]);
        for (int i = 0; i < nextRow.Count; i++)
        {
            nextRow[i].AddConnection(endNode);
        }
    }

    MapNode CreateMapNode(int row, int index, bool splitting, bool mergingRight, Encounter _encounter)
    {
        GameObject obj = Instantiate(mapNode, transform.position, Quaternion.identity, currentMapTransform);
        var _node = obj.GetComponent<MapNode>();
        _node.Init(row, index, splitting, mergingRight, _encounter);
        mapNodes.Add(_node);
        return _node;
    }

    IEnumerator PositionMapNodes()
    {
        // Position nodes
        List<MapNode> unpositionedNodes = new List<MapNode>();
        unpositionedNodes.AddRange(mapNodes);
        Vector3 currentStepPos = startPos + Vector3.forward * mapSettings.stepDistance_first;
        int row = 0;

        for (int i = unpositionedNodes.Count - 1; i >= 0; i--)
        {
            if (unpositionedNodes[i].type == MapNodeType.START_POS)
            {
                startNode = unpositionedNodes[i];
                unpositionedNodes[i].transform.position = startPos;
                unpositionedNodes.RemoveAt(i);
            }
            else if (unpositionedNodes[i].type == MapNodeType.END_POS)
            {
                endNode = unpositionedNodes[i];
                unpositionedNodes.RemoveAt(i);
            }
        }

        while (unpositionedNodes.Count > 0)
        {
            List<MapNode> rowNodes = new List<MapNode>();
            foreach (var node in unpositionedNodes)
            {
                if (node.row == row)
                {
                    rowNodes.Add(node);
                }
            }
            foreach (var rowNode in rowNodes)
            {
                unpositionedNodes.Remove(rowNode);
            }
            for (int i = 0; i < rowNodes.Count; i++)
            {
                var pos = currentStepPos - (Vector3.right * (rowNodes.Count - 1) * mapSettings.positionSideOffset * 0.5f) + new Vector3(i * mapSettings.positionSideOffset, 0, 0);
                pos += new Vector3(Random.Range(-1.00f, 1.00f) * mapSettings.positionRandomFactor, 0, Random.Range(-1.00f, 1.00f) * mapSettings.positionRandomFactor);

                rowNodes[i].transform.position = pos;
                //rowNodes[i].encounter = mapSettings.encountersByRow[row].encounters[Random.Range(0, mapSettings.encountersByRow[row].encounters.Count)];
            }
            currentStepPos += Vector3.forward * mapSettings.stepDistance;

            row++;
            yield return null;
        }

        endNode.transform.position = 
            currentStepPos + Vector3.forward * mapSettings.stepDistance_last + 
            new Vector3(Random.Range(-1.00f, 1.00f) * mapSettings.positionRandomFactor, 0, Random.Range(-1.00f, 1.00f) * mapSettings.positionRandomFactor);

        SpawnPlayerUnit();

        DrawPaths();
        //canMove = true;

        if (GameManager.Instance.CurrentMap.currentMap == null)
        {
            GameManager.Instance.CurrentMap.ClearMap();
            GameManager.Instance.CurrentMap.SetCurrentMap(new Map(mapNodes));
            GameManager.Instance.CurrentMap.AddNextNodeOnPath(startNode);
        }
    }

    void SpawnPlayerUnit()
    {
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, transform);
        //print("player: "+player);
        if (GameManager.Instance.CurrentMap.currentMap != null)
        {
            currentPosition = GameManager.Instance.CurrentMap.GetCurrentNode();
            //print("curr map != null,   curr pos: " + currentPosition);
        }
        else
        {
            currentPosition = startNode;
            //print("curr map == null,   curr pos: " + currentPosition);
        }

        player.transform.position = currentPosition.transform.position;
    }
    
    void DrawPaths()
    {
        foreach (var node in mapNodes)
        {
            foreach (var connection in node.nextNodeConnections)
            {
                DrawLine(node.transform.position, connection.transform.position, Color.green, Mathf.Infinity);
            }
        }
    }

    public void GenerateNewMap()
    {
        for (int i = mapNodes.Count - 1; i >= 0; i--)
        {
            Destroy(mapNodes[i].gameObject);
        }
        for (int i = lineTransforms.childCount - 1; i >= 0; i--)
        {
            Destroy(lineTransforms.GetChild(i).gameObject);
        }
        Destroy(player);
        GameManager.Instance.CurrentMap.ClearMap();
        player = null;
        startNode = endNode = null;
        mapNodes.Clear();
        mapNode = Resources.Load<GameObject>("map/mapNode");
        GeneratePaths();
        StartCoroutine("PositionMapNodes");
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = lineMat;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
        myLine.transform.SetParent(lineTransforms);
    }
    #endregion
}

public class Map {
    public List<MapNode> mapNodes = new List<MapNode>();
    public Map(List<MapNode> mapNodes)
    {
        this.mapNodes = mapNodes;
    }
}