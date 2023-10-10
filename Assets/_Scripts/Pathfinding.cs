using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[Serializable]
public enum UnitSearchType
{
    ENEMIES_ONLY,
    ALL_UNITS,
    ALLIES_ONLY,
    ALLIES_AND_SELF,
    ONLY_SELF,
    LOWEST_HP_ALLY_PERC,
    LOWEST_HP_ALLY_ABS,
}

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    Chessboard board;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        board = GetComponent<Chessboard>();
    }
    /*
    public void StartFindPath(
        Vector2Int startPos, 
        Vector2Int targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }
    public void StartFindClosestNodeOfType(
        Vector2Int startPos,
        NodeType targetType)
    {
        StartCoroutine(FindClosestNodeOfType(startPos, targetType));
    }*/
    public void StartFindUnit(
        Vector2Int startPos,
        Unit askingUnit,
        UnitSearchType searchType,
        int reach,
        Unit targetUnit)
    {
        StartCoroutine(FindUnit(startPos, askingUnit, searchType, reach, targetUnit));
    }

    public void StartFindClosestUnitOfType(
        Vector2Int startPos,
        Unit askingUnit,
        UnitSearchType searchType,
        int reach)
    {
        StartCoroutine(FindClosestUnitOfType(startPos, askingUnit, searchType, reach));
    }
    /*
    public IEnumerator FindPath(
        Vector2Int startPos, 
        Vector2Int targetPos) 
    {
        Vector2Int[] waypoints = new Vector2Int[0];
        bool pathSuccess = false;

        Node startNode = board.nodes[startPos.x, startPos.y];
        Node targetNode = board.nodes[targetPos.x, targetPos.y];

        Heap<Node> openSet = new Heap<Node>(board.nodes.Length);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                pathSuccess = true;
                break;
            }

            foreach (Node neighbour in board.GetNeighbourNodes(currentNode))
            {
                if (!neighbour.walkable
                    || closedSet.Contains(neighbour)
                    || (board.GetUnits()[neighbour.x, neighbour.y] != null 
                        && board.nodes[neighbour.x, neighbour.y] != board.nodes[targetPos.x, targetPos.y]))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess, false); 
    }
    */

    public IEnumerator FindUnit(Vector2Int startPos, Unit askingUnit, UnitSearchType searchType, int reach, Unit targetUnit)
    {
        Vector2Int[] waypoints = new Vector2Int[0];
        bool pathSuccess = false;
        bool inAttackRange = false;

        Node startNode = board.nodes[startPos.x, startPos.y];
        Node targetNode = null;
        Heap<Node> openSet = new Heap<Node>(board.nodes.Length);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var units = board.GetUnits();
            Node currNode = openSet.RemoveFirst();
            closedSet.Add(currNode);

            inAttackRange = GetDistance(board.nodes[currNode.x, currNode.y], board.nodes[targetUnit.x, targetUnit.y]) <= Extensions.ReachToRange(reach);
            if (inAttackRange && units[currNode.x, currNode.y] != null)
            {
                pathSuccess = true;
                targetNode = board.nodes[currNode.x, currNode.y];
                break;
            }
            /*
            if (units[currNode.x, currNode.y] != null)
            {
                if (units[currNode.x, currNode.y] == targetUnit)
                {
                    inAttackRange = GetDistance(board.nodes[askingUnit.x, askingUnit.y], board.nodes[currNode.x, currNode.y]) <= Extensions.ReachToRange(reach);
                    pathSuccess = true;
                    targetNode = board.nodes[currNode.x, currNode.y];
                    targetUnit = units[currNode.x, currNode.y];
                    break;
                }
            }*/


            foreach (Node neighbour in board.GetNeighbourNodes(currNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                if (units[neighbour.x, neighbour.y] != null)
                {
                    if (units[neighbour.x, neighbour.y] != targetUnit)
                    {
                        continue;
                    }
                }
                int newMovementCostToNeighbour = currNode.gCost + Mathf.RoundToInt(GetDistance(currNode, neighbour) + AddTerrainEffects(neighbour));
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.parent = currNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }
        yield return null;
        if (pathSuccess && targetNode != null)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess, inAttackRange, targetUnit);
    }


    public IEnumerator FindClosestUnitOfType(Vector2Int startPos, Unit askingUnit, UnitSearchType searchType, int reach)
    {
        Vector2Int[] waypoints = new Vector2Int[0];
        bool pathSuccess = false;
        bool inAttackRange = false;

        Node startNode = board.nodes[startPos.x, startPos.y];
        Node targetNode = null;
        Unit targetUnit = null;
        Heap<Node> openSet = new Heap<Node>(board.nodes.Length);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currNode = openSet.RemoveFirst();
            closedSet.Add(currNode);
            Unit[,] units = board.GetUnits();

            Unit target = IsAnUnitInRangeFromNode(currNode, reach, Mathf.Abs(askingUnit.team - 1), units);
            if (target != null && GameManager.Instance.IsValidTarget(askingUnit, target, searchType)) //units[currNode.x, currNode.y] != null)
            {
                inAttackRange = Chessboard.Instance.nodes[startPos.x, startPos.y] == currNode ? true : false;
                pathSuccess = true;
                targetNode = currNode;// board.nodes[currNode.x, currNode.y];
                targetUnit = target;
                break;
            }
            /*
            if (GameManager.Instance.IsValidTarget(askingUnit, units[currNode.x, currNode.y], searchType))
            {
                inAttackRange = GetDistance(board.nodes[askingUnit.x, askingUnit.y], board.nodes[currNode.x, currNode.y]) <= Extensions.ReachToRange(reach);
                pathSuccess = true;
                targetNode = board.nodes[currNode.x, currNode.y];
                targetUnit = units[currNode.x, currNode.y];
                break;
            }*/
            

            foreach (Node neighbour in board.GetNeighbourNodes(currNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                if (units[neighbour.x, neighbour.y] != null)
                {
                    if (!GameManager.Instance.IsValidTarget(askingUnit, units[neighbour.x, neighbour.y], searchType))
                    {
                        continue;
                    }
                }

                int newMovementCostToNeighbour = currNode.gCost + (Mathf.RoundToInt(GetDistance(currNode, neighbour) + AddTerrainEffects(neighbour)));

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.parent = currNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }

        yield return null;
        if (pathSuccess && targetNode != null)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess, inAttackRange, targetUnit);
    }

    Vector2Int[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector2Int[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    Vector2Int[] SimplifyPath(List<Node> path)
    {
        List<Vector2Int> waypoints = new List<Vector2Int>();
        //Vector2 directionOld = Vector2.zero;

        for (int i = 0; i < path.Count; i++)
        {
            //Vector2 directionNew = new(path[i-1].x - path[i].x, path[i-1].y - path[i].y);
            //if (directionNew != directionOld)
            //{
            waypoints.Add(new(path[i].x, path[i].y));
            //}
            //directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.x - nodeB.x);
        int distY = Mathf.Abs(nodeA.y - nodeB.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }


    float swampTax = 0.75f;
    public float AddTerrainEffects(Node node)
    {
        switch (node.tileTypeLayerName)
        {
            case "Swamp":
                return swampTax;
            default:
                return 0;
        }
    }

    public Unit IsAnUnitInRangeFromNode(Node node, int reach, int targetTeam, Unit[,] units)
    {
        Unit r = null;
        int closestRange = 1000000;
        for (int x = 0; x < units.GetLength(0); x++)
        {
            for (int y = 0; y < units.GetLength(1); y++)
            {
                if (units[x,y] == null || units[x,y].team != targetTeam)
                    continue;

                var dist = GetDistance(node, Chessboard.Instance.nodes[x,y]);
                if (dist < closestRange && dist <= Extensions.ReachToRange(reach))
                {
                    closestRange = dist;
                    r = units[x,y];
                }
            }
        }
        return r;
    }
}
