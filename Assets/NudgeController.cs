using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;

public enum NudgeDir { NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORT_WEST }

public class NudgeController : MonoBehaviour
{
    public float nudgeCooldown = 1;
    public bool canNudge = true;
    [SerializeField] private int nudgeLayer = 11;
    [SerializeField] private NudgeArms arms = null;

    private float maxNudgeMagnitude = 5f;

    private float t;
    private Camera currentCam;
    private LayerMask layerMask;
    private LayerMask nudgeLayerMask;
    private Chessboard board;
    private Vector2Int currentHover;
    private Vector3 currentDragPosition;
    private Unit currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private ArrowGenerator arrowGenerator;
    Vector3 draggingStartPos;
    List<Vector3> compass;
    private bool nudgeIsChip = false;
    NudgeUIController nudgeUIController;


    private void Awake()
    {
        arrowGenerator = GetComponentInChildren<ArrowGenerator>();
        nudgeUIController = GameObject.Find("Canvas").GetComponentInChildren<NudgeUIController>();
        currentCam = Camera.main;
    }

    private void Start()
    {
        t = nudgeCooldown;
        MakeCompass();

        board = GetComponent<Chessboard>();
        layerMask = GameManager.Instance.boardLayerMask;
        nudgeLayerMask |= (1 << nudgeLayer);
    }


    public void Nudge(Vector3 dir, Unit unit, float stunTime = 0.5f)
    {
        unit.SetScale(Vector3.one);
        unit.GetStunned(stunTime);
        Tuple<NudgeDir, Vector3> _nudgeDir = DetermineNudgeDir(dir);
        
        unit.GetNudged(nudgeIsChip, _nudgeDir.Item2); //GetDirectionFromNudgeDir());
        Vector2Int moveToNode;

        if (nudgeIsChip)
        { // chip  (throw unit over other units, objects and chasms)
            moveToNode = CheckNodesInNudgeDirection_chip(
                            new Vector2Int(unit.x, unit.y),
                            _nudgeDir,
                            (int)dir.magnitude);
        }
        else
        { // nudge  (push unit in a line)
            moveToNode = CheckNodesInNudgeDirectionWhenNudging(
                            new Vector2Int(unit.x, unit.y),
                            _nudgeDir,
                            (int)dir.magnitude);
        }

        if (moveToNode != -Vector2Int.one)
        {
            // tänne jatkamaan unittien stunnaamista nudgechip paikassa! tai tuolla ylemmäl mis tsekataan jo nudgeischip
            var units = board.GetUnits();
            var moves = unit.GetAvailableMoves(ref units, board.GetTilecount().x, board.GetTilecount().y);
            board.MoveTo(unit, moveToNode.x, moveToNode.y, ref moves, true);
        }
        board.RemoveAllHighlightTiles();
        t = 0;
    }

    public void NudgerUpdate()
    {
        if (t < nudgeCooldown)
        {
            t += Time.deltaTime;
            nudgeUIController?.UpdateCooldownUI(t / nudgeCooldown);
            return;
        }

        // START NUDGE - PULL:
        // See what we hover and try to start:
        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        var activeUnits = board.GetUnits();
        if (currentlyDragging == null && Physics.Raycast(ray, out hit, 100, layerMask))
        {
            // Get the indexes of the tiles I've hit
            Vector2Int hitPosition = board.LookupTileIndex(hit.transform.gameObject);
            if (hitPosition == -Vector2Int.one)
                return;

            // If hovering a tile after not hovering any tile
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If already were hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                var _unit = activeUnits[hitPosition.x, hitPosition.y];
                if (Input.GetMouseButtonDown(0) && _unit != null)
                {
                    if (_unit.team == 0)
                    {
                        currentlyDragging = _unit;
                        draggingStartPos = board.GetTileCenter(_unit.x, _unit.y);
                    }
                }

                board.tiles[currentHover.x, currentHover.y].layer =
                    (board.ContainsValidMove(ref availableMoves, currentHover)) ?
                        LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer(board.nodes[currentHover.x, currentHover.y].tileTypeLayerName);
                currentHover = hitPosition;
                board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                var _unit = activeUnits[hitPosition.x, hitPosition.y];
                if (_unit != null)
                {
                    if (_unit.team == 0)
                    {
                        nudgeIsChip = false;
                        currentlyDragging = _unit;
                        draggingStartPos = board.GetTileCenter(_unit.x, _unit.y);
                        arms.StartNudger(draggingStartPos);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                var _unit = activeUnits[hitPosition.x, hitPosition.y];
                if (_unit != null)
                {
                    if (_unit.team == 0)
                    {
                        nudgeIsChip = true;
                        currentlyDragging = _unit;
                        draggingStartPos = board.GetTileCenter(_unit.x, _unit.y);
                        arms.StartChipper(draggingStartPos);
                    }
                }
            }
        }


        // WHILE PULLING:
            // drag and highlight the aim-squares:

        // mouse 1 (nudge) :
        else if (!nudgeIsChip && currentlyDragging != null && Physics.Raycast(ray, out hit, 100, nudgeLayerMask))
        {
            Vector3 _pos = board.GetTileCenter(currentlyDragging.x, currentlyDragging.y);
            Vector3 direction = new Vector3(_pos.x, hit.point.y, _pos.z) - hit.point;
            float mag = direction.magnitude;
            arrowGenerator.UpdateArrow(new Vector3(_pos.x, transform.position.y, _pos.z), direction, Mathf.Clamp(mag * 1.3f, 0.5f, 8f));

            Tuple<NudgeDir, Vector3> _nudgeDir = DetermineNudgeDir(direction);
            Vector2Int moveToNode = CheckNodesInNudgeDirection(new Vector2Int(currentlyDragging.x, currentlyDragging.y), _nudgeDir, (int)mag);
            arms.UpdateNudgerAimPosition(Math.Clamp(mag, 0, maxNudgeMagnitude) / maxNudgeMagnitude, _pos, direction);
            board.RemoveAllHighlightTiles();

            if (moveToNode != -Vector2Int.one)
            {
                board.tiles[moveToNode.x, moveToNode.y].layer = LayerMask.NameToLayer("Highlight");
            }
            
            // If we are releasing the mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                arms.NudgerNudge();
                arrowGenerator.DeactivateArrow();
                Nudge(direction, currentlyDragging, 0.5f);
                currentlyDragging = null;
            }
        }

        // mouse 2 (chip) :
        else if (nudgeIsChip && currentlyDragging != null && Physics.Raycast(ray, out hit, 100, nudgeLayerMask))
        {
            Vector3 _pos = board.GetTileCenter(currentlyDragging.x, currentlyDragging.y);
            Vector3 direction = new Vector3(_pos.x, hit.point.y, _pos.z) - hit.point;
            float mag = direction.magnitude;
            arrowGenerator.UpdateArrow(new Vector3(_pos.x, transform.position.y, _pos.z), direction, mag);

            Tuple<NudgeDir, Vector3> _nudgeDir = DetermineNudgeDir(direction);
            Vector2Int moveToNode = CheckNodesInNudgeDirection_chip(new Vector2Int(currentlyDragging.x, currentlyDragging.y), _nudgeDir, (int)mag);

            arms.UpdateChipperAimPosition(Mathf.Clamp(mag, 0, maxNudgeMagnitude) / maxNudgeMagnitude, _pos, direction);
            board.RemoveAllHighlightTiles();
            if (moveToNode != -Vector2Int.one)
            {
                board.tiles[moveToNode.x, moveToNode.y].layer = LayerMask.NameToLayer("Highlight");
            }

            // If we are releasing the mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(1))
            {
                arms.ChipperChip();
                arrowGenerator.DeactivateArrow();
                Nudge(direction, currentlyDragging, 0.5f);
                currentlyDragging = null;
            }
        }

        // Mouse has been released :
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                board.tiles[currentHover.x, currentHover.y].layer =
                    (board.ContainsValidMove(ref availableMoves, currentHover)) ?
                        LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer(board.nodes[currentHover.x, currentHover.y].tileTypeLayerName);
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(board.GetTileCenter(currentlyDragging.x, currentlyDragging.y));
                currentlyDragging = null;
                board.RemoveHighlightTiles();
            }
        }
    }


    void MakeCompass()
    {
        compass = new List<Vector3>
        {
            Vector3.forward,
            new Vector3(0.7171068f, 0, 0.7171068f),
            Vector3.right,
            new Vector3(0.7171068f, 0, -0.7171068f),
            Vector3.back,
            new Vector3(-0.7171068f, 0, -0.7171068f),
            Vector3.left,
            new Vector3(-0.7171068f, 0, 0.7171068f)
        };
    }

    Tuple<NudgeDir,Vector3> DetermineNudgeDir(Vector3 dir)
    {
        var dirNormalized = dir.normalized;
        float xDot = Vector3.Dot(Vector3.right, dirNormalized);
        float yDot = Vector3.Dot(Vector3.forward, dirNormalized);

        // Get the 3 directions of the quadrant the input is in
        Dictionary<NudgeDir, Vector3> dirs = new Dictionary<NudgeDir, Vector3>();
        if (xDot > 0)
        {
            if (yDot > 0) 
            {
                dirs.Add(NudgeDir.NORTH, Vector3.forward);
                dirs.Add(NudgeDir.NORTH_EAST, new Vector3(0.7171068f, 0, 0.7171068f));
                dirs.Add(NudgeDir.EAST, Vector3.right);
            }
            else
            {
                dirs.Add(NudgeDir.EAST, Vector3.right);
                dirs.Add(NudgeDir.SOUTH_EAST, new Vector3(0.7171068f, 0, -0.7171068f));
                dirs.Add(NudgeDir.SOUTH, Vector3.back);
            }
        }
        else
        {
            if (yDot <= 0)
            {
                dirs.Add(NudgeDir.SOUTH, Vector3.back);
                dirs.Add(NudgeDir.SOUTH_WEST, new Vector3(-0.7171068f, 0, -0.7171068f));
                dirs.Add(NudgeDir.WEST, Vector3.left);
            }
            else
            {
                dirs.Add(NudgeDir.WEST, Vector3.left);
                dirs.Add(NudgeDir.NORT_WEST, new Vector3(-0.7171068f, 0, 0.7171068f));
                dirs.Add(NudgeDir.NORTH, Vector3.forward);
            }
        }

        // Check which direction in the quadrant is closest
        KeyValuePair<NudgeDir, Vector3> finalDir = new KeyValuePair<NudgeDir, Vector3>();
        float distance = 1000000;
        foreach (KeyValuePair<NudgeDir,Vector3> compassDirection in dirs)
        {
            float dist2 = (dirNormalized - compassDirection.Value).magnitude;
            if (dist2 < distance)
            {
                distance = dist2;
                finalDir = compassDirection;
            }
        }
        return new Tuple<NudgeDir, Vector3>(finalDir.Key, finalDir.Value);
    }

    Vector2Int CheckNodesInNudgeDirection(Vector2Int currPos, Tuple<NudgeDir, Vector3> _nudgeDir, int nudgeDistanceSquares)
    {
        if (nudgeDistanceSquares <= 0) 
            return -Vector2Int.one;

        nudgeDistanceSquares = Mathf.Clamp(nudgeDistanceSquares, 1, 5);
        Node[,] nodes = board.nodes;
        Unit[,] units = board.GetUnits();
        Vector2Int r = -Vector2Int.one;
        int maxX = board.GetBoardSize().x - 1;
        int maxY = board.GetBoardSize().y - 1;

        //print("curr pos: " + currPos + ",   distance: " + nudgeDistanceSquares + ",   nudge dir: " + _nudgeDir.Item1 + ",   nudge dir: " + _nudgeDir.Item2); ;
        switch (_nudgeDir.Item1)
        {
            case NudgeDir.NORTH:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.y + i > maxY)
                        break;

                    if (nodes[currPos.x, currPos.y + i] != null)
                    {
                        if (units[currPos.x, currPos.y + i] == null && nodes[currPos.x, currPos.y + i].walkable == true)
                            r = new Vector2Int(currPos.x, currPos.y + i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.NORTH_EAST:
                for (float i = 1; i < nudgeDistanceSquares * 0.7072f; i += 0.7072f)
                {
                    i = Mathf.RoundToInt(i);
                    if (currPos.x + i > maxX || currPos.y + i > maxY)
                        break;

                    if (nodes[currPos.x + (int)i, currPos.y + (int)i] != null)
                    {
                        if (units[currPos.x + (int)i, currPos.y + (int)i] == null && nodes[currPos.x + (int)i, currPos.y + (int)i].walkable == true)
                            r = new Vector2Int(currPos.x + (int)i, currPos.y + (int)i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.EAST:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.x + i > maxX)
                        break;

                    if (nodes[currPos.x + i, currPos.y] != null)
                    {
                        if (units[currPos.x + i, currPos.y] == null && nodes[currPos.x + i, currPos.y].walkable == true)
                            r = new Vector2Int(currPos.x + i, currPos.y);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.SOUTH_EAST:
                for (float i = 1; i < nudgeDistanceSquares * 0.7072f; i += 0.7072f)
                {
                    i = Mathf.RoundToInt(i);
                    if (currPos.x + i > maxX || currPos.y - i < 0)
                        break;

                    if (nodes[currPos.x + (int)i, currPos.y - (int)i] != null)
                    {
                        if (units[currPos.x + (int)i, currPos.y - (int)i] == null && nodes[currPos.x + (int)i, currPos.y - (int)i].walkable == true)
                            r = new Vector2Int(currPos.x + (int)i, currPos.y - (int)i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.SOUTH:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.y - i < 0)
                        break;

                    if (nodes[currPos.x, currPos.y - i] != null)
                    {
                        if (units[currPos.x, currPos.y - i] == null && nodes[currPos.x, currPos.y - i].walkable == true)
                            r = new Vector2Int(currPos.x, currPos.y - i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.SOUTH_WEST:
                for (float i = 1; i < nudgeDistanceSquares * 0.7072f; i += 0.7072f)
                {
                    i = Mathf.RoundToInt(i);
                    if (currPos.x - i < 0 || currPos.y - i < 0)
                        break;

                    if (nodes[currPos.x - (int)i, currPos.y - (int)i] != null)
                    {
                        if (units[currPos.x - (int)i, currPos.y - (int)i] == null && nodes[currPos.x - (int)i, currPos.y - (int)i].walkable == true)
                            r = new Vector2Int(currPos.x - (int)i, currPos.y - (int)i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.WEST:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.x - i < 0)
                        break;

                    if (nodes[currPos.x - i, currPos.y] != null)
                    {
                        if (units[currPos.x - i, currPos.y] == null && nodes[currPos.x - i, currPos.y].walkable == true)
                            r = new Vector2Int(currPos.x - i, currPos.y);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.NORT_WEST:
                for (float i = 1; i < nudgeDistanceSquares * 0.7072f; i += 0.7072f)
                {
                    i = Mathf.RoundToInt(i);
                    if (currPos.x - i < 0 || currPos.y + i > maxY)
                        break;

                    if (nodes[currPos.x - (int)i, currPos.y + (int)i] != null)
                    {
                        if (units[currPos.x - (int)i, currPos.y + (int)i] == null && nodes[currPos.x - (int)i, currPos.y + (int)i].walkable == true)
                            r = new Vector2Int(currPos.x - (int)i, currPos.y + (int)i);
                        else
                            break;
                    }
                }
                break;
        }

        return r;
    }

    // beautiful copy pasta for ages!
    Vector2Int CheckNodesInNudgeDirectionWhenNudging(Vector2Int currPos, Tuple<NudgeDir, Vector3> _nudgeDir, int nudgeDistanceSquares)
    {
        if (nudgeDistanceSquares <= 0)
            return -Vector2Int.one;

        nudgeDistanceSquares = Mathf.Clamp(nudgeDistanceSquares, 1, 4);
        Node[,] nodes = board.nodes;
        Unit[,] units = board.GetUnits();
        Vector2Int r = -Vector2Int.one;
        int maxX = board.GetBoardSize().x - 1;
        int maxY = board.GetBoardSize().y - 1;

        switch (_nudgeDir.Item1)
        {
            case NudgeDir.NORTH:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.y + i > maxY)
                        break;

                    if (nodes[currPos.x, currPos.y + i] != null)
                    {
                        if (CheckForUnitCollision(currPos.x, currPos.y + i, i, nudgeDistanceSquares, units, _nudgeDir.Item2) && nodes[currPos.x, currPos.y + i].walkable == true)
                            r = new Vector2Int(currPos.x, currPos.y + i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.NORTH_EAST:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.x + i > maxX || currPos.y + i > maxY)
                        break;

                    if (nodes[currPos.x + i, currPos.y + i] != null)
                    {
                        if (CheckForUnitCollision(currPos.x + i, currPos.y + i, i, nudgeDistanceSquares, units, _nudgeDir.Item2) && nodes[currPos.x + i, currPos.y + i].walkable == true)
                            r = new Vector2Int(currPos.x + i, currPos.y + i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.EAST:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.x + i > maxX)
                        break;

                    if (nodes[currPos.x + i, currPos.y] != null)
                    {
                        if (CheckForUnitCollision(currPos.x + i, currPos.y, i, nudgeDistanceSquares, units, _nudgeDir.Item2) && nodes[currPos.x + i, currPos.y].walkable == true)
                            r = new Vector2Int(currPos.x + i, currPos.y);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.SOUTH_EAST:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.x + i > maxX || currPos.y - i < 0)
                        break;

                    if (nodes[currPos.x + i, currPos.y - i] != null)
                    {
                        if (CheckForUnitCollision(currPos.x + i, currPos.y - i, i, nudgeDistanceSquares, units, _nudgeDir.Item2) && nodes[currPos.x + i, currPos.y - i].walkable == true)
                            r = new Vector2Int(currPos.x + i, currPos.y - i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.SOUTH:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.y - i < 0)
                        break;

                    if (nodes[currPos.x, currPos.y - i] != null)
                    {
                        if (CheckForUnitCollision(currPos.x, currPos.y - i, i, nudgeDistanceSquares, units, _nudgeDir.Item2) && nodes[currPos.x, currPos.y - i].walkable == true)
                            r = new Vector2Int(currPos.x, currPos.y - i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.SOUTH_WEST:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.x - i < 0 || currPos.y - i < 0)
                        break;

                    if (nodes[currPos.x - i, currPos.y - i] != null)
                    {
                        if (CheckForUnitCollision(currPos.x - i, currPos.y - i, i, nudgeDistanceSquares, units, _nudgeDir.Item2) && nodes[currPos.x - i, currPos.y - i].walkable == true)
                            r = new Vector2Int(currPos.x - i, currPos.y - i);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.WEST:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.x - i < 0)
                        break;

                    if (nodes[currPos.x - i, currPos.y] != null)
                    {
                        if (CheckForUnitCollision(currPos.x - i, currPos.y, i, nudgeDistanceSquares, units, _nudgeDir.Item2) && nodes[currPos.x - i, currPos.y].walkable == true)
                            r = new Vector2Int(currPos.x - i, currPos.y);
                        else
                            break;
                    }
                }
                break;
            case NudgeDir.NORT_WEST:
                for (int i = 1; i < nudgeDistanceSquares; i++)
                {
                    if (currPos.x - i < 0 || currPos.y + i > maxY)
                        break;

                    if (nodes[currPos.x - i, currPos.y + i] != null)
                    {
                        if (CheckForUnitCollision(currPos.x-i, currPos.y+i, i, nudgeDistanceSquares, units, _nudgeDir.Item2) && nodes[currPos.x - i, currPos.y + i].walkable == true)
                            r = new Vector2Int(currPos.x - i, currPos.y + i);
                        else
                            break;
                    }
                }
                break;
        }

        return r;
    }
    bool CheckForUnitCollision(int x, int y, int i, int nudgeDistance, Unit[,] units, Vector3 dir)
    {
        if (units[x,y] != null)
        {
            Nudge(dir.normalized * (nudgeDistance - i), units[x,y], 1.5f);
            return false;
        }
        return true;
    }

    Vector2Int CheckNodesInNudgeDirection_chip(Vector2Int currPos, Tuple<NudgeDir, Vector3> _nudgeDir, int nudgeDistanceSquares)
    {
        if (nudgeDistanceSquares <= 0)
            return -Vector2Int.one;

        nudgeDistanceSquares = Mathf.Clamp(nudgeDistanceSquares, 1, 4);
        Node[,] nodes = board.nodes;
        Unit[,] units = board.GetUnits();
        Vector2Int r = -Vector2Int.one;
        int maxX = board.GetBoardSize().x - 1;
        int maxY = board.GetBoardSize().y - 1;

        //print("curr pos: " + currPos + ",   distance: " + nudgeDistanceSquares + ",   nudge dir: " + _nudgeDir.Item1 + ",   nudge dir: " + _nudgeDir.Item2); ;
        switch (_nudgeDir.Item1)
        {
            case NudgeDir.NORTH:
                for (int i = nudgeDistanceSquares; i > 0; i--)
                {
                    if (currPos.y + i > maxY)
                        continue;

                    if (nodes[currPos.x, currPos.y + i] != null)
                    {
                        if (units[currPos.x, currPos.y + i] == null && nodes[currPos.x, currPos.y + i].walkable == true)
                        {
                            r = new Vector2Int(currPos.x, currPos.y + i);
                            break;
                        }
                        else
                            continue;
                    }
                }
                break;
            case NudgeDir.NORTH_EAST:
                for (float i = nudgeDistanceSquares * 0.7072f; i > 0; i -= 0.7072f)
                {
                    i = Mathf.RoundToInt(i);
                    if (currPos.x + i > maxX || currPos.y + i > maxY)
                        continue;

                    if (nodes[currPos.x + (int)i, currPos.y + (int)i] != null)
                    {
                        if (units[currPos.x + (int)i, currPos.y + (int)i] == null && nodes[currPos.x + (int)i, currPos.y + (int)i].walkable == true)
                        {
                            r = new Vector2Int(currPos.x + (int)i, currPos.y + (int)i);
                            break;
                        }   
                        else
                            continue;
                    }
                }
                break;
            case NudgeDir.EAST:
                for (int i = nudgeDistanceSquares; i > 0; i--)
                {
                    if (currPos.x + i > maxX)
                        continue;

                    if (nodes[currPos.x + i, currPos.y] != null)
                    {
                        if (units[currPos.x + i, currPos.y] == null && nodes[currPos.x + i, currPos.y].walkable == true)
                        {
                            r = new Vector2Int(currPos.x + i, currPos.y);
                            break;
                        }
                        else
                            continue;
                    }
                }
                break;
            case NudgeDir.SOUTH_EAST:
                for (float i = nudgeDistanceSquares * 0.7072f; i > 0; i -= 0.7072f)
                {
                    i = Mathf.RoundToInt(i);
                    if (currPos.x + i > maxX || currPos.y - i < 0)
                        continue;

                    if (nodes[currPos.x + (int)i, currPos.y - (int)i] != null)
                    {
                        if (units[currPos.x + (int)i, currPos.y - (int)i] == null && nodes[currPos.x + (int)i, currPos.y - (int)i].walkable == true)
                        {
                            r = new Vector2Int(currPos.x + (int)i, currPos.y - (int)i);
                            break;
                        }
                        else
                            continue;
                    }
                }
                break;
            case NudgeDir.SOUTH:
                for (int i = nudgeDistanceSquares; i > 0; i--)
                {
                    if (currPos.y - i < 0)
                        continue;

                    if (nodes[currPos.x, currPos.y - i] != null)
                    {
                        if (units[currPos.x, currPos.y - i] == null && nodes[currPos.x, currPos.y - i].walkable == true)
                        {
                            r = new Vector2Int(currPos.x, currPos.y - i);
                            break;
                        }
                        else
                            continue;
                    }
                }
                break;
            case NudgeDir.SOUTH_WEST:
                for (float i = nudgeDistanceSquares * 0.7072f; i > 0; i -= 0.7072f)
                {
                    i = Mathf.RoundToInt(i);
                    if (currPos.x - i < 0 || currPos.y - i < 0)
                        continue;

                    if (nodes[currPos.x - (int)i, currPos.y - (int)i] != null)
                    {
                        if (units[currPos.x - (int)i, currPos.y - (int)i] == null && nodes[currPos.x - (int)i, currPos.y - (int)i].walkable == true)
                        {
                            r = new Vector2Int(currPos.x - (int)i, currPos.y - (int)i);
                            break;
                        }
                        else
                            continue;
                    }
                }
                break;
            case NudgeDir.WEST:
                for (int i = nudgeDistanceSquares; i > 0; i--)
                {
                    if (currPos.x - i < 0)
                        continue;

                    if (nodes[currPos.x - i, currPos.y] != null)
                    {
                        if (units[currPos.x - i, currPos.y] == null && nodes[currPos.x - i, currPos.y].walkable == true)
                        {
                            r = new Vector2Int(currPos.x - i, currPos.y);
                            break;
                        }
                        else
                            continue;
                    }
                }
                break;
            case NudgeDir.NORT_WEST:
                for (float i = nudgeDistanceSquares * 0.7072f; i > 0; i -= 0.7072f)
                {
                    i = Mathf.RoundToInt(i);
                    if (currPos.x - i < 0 || currPos.y + i > maxY)
                        continue;

                    if (nodes[currPos.x - (int)i, currPos.y + (int)i] != null)
                    {
                        if (units[currPos.x - (int)i, currPos.y + (int)i] == null && nodes[currPos.x - (int)i, currPos.y + (int)i].walkable == true)
                        {
                            r = new Vector2Int(currPos.x - (int)i, currPos.y + (int)i);
                            break;
                        }
                        else
                            continue;
                    }
                }
                break;
        }

        return r;
    }

    //Tuple<NudgeDir, Vector3> _nudgeDir = DetermineNudgeDir(dir);
    //currentlyDragging.GetNudged(nudgeIsChip, 
    Vector2Int GetDirectionFromNudgeDir(int x, int y, NudgeDir dir)
    {
        Vector2Int r = -Vector2Int.one;

        Node[,] nodes = board.nodes;
        Unit[,] units = board.GetUnits();
        int maxX = board.GetBoardSize().x - 1;
        int maxY = board.GetBoardSize().y - 1;

        switch ( dir )
        {
            case NudgeDir.NORTH:
                break;
            default:
                break;
        }

        return r;
    }
}
