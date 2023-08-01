using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ScenarioBuilderPanel
{
    UNITS,
    TERRAIN,
    OBJECTS,
    SAVE
}

public class ScenarioBuilder : MonoBehaviour
{
    [SerializeField] Image currentlyChosenImage;

    public Chessboard board;

    public static ScenarioBuilder Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    [SerializeField] private float draggingScale = 0.8f;
    [SerializeField] private float draggingOffset = 1.5f;
    [SerializeField] private GameObject unitsPanel, terrainPanel, objectsPanel;
    [SerializeField] private float yOffset = 0.2f;

    private int currentTeam = 1;
    private int currentlyChosenUnit_Index;
    private GameObject currentlyChosenUnit;
    private ScenarioBuilderPanel currentlyOpenPanel = ScenarioBuilderPanel.TERRAIN;
    private NodeType currentNodeType = NodeType.NONE;
    private Camera currentCam;
    private Vector2Int currentHover;
    private Unit currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();


    private void Start()
    {  
        board = Chessboard.Instance;
        if (GameManager.Instance.UnitSavePaths.unitsDatas.Count > 0)
        {
            currentlyChosenUnit_Index = 0;
            currentlyChosenUnit = GameManager.Instance.UnitSavePaths.unitsDatas[0].unitPrefab;
            //check if currentlychosenimage is null
            if (currentlyChosenImage != null)
                currentlyChosenImage.sprite = GameManager.Instance.UnitSavePaths.unitsDatas[0].image;
        }
    }

    public void SetCurrentNodeType(int type)
    {
        // normal ground
        if (type == 0) 
        {
            currentNodeType = NodeType.NONE;
            terrainPanel.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;
            terrainPanel.transform.GetChild(2).GetComponent<Image>().color = Color.white;
            terrainPanel.transform.GetChild(3).GetComponent<Image>().color = Color.white;
        }
        else if (type == 1)
        {
            currentNodeType = NodeType.EMPTY;
            terrainPanel.transform.GetChild(1).GetComponent<Image>().color = Color.white;
            terrainPanel.transform.GetChild(2).GetComponent<Image>().color = Color.yellow;
            terrainPanel.transform.GetChild(3).GetComponent<Image>().color = Color.white;
        }
        // swamp
        else 
        {
            currentNodeType = NodeType.SWAMP;
            terrainPanel.transform.GetChild(1).GetComponent<Image>().color = Color.white;
            terrainPanel.transform.GetChild(2).GetComponent<Image>().color = Color.white;
            terrainPanel.transform.GetChild(3).GetComponent<Image>().color = Color.yellow;
        }
    }
    public void ScenarioBuilderUpdate()
    {
        if (!currentCam)
        {
            currentCam = Camera.main;
            return;
        }

        if (GameManager.Instance.state != GameState.SCENARIO_BUILDER)
        {
            return;
        }

        if (IsPointerOverUIObject())
        {
            if (currentHover != -Vector2Int.one)
            {
                board.tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
            return;
        }

        if (currentlyOpenPanel == ScenarioBuilderPanel.UNITS)
        {
            UnitPlacerUpdate();
        }
        else if (currentlyOpenPanel == ScenarioBuilderPanel.TERRAIN)
        {
            TerrainEditorUpdate();
        }
        else if (currentlyOpenPanel == ScenarioBuilderPanel.OBJECTS)
        {
            ObjectPlacerUpdate();
        }
    }
    private void TerrainEditorUpdate()
    {
        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Tile", "Hover", "Highlight", "Empty", "Swamp")))
        {
            // Get the indexes of the tiles I've hit
            Vector2Int hitPosition = board.LookupTileIndex(hit.transform.gameObject);
            if (hitPosition == -Vector2Int.one)
            {
                return;
            }

            // If hovering a tile after not hovering any tile
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If already were hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                // Edit terrain if LeftClicking
                if (Input.GetMouseButton(0) && currentlyDragging == null)
                {
                    ChangeNodeType(hitPosition.x, hitPosition.y, currentNodeType);
                }
                // Set terrain to BASIC if RightClicking
                else if (Input.GetMouseButton(1))
                {
                    ChangeNodeType(hitPosition.x, hitPosition.y, NodeType.NONE);
                }

                board.tiles[currentHover.x, currentHover.y].layer =
                    (board.ContainsValidMove(ref availableMoves, currentHover)) ?
                        LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer(board.nodes[currentHover.x, currentHover.y].tileTypeLayerName);
                currentHover = hitPosition;
                board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
                ChangeNodeType(hitPosition.x, hitPosition.y, currentNodeType);
            else if (Input.GetMouseButtonDown(1))
                ChangeNodeType(hitPosition.x, hitPosition.y, NodeType.NONE);

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
        else // If we are not hovering any tile
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
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentlyDragging.SetScale(Vector3.one * draggingScale);
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * draggingOffset);
            }
        }
    }
    private void UnitPlacerUpdate()
    {
        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        var activeUnits = board.GetUnits();
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Tile", "Hover", "Highlight", "Swamp")))
        {
            // Get the indexes of the tiles I've hit
            Vector2Int hitPosition = board.LookupTileIndex(hit.transform.gameObject);
            if (hitPosition == -Vector2Int.one)
            {
                return;
            }

            // If hovering a tile after not hovering any tile
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If already were hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                // Spawn enemy if empty
                if (Input.GetMouseButton(0) && currentlyDragging == null && activeUnits[hitPosition.x, hitPosition.y] == null)
                {
                    activeUnits[hitPosition.x, hitPosition.y] = board.SpawnSingleUnit(currentlyChosenUnit, currentTeam);
                    board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                }
                // Remove if RighClicking an unit
                else if (Input.GetMouseButton(1) && activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    var u = activeUnits[hitPosition.x, hitPosition.y].gameObject;
                    activeUnits[hitPosition.x, hitPosition.y] = null;
                    u.GetComponent<UnitHealth>().GetDamaged(Mathf.Infinity);
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
                    activeUnits[hitPosition.x, hitPosition.y] = board.SpawnSingleUnit(currentlyChosenUnit, currentTeam);
                    board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                // Right click
                if (activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    var u = activeUnits[hitPosition.x, hitPosition.y].gameObject;
                    activeUnits[hitPosition.x, hitPosition.y] = null;
                    u.GetComponent<UnitHealth>().GetDamaged(Mathf.Infinity);
                }
            }
            ////remove when right clicking
            //else if (Input.GetMouseButtonDown(1))
            //{
            //    if (activeUnits[hitPosition.x, hitPosition.y] != null)
            //    {
            //        if (GameManager.Instance.state == GameState.SCENARIO_BUILDER && currentlyDragging != null)
            //        {
            //            var u = activeUnits[hitPosition.x, hitPosition.y].gameObject;
            //            activeUnits[hitPosition.x, hitPosition.y] = null;
            //            u.GetComponent<UnitHealth>().GetDamaged(Mathf.Infinity);
            //        }
            //    }
            //}

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
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentlyDragging.SetScale(Vector3.one * draggingScale);
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * draggingOffset);
            }
        }
    }

    void ObjectPlacerUpdate()
    {
        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        var activeUnits = board.GetUnits();
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Tile", "Hover", "Highlight", "Swamp")))
        {
            // Get the indexes of the tiles I've hit
            Vector2Int hitPosition = board.LookupTileIndex(hit.transform.gameObject);
            if (hitPosition == -Vector2Int.one)
            {
                return;
            }

            // If hovering a tile after not hovering any tile
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If already were hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                // Spawn enemy if empty
                if (Input.GetMouseButton(0) && currentlyDragging == null && activeUnits[hitPosition.x, hitPosition.y] == null)
                {
                    activeUnits[hitPosition.x, hitPosition.y] = board.SpawnSingleUnit(currentlyChosenUnit, currentTeam);
                    board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                }
                // Remove if RighClicking an unit
                else if (Input.GetMouseButton(1) && activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    var u = activeUnits[hitPosition.x, hitPosition.y].gameObject;
                    activeUnits[hitPosition.x, hitPosition.y] = null;
                    u.GetComponent<UnitHealth>().GetDamaged(Mathf.Infinity);
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
                    activeUnits[hitPosition.x, hitPosition.y] = board.SpawnSingleUnit(currentlyChosenUnit, currentTeam);
                    board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                // Right click
                if (activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    var u = activeUnits[hitPosition.x, hitPosition.y].gameObject;
                    activeUnits[hitPosition.x, hitPosition.y] = null;
                    u.GetComponent<UnitHealth>().GetDamaged(Mathf.Infinity);
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
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentlyDragging.SetScale(Vector3.one * draggingScale);
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * draggingOffset);
            }
        }
    }

    public void OpenPanel(ScenarioBuilderPanel panel)
    {
        switch (panel)
        {
            case ScenarioBuilderPanel.TERRAIN:
                unitsPanel.SetActive(false); terrainPanel.SetActive(true); objectsPanel.SetActive(false); currentlyOpenPanel = ScenarioBuilderPanel.TERRAIN; break;
            case ScenarioBuilderPanel.UNITS:
                unitsPanel.SetActive(true); terrainPanel.SetActive(false); objectsPanel.SetActive(false); currentlyOpenPanel = ScenarioBuilderPanel.UNITS; break;
            case ScenarioBuilderPanel.OBJECTS:
                unitsPanel.SetActive(false); terrainPanel.SetActive(false); objectsPanel.SetActive(true); currentlyOpenPanel = ScenarioBuilderPanel.OBJECTS; break;
            case ScenarioBuilderPanel.SAVE:
                currentlyOpenPanel = ScenarioBuilderPanel.SAVE; break;
            default:
                break;
        }
    }

    public GameObject GetOriginalUnitType_From_InstantiatedUnitObject(GameObject unit)
    {
        foreach (var u in GameManager.Instance.UnitSavePaths.unitsDatas)
        {
            if (unit.name.Contains(u.unitPrefab.name))
            {
                return u.unitPrefab;
            }
        }
        return null;
    }


    public void NextSpawableUnit()
    {
        currentlyChosenUnit_Index++;
        if (currentlyChosenUnit_Index >= GameManager.Instance.UnitSavePaths.unitsDatas.Count)
        {
            currentlyChosenUnit_Index = 0;
        }
        currentlyChosenUnit = GameManager.Instance.UnitSavePaths.unitsDatas[currentlyChosenUnit_Index].unitPrefab;
        currentlyChosenImage.sprite = GameManager.Instance.UnitSavePaths.unitsDatas[currentlyChosenUnit_Index].image;
    }


    public void PrevSpawableUnit()
    {
        currentlyChosenUnit_Index--;
        if (currentlyChosenUnit_Index < 0)
        {
            currentlyChosenUnit_Index = GameManager.Instance.UnitSavePaths.unitsDatas.Count - 1;
        }
        currentlyChosenUnit = GameManager.Instance.UnitSavePaths.unitsDatas[currentlyChosenUnit_Index].unitPrefab;
        currentlyChosenImage.sprite = GameManager.Instance.UnitSavePaths.unitsDatas[currentlyChosenUnit_Index].image;
    }


    public void ToggleCurrentTeam()
    {
        if (currentTeam == 0) currentTeam = 1;
        else if (currentTeam == 1) currentTeam = 0;
    }


    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void ChangeNodeType(int x, int y, NodeType type)
    {
        switch (type)
        {
            case NodeType.NONE:
                board.nodes[x, y].tileTypeLayerName = "Tile";
                board.nodes[x, y].walkable = true;
                break;
            case NodeType.SWAMP:
                board.nodes[x, y].tileTypeLayerName = "Swamp";
                board.nodes[x, y].walkable = true;
                break;
            case NodeType.EMPTY:
                board.nodes[x, y].tileTypeLayerName = "Empty";
                board.nodes[x, y].walkable = false;
                break;
            default:
                break;
        }
    }
}
