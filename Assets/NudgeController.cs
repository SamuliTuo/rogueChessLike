using System;
using System.Collections.Generic;
using UnityEngine;

public class NudgeController : MonoBehaviour
{
    public float nudgeCooldown = 1;
    public bool canNudge = true;
    [SerializeField] private int nudgeLayer = 11;
    [SerializeField] private NudgeArms arms = null;
    [SerializeField] private float stunDuration_primaryTarget = 0.3f;
    [SerializeField] private float stunDuration_secondaryTargets = 1.5f;

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
    Dictionary<CompassDir, Vector2Int> compass;
    private bool nudgeIsChip = false;
    NudgeUIController nudgeUIController;

    private void Awake()
    {
        board = GetComponent<Chessboard>();
        arrowGenerator = GetComponentInChildren<ArrowGenerator>();
        nudgeUIController = GameObject.Find("Canvas").GetComponentInChildren<NudgeUIController>();
        currentCam = Camera.main;
    }

    private void Start()
    {
        t = nudgeCooldown;
        MakeCompass();

        layerMask = GameManager.Instance.boardLayerMask;
        nudgeLayerMask |= (1 << nudgeLayer);
    }

    bool dying = false;
    public void Nudge(Vector3 dir, float mag, Unit unit, float stunTime = 0.5f)
    {
        if ((int)mag == 0 || unit == null)
        {
            return;
        }
        unit.SetScale(Vector3.one);
        unit.GetStunned(stunTime, false);
        Tuple<CompassDir, Vector3> _nudgeDir = HelperUtilities.DetermineCompassDir(dir);
        
        // animation:
        unit.GetNudged(nudgeIsChip, _nudgeDir.Item2); //GetDirectionFromNudgeDir());

        Vector2Int moveToNode;
        if (nudgeIsChip)
        { // chip  (throw unit over other units, objects and chasms)
            moveToNode = CheckNodesInNudgeDirection_chip(
                            new Vector2Int(unit.x, unit.y),
                            _nudgeDir,
                            (int)mag);
        }
        else
        { // nudge  (push unit in a line)
            moveToNode = CheckNodesInNudgeDirectionWhenNudging(
                            new Vector2Int(unit.x, unit.y),
                            _nudgeDir,
                            (int)mag);
        }

        if (moveToNode != -Vector2Int.one)
        {
            if (board.nodes[moveToNode.x, moveToNode.y].tileTypeLayerName == "Empty" || board.nodes[moveToNode.x, moveToNode.y].tileTypeLayerName == "Water")
            {
                dying = true;
            }
            else
            {
                dying = false;
            }
            // tänne jatkamaan unittien stunnaamista nudgechip paikassa! tai tuolla ylemmäl mis tsekataan jo nudgeischip
            //var units = board.GetUnits();
            //var moves = unit.GetAvailableMoves(ref units, board.GetTilecount().x, board.GetTilecount().y);
            board.PushUnit(unit, moveToNode.x, moveToNode.y, mag, nudgeIsChip, dying);
            //board.MoveTo(unit, moveToNode.x, moveToNode.y, ref moves, true);
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
        // Check what mouse is hovering and try to start the nudge:
        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        var activeUnits = board.GetUnits();
        if (currentlyDragging == null && Physics.Raycast(ray, out hit, 100, layerMask))
        {
            // Get the indexes of the tiles I've hit
            Vector2Int hitPosition = board.LookupTileIndex(hit.transform.gameObject);
            if (hitPosition == -Vector2Int.one)
                return;

            // If hovering a tile with an unit, after not hovering any tile
            var _unit = activeUnits[hitPosition.x, hitPosition.y];
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                if (_unit)
                {
                    if (_unit.team == 0)
                    {
                        board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                    }
                }
            }

            // If already were hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                if (_unit)
                {
                    if (_unit.team == 0)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            currentlyDragging = _unit;
                            draggingStartPos = board.GetTileCenter(_unit.x, _unit.y);
                        }
                        board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                    }
                }

                board.tiles[currentHover.x, currentHover.y].layer =
                    (board.ContainsValidMove(ref availableMoves, currentHover)) ?
                        LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer(board.nodes[currentHover.x, currentHover.y].tileTypeLayerName);
                currentHover = hitPosition;
            }

            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
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
            float mag = Mathf.Clamp(direction.magnitude, 0, maxNudgeMagnitude);
            arrowGenerator.UpdateArrow(new Vector3(_pos.x, transform.position.y, _pos.z), direction, Mathf.Clamp(mag * 1.3f, 0.5f, 8f));

            Tuple<CompassDir, Vector3> _nudgeDir = HelperUtilities.DetermineCompassDir(direction);
            Tuple<Vector2Int, bool> moveToNode = CheckNodesInNudgeDirection(new Vector2Int(currentlyDragging.x, currentlyDragging.y), _nudgeDir, (int)mag);
            arms.UpdateNudgerAimPosition(mag / maxNudgeMagnitude, _pos, direction);
            board.RemoveAllHighlightTiles();

            if (moveToNode.Item1 != -Vector2Int.one)
            {
                if (moveToNode.Item2 == true)
                {
                    board.tiles[moveToNode.Item1.x, moveToNode.Item1.y].layer = LayerMask.NameToLayer("Highlight");
                }
                else
                {
                    board.tiles[moveToNode.Item1.x, moveToNode.Item1.y].layer = LayerMask.NameToLayer("Hover");
                }
            }
            
            // If we are releasing the mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                arms.NudgerNudge();
                arrowGenerator.DeactivateArrow();
                Nudge(direction, mag, currentlyDragging, stunDuration_primaryTarget);
                currentlyDragging = null;
            }
        }

        // mouse 2 (chip) :
        else if (nudgeIsChip && currentlyDragging != null && Physics.Raycast(ray, out hit, 100, nudgeLayerMask))
        {
            Vector3 _pos = board.GetTileCenter(currentlyDragging.x, currentlyDragging.y);
            Vector3 direction = new Vector3(_pos.x, hit.point.y, _pos.z) - hit.point;
            float mag = Mathf.Clamp(direction.magnitude, 0, maxNudgeMagnitude);
            arrowGenerator.UpdateArrow(new Vector3(_pos.x, transform.position.y, _pos.z), direction, mag);

            Tuple<CompassDir, Vector3> _nudgeDir = HelperUtilities.DetermineCompassDir(direction);
            Vector2Int moveToNode = CheckNodesInNudgeDirection_chip(new Vector2Int(currentlyDragging.x, currentlyDragging.y), _nudgeDir, (int)mag);

            arms.UpdateChipperAimPosition(mag / maxNudgeMagnitude, _pos, direction);
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
                Nudge(direction, mag, currentlyDragging, stunDuration_primaryTarget);
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

    Tuple<Vector2Int, bool> CheckNodesInNudgeDirection(Vector2Int currPos, Tuple<CompassDir, Vector3> _nudgeDir, int nudgeDistanceSquares)
    {
        if (nudgeDistanceSquares <= 0) 
            return new(-Vector2Int.one, false);

        nudgeDistanceSquares = Mathf.Clamp(nudgeDistanceSquares, 1, 5);
        Node[,] nodes = board.nodes;
        Unit[,] units = board.GetUnits();
        Vector2Int r = -Vector2Int.one;
        int maxX = board.GetBoardSize().x;
        int maxY = board.GetBoardSize().y;

        foreach (var compassDirection in compass)
        {
            if (compassDirection.Key == _nudgeDir.Item1)
            {
                bool isCardinal = CheckIfIsCardinal(compassDirection.Key);
                float increment = isCardinal ? 1 : 0.7072f;

                for (float i = 1; i < nudgeDistanceSquares * increment; i += increment)
                {
                    var currX = Mathf.RoundToInt(currPos.x + (compassDirection.Value.x * i));
                    var currY = Mathf.RoundToInt(currPos.y + (compassDirection.Value.y * i));
                    if (currX < 0 || currX >= maxX || currY < 0 || currY >= maxY || nodes[currX, currY] == null)
                    {
                        return new(r, true);
                    }

                    if (units[currX, currY] == null && nodes[currX, currY].walkable == true)
                    {
                        r = new Vector2Int(currX, currY);
                    }
                    else if (nodes[currX, currY].tileTypeLayerName == "Empty" || nodes[currX, currY].tileTypeLayerName == "Water")
                    {
                        return new(new Vector2Int(currX, currY), false);
                    }
                    else
                    {
                        return new(r, true);
                    }
                }
                return new(r, true);
            }
        }
        return new(r, true);
    }

    Vector2Int CheckNodesInNudgeDirectionWhenNudging(Vector2Int currPos, Tuple<CompassDir, Vector3> _nudgeDir, int nudgeDistanceSquares)
    {
        if (nudgeDistanceSquares <= 0)
            return -Vector2Int.one;

        nudgeDistanceSquares = Mathf.Clamp(nudgeDistanceSquares, 1, 5);
        Node[,] nodes = board.nodes;
        Vector2Int r = -Vector2Int.one;
        int maxX = board.GetBoardSize().x;
        int maxY = board.GetBoardSize().y;

        foreach (var compassDirection in compass)
        {
            if (compassDirection.Key == _nudgeDir.Item1)
            {
                bool isCardinal = CheckIfIsCardinal(compassDirection.Key);
                float increment = isCardinal ? 1 : 0.7072f;

                for (float i = 1; i < nudgeDistanceSquares * increment; i += increment)
                {
                    int currX = Mathf.RoundToInt(currPos.x + (compassDirection.Value.x * i));
                    int currY = Mathf.RoundToInt(currPos.y + (compassDirection.Value.y * i));
                    if (currX < 0 || currX >= maxX || currY < 0 || currY >= maxY || nodes[currX, currY] == null)
                    {
                        return r;
                    }

                    if (nodes[currX, currY].tileTypeLayerName == "Empty" || nodes[currX, currY].tileTypeLayerName == "Water")
                    {
                        return new Vector2Int(currX, currY);
                    }

                    if (nodes[currX, currY].walkable == false)
                    {
                        return r;
                    }

                    if (CheckForUnitCollision(currX, currY, Mathf.RoundToInt(nudgeDistanceSquares - i), _nudgeDir))
                    {
                        return r;
                    }
                    r = new Vector2Int(currX, currY);
                }
                return r;
            }
        }
        return r;
    }


    bool CheckForUnitCollision(int x, int y, int remainingDistance, Tuple<CompassDir, Vector3> dir)
    {
        var units = board.GetUnits();
        if (units[x,y] != null)
        {
            //var pushDir = CheckNodesInNudgeDirection(new(x, y), dir, remainingDistance - 1);
            var mag = Mathf.Clamp((dir.Item2.normalized * remainingDistance).magnitude, 0, maxNudgeMagnitude);
            Nudge(dir.Item2.normalized * remainingDistance, mag, units[x,y], stunDuration_secondaryTargets);
            return true;
        }
        return false;
    }


    Vector2Int CheckNodesInNudgeDirection_chip(Vector2Int currPos, Tuple<CompassDir, Vector3> _nudgeDir, int nudgeDistanceSquares)
    {
        if (nudgeDistanceSquares <= 0)
            return -Vector2Int.one;

        nudgeDistanceSquares = Mathf.Clamp(nudgeDistanceSquares, 1, 4);
        Node[,] nodes = board.nodes;
        Unit[,] units = board.GetUnits();
        Vector2Int r = -Vector2Int.one;
        int maxX = board.GetBoardSize().x;
        int maxY = board.GetBoardSize().y;

        foreach (var compassDirection in compass)
        {
            if (compassDirection.Key == _nudgeDir.Item1)
            {
                bool isCardinal = CheckIfIsCardinal(compassDirection.Key);
                float increment = isCardinal ? 1 : 0.7072f;

                for (float i = nudgeDistanceSquares * increment; i > 0; i -= increment)
                {
                    var currX = Mathf.RoundToInt(currPos.x + (compassDirection.Value.x * i));
                    var currY = Mathf.RoundToInt(currPos.y + (compassDirection.Value.y * i));

                    if (currX < 0 || currX >= maxX || currY < 0 || currY >= maxY || nodes[currX, currY] == null)
                        continue;

                    if (units[currX, currY] == null && nodes[currX, currY].walkable == true)
                    {
                        r = new Vector2Int(currX, currY);
                        return r;
                    }
                    else
                        continue;
                }
                return r;
            }
        }
        return r;
    }


    //Tuple<NudgeDir, Vector3> _nudgeDir = DetermineNudgeDir(dir);
    //currentlyDragging.GetNudged(nudgeIsChip, 
    Vector2Int GetDirectionFromNudgeDir(int x, int y, CompassDir dir)
    {
        Vector2Int r = -Vector2Int.one;

        Node[,] nodes = board.nodes;
        Unit[,] units = board.GetUnits();
        int maxX = board.GetBoardSize().x - 1;
        int maxY = board.GetBoardSize().y - 1;

        switch ( dir )
        {
            case CompassDir.NORTH:
                break;
            default:
                break;
        }

        return r;
    }
    Dictionary<CompassDir, Vector2Int> cardinalDirections;
    Dictionary<CompassDir, Vector2Int> interCardinalDirections;

    void MakeCompass()
    {
        compass = new Dictionary<CompassDir, Vector2Int>()
        {
            { CompassDir.NORTH, new(0,1) },
            { CompassDir.NORTH_EAST, new(1,1) },
            { CompassDir.EAST, new(1,0) },
            { CompassDir.SOUTH_EAST, new(1,-1) },
            { CompassDir.SOUTH, new(0,-1) },
            { CompassDir.SOUTH_WEST, new(-1,-1) },
            { CompassDir.WEST, new(-1,0) },
            { CompassDir.NORTH_WEST, new(-1,1) }
        };
        cardinalDirections = new Dictionary<CompassDir, Vector2Int>()
        {
            { CompassDir.NORTH, new(0,1) },
            { CompassDir.EAST, new(1,0) },
            { CompassDir.SOUTH, new(0,-1) },
            { CompassDir.WEST, new(-1,0) }
        };
        interCardinalDirections = new Dictionary<CompassDir, Vector2Int>()
        {
            { CompassDir.NORTH_EAST, new(1,1) },
            { CompassDir.SOUTH_EAST, new(1,-1) },
            { CompassDir.SOUTH_WEST, new(-1,-1) },
            { CompassDir.NORTH_WEST, new(-1,1) }
        };
        //compass = new List<Vector3>
        //{
        //    Vector3.forward,
        //    new Vector3(0.7171068f, 0, 0.7171068f),
        //    Vector3.right,
        //    new Vector3(0.7171068f, 0, -0.7171068f),
        //    Vector3.back,
        //    new Vector3(-0.7171068f, 0, -0.7171068f),
        //    Vector3.left,
        //    new Vector3(-0.7171068f, 0, 0.7171068f)
        //};
    }

    bool CheckIfIsCardinal(CompassDir dir)
    {
        foreach (var direction in cardinalDirections) 
        {
            if (direction.Key == dir)
            {
                return true;
            }
        }
        return false;
    }
}
