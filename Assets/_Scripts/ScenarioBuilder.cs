using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class SpawnableUnit
{
    public GameObject unit;
    public Sprite image;
}
public class ScenarioBuilder : MonoBehaviour
{
    [SerializeField] Image currentlyChosenImage;


    public List<SpawnableUnit> units = new List<SpawnableUnit>();

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

    [SerializeField] private float yOffset = 0.2f;

    private int currentTeam = 1;
    private int currentlyChosenUnit_Index;
    private GameObject currentlyChosenUnit;
    private Chessboard board;
    private Camera currentCam;
    private Vector2Int currentHover;
    private Unit currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();

    private void Start()
    {  
        board = Chessboard.Instance;
        if (units.Count > 0)
        {
            currentlyChosenUnit_Index = 0;
            currentlyChosenUnit = units[0].unit;
            //check if currentlychosenimage is null
            if (currentlyChosenImage != null)
                currentlyChosenImage.sprite = units[0].image;
        }
    }

    public void NextSpawableUnit()
    {
        currentlyChosenUnit_Index++;
        if (currentlyChosenUnit_Index >= units.Count)
        {
            currentlyChosenUnit_Index = 0;
        }
        currentlyChosenUnit = units[currentlyChosenUnit_Index].unit;
        currentlyChosenImage.sprite = units[currentlyChosenUnit_Index].image;
    }
    public void PrevSpawableUnit()
    {
        currentlyChosenUnit_Index--;
        if (currentlyChosenUnit_Index < 0)
        {
            currentlyChosenUnit_Index = units.Count - 1;
        }
        currentlyChosenUnit = units[currentlyChosenUnit_Index].unit;
        currentlyChosenImage.sprite = units[currentlyChosenUnit_Index].image;
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
            

        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        var activeUnits = board.GetUnits();
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
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

                board.tiles[currentHover.x, currentHover.y].layer = (board.ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
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
                board.tiles[currentHover.x, currentHover.y].layer = (board.ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
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
    public GameObject GetOriginalUnitType_From_InstantiatedUnitObject(GameObject unit)
    {
        foreach (var u in units)
        {
            if (unit.name.Contains(u.unit.name))
            {
                return u.unit;
            }
        }
        return null;
    }
}
