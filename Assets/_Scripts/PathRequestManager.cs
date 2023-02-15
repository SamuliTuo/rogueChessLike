using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }
    /*
    public static void RequestPath(
        PathType type,
        Vector2Int pathStart,
        Vector2Int pathEnd,
        Action<Vector2Int[], bool, bool> callback)
    {
        PathRequest newRequest = new PathRequest(type, pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }
    public static void RequestFindClosestNode(
        PathType type,
        Vector2Int pathStart,
        NodeType targetType,
        Action<Vector2Int[], bool, bool> callback)
    {
        PathRequest newRequest = new PathRequest(type, pathStart, targetType, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }*/
    public static void RequestFindClosestEnemy(
        //PathType type,
        Vector2Int pathStart,
        Unit askingUnit,
        UnitSearchType searchType,
        int reach,
        Action<Vector2Int[], bool, bool, Unit> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, askingUnit, searchType, reach, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;

            //switch (currentPathRequest.pathType)
            //{
            //    case PathType.NORMAL:
            //        pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd); 
            //        break;
            //    case PathType.CLOSEST_NODE_OF_TYPE:
            //        pathfinding.StartFindClosestNodeOfType(currentPathRequest.pathStart, currentPathRequest.targetNodeType);
            //        break;
            //    case PathType.CLOSEST_ENEMY:
                    pathfinding.StartFindClosestUnitOfType(currentPathRequest.pathStart, currentPathRequest.askingUnit, currentPathRequest.searchType, currentPathRequest.reach);
            //        break;
            //    default: break;
            //}   
        }
    }

    public void FinishedProcessingPath(Vector2Int[] path, bool success, bool inAttackRange, Unit unit)
    {
        currentPathRequest.callback(path, success, inAttackRange, unit);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest {
        public Vector2Int pathStart;
        public Vector2Int pathEnd;
        public NodeType targetNodeType;
        public UnitSearchType searchType;
        public Unit askingUnit;
        public int reach;
        public Action<Vector2Int[], bool, bool, Unit> callback;

        /*
        public PathRequest(PathType _pathType, Vector2Int _start, Vector2Int _end, Action<Vector2Int[], bool, bool> _callback)
        {
            pathType = _pathType;
            pathStart = _start;
            pathEnd = _end;
            targetNodeType = NodeType.NONE;
            unit = null;
            callback = _callback;
        }

        public PathRequest(PathType _pathType, Vector2Int _start, NodeType _type, Action<Vector2Int[], bool, bool> _callback)
        {
            pathType = _pathType;
            pathStart = _start;
            pathEnd = -Vector2Int.one;
            targetNodeType = _type;
            unit = null;
            callback = _callback;
        }*/

        public PathRequest(Vector2Int _start, Unit _askingUnit, UnitSearchType _searchType, int _reach, Action<Vector2Int[], bool, bool, Unit> _callback)
        {
            pathStart = _start;
            searchType = _searchType;
            pathEnd = -Vector2Int.one;
            targetNodeType = NodeType.NONE;
            askingUnit = _askingUnit;
            callback = _callback;
            reach = _reach;
        }
    }
}
