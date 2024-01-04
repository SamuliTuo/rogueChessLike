using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using TMPro;
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
    public static ScenarioBuilder Instance;
    [HideInInspector] public Chessboard board;
    [HideInInspector] public ScenarioBuilderCameraSettings camSettings;

    [SerializeField] private Image currentlyChosenImage_unit;
    [SerializeField] private Image currentlyChosenImage_object;
    [SerializeField] private float draggingScale = 0.8f;
    [SerializeField] private float draggingOffset = 1.5f;
    [SerializeField] private GameObject unitsPanel, terrainPanel, objectsPanel;
    [SerializeField] private float yOffset = 0.2f;

    private int currentTeam = 1;
    private int currentlyChosenUnit_index, currentlyChosenObject_index;
    private GameObject currentlyChosenUnit, currentlyChosenObject;
    private int objectRotation = 0;
    private ScenarioBuilderPanel currentlyOpenPanel = ScenarioBuilderPanel.TERRAIN;
    private NodeType currentNodeType_m1 = NodeType.NONE;
    private NodeType currentNodeType_m2 = NodeType.NONE;
    private int currentNodeGraphicsVariation_m1 = 0;
    private int currentNodeGraphicsVariation_m2 = 0;
    private int currentTileRotation_m1 = 0;
    private int currentTileRotation_m2 = 0;
    private Camera currentCam;
    private Vector2Int currentHover;
    private Unit currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private TileGraphics tileGraphics;
    private ScenarioEditorPanel scenarioEditorPanel;
    private LayerMask scenarioEditorLayerMask;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }


    private void Start()
    {
        scenarioEditorLayerMask = LayerMask.GetMask("Tile", "Hover", "Highlight", "Empty", "Swamp", "Grass_purple");
        board = Chessboard.Instance;
        tileGraphics = board.GetComponentInChildren<TileGraphics>();

        if (GameManager.Instance.UnitSavePaths.unitsDatas.Count > 0)
        {
            currentlyChosenUnit_index = 0;
            currentlyChosenUnit = GameManager.Instance.UnitSavePaths.unitsDatas[0].unitPrefab;
            if (currentlyChosenImage_unit != null)
                currentlyChosenImage_unit.sprite = GameManager.Instance.UnitSavePaths.unitsDatas[0].image;
        }
        if (GameManager.Instance.UnitSavePaths)
        {
            currentlyChosenObject_index = 0;
            currentlyChosenObject = GameManager.Instance.ObjectSavePaths.objectDatas[0].objectPrefab;
            if (currentlyChosenImage_unit != null)
                currentlyChosenImage_object.sprite = GameManager.Instance.ObjectSavePaths.objectDatas[0].image;
        }

        camSettings = GameObject.Find("Canvas").GetComponentInChildren<ScenarioBuilderCameraSettings>();
    }


    public void ScenarioBuilderUpdate()
    {
        if (!currentCam)
        {
            currentCam = Camera.main;
            return;
        }

        if (GameManager.Instance.state != GameState.SCENARIO_BUILDER)
            return;

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
            UnitPlacerUpdate();
        else if (currentlyOpenPanel == ScenarioBuilderPanel.TERRAIN)
            TerrainEditorUpdate();
        else if (currentlyOpenPanel == ScenarioBuilderPanel.OBJECTS)
            ObjectPlacerUpdate();
    }


    private void TerrainEditorUpdate()
    {
        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 500, scenarioEditorLayerMask))
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
                // Edit terrain if LeftClicking
                if (Input.GetMouseButton(0) && currentlyDragging == null)
                    ChangeNodeType(hitPosition.x, hitPosition.y, currentTileRotation_m1, currentNodeType_m1, currentNodeGraphicsVariation_m1);

                // Edit terrain to BASIC if RightClicking
                else if (Input.GetMouseButton(1))
                    ChangeNodeType(hitPosition.x, hitPosition.y, currentTileRotation_m2, currentNodeType_m2, currentNodeGraphicsVariation_m2);

                board.tiles[currentHover.x, currentHover.y].layer =
                    (board.ContainsValidMove(ref availableMoves, currentHover)) ?
                        LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer(board.nodes[currentHover.x, currentHover.y].tileTypeLayerName);
                currentHover = hitPosition;
                board.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
                ChangeNodeType(hitPosition.x, hitPosition.y, currentTileRotation_m1, currentNodeType_m1, currentNodeGraphicsVariation_m1);
            else if (Input.GetMouseButtonDown(1))
                ChangeNodeType(hitPosition.x, hitPosition.y, currentTileRotation_m2, currentNodeType_m2, currentNodeGraphicsVariation_m2);

            // If we are releasing the mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPos = new Vector2Int(currentlyDragging.x, currentlyDragging.y);

                bool validMove = board.MoveTo(currentlyDragging, hitPosition.x, hitPosition.y, ref availableMoves);
                if (!validMove)
                    currentlyDragging.SetPosition(board.GetTileCenter(previousPos.x, previousPos.y));

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
        if (Physics.Raycast(ray, out hit, 100, scenarioEditorLayerMask))
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
                // Spawn enemy if empty
                if (Input.GetMouseButton(0) && currentlyDragging == null && activeUnits[hitPosition.x, hitPosition.y] == null)
                {
                    Unit clone = board.SpawnSingleUnit(currentlyChosenUnit, currentTeam);
                    activeUnits[hitPosition.x, hitPosition.y] = clone;
                    clone.spawnRotation = objectRotation;
                    board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                    board.RotateSingleUnit(hitPosition.x, hitPosition.y, board.GetCurrentUnitRotation(objectRotation));
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
                    Unit clone = board.SpawnSingleUnit(currentlyChosenUnit, currentTeam);
                    activeUnits[hitPosition.x, hitPosition.y] = clone;
                    clone.spawnRotation = objectRotation;
                    board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                    board.RotateSingleUnit(hitPosition.x, hitPosition.y, board.GetCurrentUnitRotation(objectRotation));
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
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentlyDragging.SetScale(Vector3.one * draggingScale);
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * draggingOffset);
            }
        }
    }


    private void ObjectPlacerUpdate()
    {
        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        var activeUnits = board.GetUnits();
        if (Physics.Raycast(ray, out hit, 100, scenarioEditorLayerMask))
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
                // Spawn obj if empty
                if (Input.GetMouseButton(0) && currentlyDragging == null && activeUnits[hitPosition.x, hitPosition.y] == null)
                {
                    Unit clone = board.SpawnSingleUnit(currentlyChosenObject, 2);
                    activeUnits[hitPosition.x, hitPosition.y] = clone;
                    clone.spawnRotation = objectRotation;
                    board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                    board.RotateSingleUnit(hitPosition.x, hitPosition.y, board.GetCurrentUnitRotation(objectRotation));
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
                    Unit clone = board.SpawnSingleUnit(currentlyChosenObject, 2);
                    activeUnits[hitPosition.x, hitPosition.y] = clone;
                    clone.spawnRotation = objectRotation;
                    board.PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                    board.RotateSingleUnit(hitPosition.x, hitPosition.y, board.GetCurrentUnitRotation(objectRotation));
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                // Right click
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
        // is object:
        if (unit.GetComponent<Unit>().isObstacle)
        {
            foreach (var o in GameManager.Instance.ObjectSavePaths.objectDatas)
            {
                if (unit.name.Contains(o.objectPrefab.name))
                {
                    return o.objectPrefab;
                }
            }
            return null;
        }
        // is unit:
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
        if (GameManager.Instance.UnitSavePaths.unitsDatas.Count == 1)
            return;

        currentlyChosenUnit_index++;
        if (currentlyChosenUnit_index >= GameManager.Instance.UnitSavePaths.unitsDatas.Count)
        {
            currentlyChosenUnit_index = 0;
        }
        currentlyChosenUnit = GameManager.Instance.UnitSavePaths.unitsDatas[currentlyChosenUnit_index].unitPrefab;
        currentlyChosenImage_unit.sprite = GameManager.Instance.UnitSavePaths.unitsDatas[currentlyChosenUnit_index].image;
    }
    public void PrevSpawableUnit()
    {
        if (GameManager.Instance.UnitSavePaths.unitsDatas.Count == 1)
            return;

        currentlyChosenUnit_index--;
        if (currentlyChosenUnit_index < 0)
        {
            currentlyChosenUnit_index = GameManager.Instance.UnitSavePaths.unitsDatas.Count - 1;
        }
        currentlyChosenUnit = GameManager.Instance.UnitSavePaths.unitsDatas[currentlyChosenUnit_index].unitPrefab;
        currentlyChosenImage_unit.sprite = GameManager.Instance.UnitSavePaths.unitsDatas[currentlyChosenUnit_index].image;
    }
    public void NextSpawableObject()
    {
        if (GameManager.Instance.ObjectSavePaths.objectDatas.Count == 1)
            return;

        currentlyChosenObject_index++;
        if (currentlyChosenObject_index >= GameManager.Instance.ObjectSavePaths.objectDatas.Count)
        {
            currentlyChosenObject_index = 0;
        }
        currentlyChosenObject = GameManager.Instance.ObjectSavePaths.objectDatas[currentlyChosenObject_index].objectPrefab;
        currentlyChosenImage_object.sprite = GameManager.Instance.ObjectSavePaths.objectDatas[currentlyChosenObject_index].image;
    }
    public void PrevSpawableObject()
    {
        if (GameManager.Instance.ObjectSavePaths.objectDatas.Count == 1)
            return;

        currentlyChosenObject_index--;
        if (currentlyChosenObject_index < 0)
        {
            currentlyChosenObject_index = GameManager.Instance.UnitSavePaths.unitsDatas.Count - 1;
        }
        currentlyChosenObject = GameManager.Instance.ObjectSavePaths.objectDatas[currentlyChosenObject_index].objectPrefab;
        currentlyChosenImage_object.sprite = GameManager.Instance.ObjectSavePaths.objectDatas[currentlyChosenObject_index].image;
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

    public void ChangeNodeType(int x, int y, int rotation, NodeType type, int variation)
    {
        switch (type)
        {
            case NodeType.NONE:
                board.ChangeTileGraphics(x, y, "Tile", variation, true, rotation);
                break;
            case NodeType.SWAMP:
                board.ChangeTileGraphics(x, y, "Swamp", variation, true, rotation);
                break;
            case NodeType.HOLE:
                board.ChangeTileGraphics(x, y, "Empty", variation, false, rotation);
                break;
            case NodeType.GRASS_PURPLE:
                board.ChangeTileGraphics(x, y, "Grass_purple", variation, true, rotation);
                break;
            default:
                break;
        }
    }

    public void SetToolCurrentNodeType_m1(int type)
    {
        if (type == 0) currentNodeType_m1 = NodeType.NONE;
        else if (type == 1) currentNodeType_m1 = NodeType.SWAMP;
        else if (type == 2) currentNodeType_m1 = NodeType.HOLE;
        else currentNodeType_m1 = NodeType.GRASS_PURPLE;
    }
    public void SetToolCurrentNodeType_m2(int type)
    {
        if (type == 0) currentNodeType_m2 = NodeType.NONE;
        else if (type == 1) currentNodeType_m2 = NodeType.HOLE;
        else if (type == 2) currentNodeType_m2 = NodeType.SWAMP;
        else currentNodeType_m2 = NodeType.GRASS_PURPLE;
    }
    public void ChangeVariation_m1(bool add)
    {
        if (scenarioEditorPanel == null)
            scenarioEditorPanel = GameObject.Find("Canvas").GetComponentInChildren<ScenarioEditorPanel>();

        if (add)
        {
            currentNodeGraphicsVariation_m1++;
            if (currentNodeGraphicsVariation_m1 >= tileGraphics.GetTiletypeVariationsCount(currentNodeType_m1))
            {
                currentNodeGraphicsVariation_m1 = 0;
            }
        }
        else
        {
            currentNodeGraphicsVariation_m1--;
            if (currentNodeGraphicsVariation_m1 < 0)
            {
                currentNodeGraphicsVariation_m1 = tileGraphics.GetTiletypeVariationsCount(currentNodeType_m1) - 1;
            }
        }
        scenarioEditorPanel.SetVariationText(1, currentNodeGraphicsVariation_m1.ToString());
    }
    public void ChangeVariation_m2(bool add)
    {
        if (scenarioEditorPanel == null)
            scenarioEditorPanel = GameObject.Find("Canvas").GetComponentInChildren<ScenarioEditorPanel>();

        if (add)
        {
            currentNodeGraphicsVariation_m2++;
            if (currentNodeGraphicsVariation_m2 >= tileGraphics.GetTiletypeVariationsCount(currentNodeType_m2))
            {
                currentNodeGraphicsVariation_m2 = 0;
            }
        }
        else
        {
            currentNodeGraphicsVariation_m2--;
            if (currentNodeGraphicsVariation_m2 < 0)
            {
                currentNodeGraphicsVariation_m2 = tileGraphics.GetTiletypeVariationsCount(currentNodeType_m2) - 1;
            }
        }
        scenarioEditorPanel.SetVariationText(2, currentNodeGraphicsVariation_m2.ToString());
    }
    public void ChangeTileRotation_m1(bool add)
    {
        if (scenarioEditorPanel == null)
            scenarioEditorPanel = GameObject.Find("Canvas").GetComponentInChildren<ScenarioEditorPanel>();

        if (add)
        {
            currentTileRotation_m1++;
            if (currentTileRotation_m1 > 3)
            {
                currentTileRotation_m1 = 0;
            }
        }
        else
        {
            currentTileRotation_m1--;
            if (currentTileRotation_m1 < 0)
            {
                currentTileRotation_m1 = 3;
            }
        }
        scenarioEditorPanel.SetRotationText(1, currentTileRotation_m1);
    }
    public void ChangeTileRotation_m2(bool add)
    {
        if (scenarioEditorPanel == null)
            scenarioEditorPanel = GameObject.Find("Canvas").GetComponentInChildren<ScenarioEditorPanel>();

        if (add)
        {
            currentTileRotation_m2++;
            if (currentTileRotation_m2 > 3)
            {
                currentTileRotation_m2 = 0;
            }
        }
        else
        {
            currentTileRotation_m2--;
            if (currentTileRotation_m2 < 0)
            {
                currentTileRotation_m2 = 3;
            }
        }
        scenarioEditorPanel.SetRotationText(2, currentTileRotation_m2);
    }
    public void ChangeObjectRotation(bool add)
    {
        if (scenarioEditorPanel == null)
            scenarioEditorPanel = GameObject.Find("Canvas").GetComponentInChildren<ScenarioEditorPanel>();

        if (add)
        {
            objectRotation++;
            if (objectRotation > 3)
                objectRotation = 0;
        }
        else
        {
            objectRotation--;
            if (objectRotation < 0)
                objectRotation = 3;
        }
        scenarioEditorPanel.SetObjectRotationText(objectRotation);
    }
}
