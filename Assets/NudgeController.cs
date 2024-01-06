using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NudgeController : MonoBehaviour
{
    public float nudgeCooldown = 1;
    public bool canNudge = true;

    private float t;
    private Camera currentCam;
    private LayerMask layerMask;
    private Chessboard board;
    private Vector2Int currentHover;
    private Unit currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();


    private void Start()
    {
        board = GetComponent<Chessboard>();
        layerMask = GameManager.Instance.boardLayerMask;
    }

    public void NudgerUpdate()
    {
        if (t < 0)
        {
            t -= Time.deltaTime;
            return;
        }
        if (!currentCam)
        {
            currentCam = Camera.main;
            return;
        }


        Nudge();
    }

    public void Nudge()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var activeUnits = Chessboard.Instance.GetUnits();
        //if (Physics.Raycast(ray, out hit, 100, ))
    }

    private void UnitPlacerUpdate()
    {
        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        var activeUnits = board.GetUnits();
        if (Physics.Raycast(ray, out hit, 100, layerMask))
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
                if (Input.GetMouseButtonDown(0) && activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    // jatkuu ->
                }
                // Spawn enemy if empty
                if (Input.GetMouseButton(0) && currentlyDragging == null && activeUnits[hitPosition.x, hitPosition.y] == null)
                {
                    //Unit clone = board.SpawnSingleUnit(currentlyChosenUnit, currentTeam);
                    //activeUnits[hitPosition.x, hitPosition.y] = clone;
                    //clone.spawnRotation = objectRotation;
                    //board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                    //board.RotateSingleUnit(hitPosition.x, hitPosition.y, board.GetCurrentUnitRotation(objectRotation));
                }
                // Remove if RighClicking an unit
                else if (Input.GetMouseButton(1) && activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    var u = activeUnits[hitPosition.x, hitPosition.y].gameObject;
                    activeUnits[hitPosition.x, hitPosition.y] = null;
                    u.GetComponent<UnitHealth>().RemoveHP(Mathf.Infinity, true);
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
                // Clicked an unit:
                if (activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    // Is it our turn?
                    if (GameManager.Instance.state == GameState.SCENARIO_BUILDER)
                    {
                        currentlyDragging = activeUnits[hitPosition.x, hitPosition.y];

                        // Get a list of where i can go, highlight the tiles
                        availableMoves = board.GetEmptyCoordinates(); //currentlyDragging.GetAvailableMoves(ref activeUnits, TILE_COUNT_X, TILE_COUNT_Y);
                        board.HighlightTiles();
                    }
                }
                else // SPAWN ENEMY UNIT WHEN CLICKED EMPTY  \\
                {
                    //Unit clone = board.SpawnSingleUnit(currentlyChosenUnit, currentTeam);
                    //activeUnits[hitPosition.x, hitPosition.y] = clone;
                    //clone.spawnRotation = objectRotation;
                    //board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                    //board.RotateSingleUnit(hitPosition.x, hitPosition.y, board.GetCurrentUnitRotation(objectRotation));
                }
            }
            // Right click
            else if (Input.GetMouseButtonDown(1))
            {
                if (activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    var u = activeUnits[hitPosition.x, hitPosition.y].gameObject;
                    activeUnits[hitPosition.x, hitPosition.y] = null;
                    u.GetComponent<UnitHealth>().RemoveHP(Mathf.Infinity, true);
                }
            }

            // If we are releasing the mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPos = new Vector2Int(currentlyDragging.x, currentlyDragging.y);

                bool validMove = board.MoveTo(currentlyDragging, hitPosition.x, hitPosition.y, ref availableMoves);
                if (!validMove)
                {
                    currentlyDragging.SetPosition(board.GetTileCenter(previousPos.x, previousPos.y));
                }
                currentlyDragging.SetScale(Vector3.one);
                currentlyDragging = null;
                board.RemoveHighlightTiles();
            }
        }
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

        // If we're dragging a piece
        if (currentlyDragging)
        {
            //Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            //float distance;
            //if (horizontalPlane.Raycast(ray, out distance))
            //{
            //    currentlyDragging.SetScale(Vector3.one * draggingScale);
            //    currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * draggingOffset);
            //}
        }
    }
}
