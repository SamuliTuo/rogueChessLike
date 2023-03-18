using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [SerializeField] private int TILE_COUNT_X = 20;
    [SerializeField] private int TILE_COUNT_Y = 20;
    public Vector2Int GetTilecount()
    {
        return new Vector2Int(TILE_COUNT_X, TILE_COUNT_Y);
    }

    [Header("Art Stuff")]
    [SerializeField] private Material tileMat;
    [SerializeField] private Material gardenMat;
    [SerializeField] private Material queueMat;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.3f;
    [SerializeField] private float deathSpacing = 0.3f;
    [SerializeField] private float draggingScale = 0.8f;
    [SerializeField] private float draggingOffset = 1.5f;

    [Header("Materials")]
    //[SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    public static Chessboard Instance { get; private set; }
    public Pathfinding Pathfinding { get; private set; }

    // LOGIC
    private Unit[,] activeUnits;
    public Unit[,] GetUnits() { return activeUnits; }

    [HideInInspector] public GameObject[,] tiles;
    [HideInInspector] public Node[,] nodes;
    private Unit currentlyDragging;

    public Vector2Int GetBoardSize() { return new(TILE_COUNT_X, TILE_COUNT_Y); }
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<Unit> deadUnits_player = new List<Unit>();
    private List<Unit> deadUnits_enemy = new List<Unit>();
    private Camera currentCam;
    private Vector2Int currentHover;
    GameObject platform_team0;
    GameObject platform_team1;

    [SerializeField] private GameObject enemy;

    private void Awake() 
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        GenerateGrid(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        Pathfinding = GetComponent<Pathfinding>();
    }

    private void Start()
    {
        platform_team0 = Resources.Load<GameObject>("units/_platforms/platform_team0");
        platform_team1 = Resources.Load<GameObject>("units/_platforms/platform_team1");

        SpawnAllUnits(GameManager.Instance.currentScenario);
        PositionAllUnits();
    }

    
    public void UnitPlacerUpdate()
    {
        if (!currentCam)
        {
            currentCam = Camera.main;
            return;
        }
        if (GameManager.Instance.state != GameState.PRE_BATTLE)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the indexes of the tiles I've hit
            Vector2Int hitPosition = LookupTileIndex(hit.transform.gameObject);

            // If hovering a tile after not hovering any tile
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If already were hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                // Spawn enemy if empty
                if (Input.GetMouseButton(0) && currentlyDragging == null && activeUnits[hitPosition.x, hitPosition.y] == null)
                {
                    activeUnits[hitPosition.x, hitPosition.y] = SpawnSingleUnit(enemy, 1);
                    PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                }
                // Remove if RighClicking an unit
                else if (Input.GetMouseButton(1) && activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    var u = activeUnits[hitPosition.x, hitPosition.y].gameObject;
                    activeUnits[hitPosition.x, hitPosition.y] = null;
                    u.GetComponent<UnitHealth>().GetDamaged(Mathf.Infinity);
                }

                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                // Clicked an unit:
                if (activeUnits[hitPosition.x, hitPosition.y] != null)
                {
                    // Is it our turn?
                    if (GameManager.Instance.state == GameState.PRE_BATTLE)
                    {
                        currentlyDragging = activeUnits[hitPosition.x, hitPosition.y];

                        // Get a list of where i can go, highlight the tiles
                        availableMoves = GetEmptyCoordinates(); //currentlyDragging.GetAvailableMoves(ref activeUnits, TILE_COUNT_X, TILE_COUNT_Y);
                        HighlightTiles();
                    }
                }
                else // SPAWN ENEMY UNIT WHEN CLICKED EMPTY  \\
                {
                    activeUnits[hitPosition.x, hitPosition.y] = SpawnSingleUnit(enemy, 1);
                    PositionSingleUnit(hitPosition.x, hitPosition.y, true);
                }
            }

            // If we are releasing the mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPos = new Vector2Int(currentlyDragging.x, currentlyDragging.y);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y, ref availableMoves);
                if (!validMove)
                {
                    currentlyDragging.SetPosition(GetTileCenter(previousPos.x, previousPos.y));
                }
                currentlyDragging.SetScale(Vector3.one);
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.x, currentlyDragging.y));
                currentlyDragging = null;
                RemoveHighlightTiles();
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

    public bool TryMoveUnit(Unit unit, Vector2Int targetPos)
    {
        Vector2Int previousPos = new Vector2Int(unit.x, unit.y);

        List<Vector2Int> availableMoves = unit.GetAvailableMoves(ref activeUnits, TILE_COUNT_X, TILE_COUNT_Y);
        bool validMove = MoveTo(unit, targetPos.x, targetPos.y, ref availableMoves);
        if (!validMove)
        {
            unit.SetPosition(GetTileCenter(previousPos.x, previousPos.y));
            unit.ResetPath();
            return false;
        }
        else return true;
        //currentlyDragging.SetScale(Vector3.one);
        //currentlyDragging = null;
        //RemoveHighlightTiles();
    }

    // Board
    private void GenerateGrid(float tileSize, int tileCountX, int tileCountY)
    {
        tiles = new GameObject[tileCountX, tileCountY];
        nodes = new Node[tileCountX, tileCountY];
        GenerateKitchen(tileSize, 0, tileCountX, tileCountY);
        //GenerateGarden(tileSize, 8, 10, tileCountY);
        //GenerateQueue(tileSize, 0, 1, tileCountY);
    }
    private void GenerateKitchen(float tileSize, int fromRow, int toRow, int tileCountY)
    {
        yOffset += transform.position.y;
        int tileCountX = toRow - fromRow;

        for (int x = fromRow; x < toRow; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y, tileMat, "Tile");
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y, Material material, string layer)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = material;


        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize);
        vertices[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize);
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y * tileSize);
        vertices[3] = new Vector3((x+1) * tileSize, yOffset, (y+1) * tileSize);

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        /////////////////////// 
        // jätän nää nyt tänne pyörimään vihjeeks sitä varten, kun alan taas tekee esteitä kartalle:
        /*
        if ((x==7 && y==3) || (x==7 && y==5))
        {
            tileObject.layer = LayerMask.NameToLayer("Sink");
            nodes[x, y] = new Node(false, x, y, NodeType.SINK);
            Instantiate(Resources.Load("furniture_sink") as GameObject, GetTileCenter(x, y), Quaternion.identity);
        }
        else if ((x==2 && y==2) || (x==4 && y==2))
        {
            tileObject.layer = LayerMask.NameToLayer("Counter");
            nodes[x, y] = new Node(false, x, y, NodeType.COUNTER);
            Instantiate(Resources.Load("furniture_counter") as GameObject, GetTileCenter(x, y), Quaternion.identity);
        }
        else if (x==5 && y==9)
        {
            tileObject.layer = LayerMask.NameToLayer("Fridge");
            nodes[x, y] = new Node(false, x, y, NodeType.FRIDGE);
            Instantiate(Resources.Load("furniture_fridge") as GameObject, GetTileCenter(x, y), Quaternion.identity);
        }
        else if (x==1 && y==0)
        {
            tileObject.layer = LayerMask.NameToLayer("Counter");
            nodes[x,y] = new Node(false, x, y, NodeType.PIZZA_BOXES);
            Instantiate(Resources.Load("furniture_pizzaBoxes") as GameObject, GetTileCenter(x, y), Quaternion.identity);
        }
        else if (x==0 && y==9)
        {
            tileObject.layer = LayerMask.NameToLayer("Counter");
            nodes[x,y] = new Node(false, x, y, NodeType.OVEN);
            Instantiate(Resources.Load("furniture_oven") as GameObject, GetTileCenter(x, y), Quaternion.identity);
        }
        */
        ///////////////////////
        tileObject.layer = LayerMask.NameToLayer(layer);
        nodes[x, y] = new Node(true, x, y, NodeType.NONE);

        tileObject.AddComponent<BoxCollider>().size = new Vector3(tileSize, 0.1f, tileSize);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return tileObject;
    }
    public void AddTileCount(int x, int y)
    {
        currentHover = -Vector2Int.one;

        TILE_COUNT_X = Mathf.Max(1, TILE_COUNT_X + x);
        TILE_COUNT_Y = Mathf.Max(1, TILE_COUNT_Y + y);
        RefreshBoard();
    }
    public void RefreshBoard(Unit[,] quickSave = null)
    {
        foreach (var unit in activeUnits)
        {
            if (unit == null) continue;
            unit.GetComponent<UnitHealth>().Die();
        }
        foreach (var tile in tiles)
        {
            if (tile == null) continue;
            Destroy(tile.gameObject);
        }

        nodes = null;
        activeUnits = null;
        tiles = null;
        GenerateGrid(tileSize, TILE_COUNT_X, TILE_COUNT_Y);

        if (quickSave != null) SpawnAllUnits(quickSave);
        else SpawnAllUnits(GameManager.Instance.currentScenario);

        PositionAllUnits();  
    }
    public List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;


                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < TILE_COUNT_X
                    && checkY >= 0 && checkY < TILE_COUNT_Y)
                {
                    neighbours.Add(nodes[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    private bool IsOfType(int x, int y, NodeType type)
    {
        if (x < TILE_COUNT_X && x >= 0 && y < TILE_COUNT_Y && y >= 0) {
            return nodes[x,y].type == type;
        }
        return false;
    }

    public bool IsNeighbourOfType(int x, int y, NodeType type)
    {
        return (IsOfType(x-1, y, type) || IsOfType(x+1, y, type) || IsOfType(x, y-1, type) || IsOfType(x, y+1, type));
    }

    public void SpawnUnit(GameObject unit, int team, Vector2Int pos)
    {
        if (activeUnits[pos.x, pos.y] != null)
            return;

        activeUnits[pos.x, pos.y] = SpawnSingleUnit(unit, team);
    }
    private void SpawnAllUnits(Scenario scenario)
    {
        activeUnits = new Unit[TILE_COUNT_X, TILE_COUNT_Y];
        if (scenario.scenarioUnits == null)
            scenario.scenarioUnits = new List<Scenario.ScenarioUnit>();

        foreach (var unit in scenario.scenarioUnits)
        {
            if (unit.spawnPosX >= 0 && unit.spawnPosX < TILE_COUNT_X)
                if (unit.spawnPosY >= 0 && unit.spawnPosY < TILE_COUNT_Y)
                {
                    print(unit.unit);
                    print(GameManager.Instance.UnitSavePaths);
                    var path = GameManager.Instance.UnitSavePaths.GetPath(unit.unit);
                    activeUnits[unit.spawnPosX, unit.spawnPosY]
                        = SpawnSingleUnit(path, unit.team);
                }
        }
        /*foreach (var unit in scenario.scenarioUnits)
            if (unit.spawnPos.x >= 0 && unit.spawnPos.x < TILE_COUNT_X)
                if (unit.spawnPos.y >= 0 && unit.spawnPos.y < TILE_COUNT_Y)
                    activeUnits[unit.spawnPos.x, unit.spawnPos.y] = SpawnSingleUnit(unit.unit, unit.team);
        */
    }
    private void SpawnAllUnits(Unit[,] _units)
    {
        activeUnits = new Unit[TILE_COUNT_X, TILE_COUNT_Y];
        for (int x = 0; x < _units.GetLength(0); x++)
            for (int y = 0; y < _units.GetLength(1); y++)
                if (_units[x,y] != null)
                    activeUnits[x,y] = SpawnSingleUnit(ScenarioBuilder.Instance.GetOriginalUnitType_From_InstantiatedUnitObject(_units[x, y].gameObject), _units[x, y].team);
    }

    
    public Unit SpawnSingleUnit(GameObject _unit, int team)
    {
        Unit unit = Instantiate(_unit, transform).GetComponent<Unit>();
        
        unit.team = team;
        if (unit.team == 0)
        {
            GameObject unit_platform = Instantiate(platform_team0, unit.transform);
        }
        else
        {
            GameObject unit_platform = Instantiate(platform_team1, unit.transform);
        }

        return unit;
    }
    public Unit SpawnSingleUnit(string unitName, int team)
    {
        GameObject _unit = Resources.Load<GameObject>(unitName);
        Unit unit = Instantiate(_unit, transform).GetComponent<Unit>();

        unit.team = team;
        if (unit.team == 0)
        {
            GameObject unit_platform = Instantiate(platform_team0, unit.transform);
        }
        else
        {
            GameObject unit_platform = Instantiate(platform_team1, unit.transform);
        }

        return unit;
    }

    private void PositionAllUnits()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (activeUnits[x, y] != null)
                    PositionSingleUnit(x, y, true);
    }
    public void PositionSingleUnit(int x, int y, bool force = false)
    {
        activeUnits[x, y].x = x;
        activeUnits[x, y].y = y;
        activeUnits[x, y].SetPosition(GetTileCenter(x, y), force);
    }
    public Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) + new Vector3(tileSize * 0.5f, 0, tileSize * 0.5f);
    }
    public List<Vector2Int> GetEmptyCoordinates()
    {
        List<Vector2Int> r = new List<Vector2Int>();
        foreach (var node in nodes)
        {
            if (activeUnits[node.x, node.y] == null)
            {
                r.Add(new Vector2Int(node.x, node.y));
            }
        }
        return r;
    }

    // Highlight tiles
    public void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
    }
    public void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");

        availableMoves.Clear();
    }

    // Operations
    public bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }
    public bool MoveTo(Unit unit, int x, int y, ref List<Vector2Int> moves)
    {
        if (!ContainsValidMove(ref moves, new Vector2(x,y)))
            return false;

        Vector2Int previousPos = new(unit.x, unit.y);

        // Is there another unit on the target pos?
        if (activeUnits[x,y] != null)
        {
            return false;
            /*Unit other = activeUnits[x, y];

            if (unit.team == other.team)
            {
                return false;
            }


            //puts the dead unit to the side of board 
            // Player unit doing damage
            if (unit.team == 0)
            {
                //other.DoDamage(10);
                deadUnits_enemy.Add(other);
                other.SetScale(Vector3.one * deathSize);
                other.SetPosition(
                    new Vector3(-1 * tileSize, yOffset, 20 * tileSize)
                    + new Vector3(tileSize * 0.5f, 0, tileSize * 0.5f)
                    + (Vector3.back * deathSpacing) * deadUnits_enemy.Count);
            }
            // Enemy unit doing damage to player
            else
            {
                deadUnits_player.Add(other);
                other.SetScale(Vector3.one * deathSize);
                other.SetPosition(
                    new Vector3(20 * tileSize, yOffset, -1 * tileSize)
                    + new Vector3(tileSize * 0.5f, 0, tileSize * 0.5f)
                    + (Vector3.forward * deathSpacing) * deadUnits_player.Count);
            }
            */
        }

        activeUnits[x, y] = unit;
        activeUnits[previousPos.x, previousPos.y] = null;

        PositionSingleUnit(x, y);

        return true;
    }
    void MoveToGrave(Unit unit)
    {
        //Player units
        if (unit.team == 0)
        {
            deadUnits_player.Add(unit);
            activeUnits[unit.x, unit.y] = null;
            unit.SetScale(Vector3.one * deathSize);
            unit.SetPosition(
                new Vector3(-1 * tileSize, yOffset, 20 * tileSize)
                + new Vector3(tileSize * 0.5f, 0, tileSize * 0.5f)
                + (Vector3.back * deathSpacing) * deadUnits_enemy.Count);
        }
        else
        {
            unit.SetPosition(
                new Vector3(20 * tileSize, yOffset, -1 * tileSize)
                + new Vector3(tileSize * 0.5f, 0, tileSize * 0.5f)
                + (Vector3.forward * deathSpacing) * deadUnits_player.Count);
        }   
    }

    public Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x,y] == hitInfo)
                    return new Vector2Int(x,y);

        return -Vector2Int.one; // Invalid
    }

    public Unit GetLowestTeammate(UnitSearchType searchType, Unit askingUnit, int range = 100000, bool canBeSelf = true, bool dontReturnFullHPTargets = true)
    {
        Unit u = null;
        float lowest = Mathf.Infinity;
        foreach (Unit unit in activeUnits)
        {
            if (unit == null)
                continue;
            if (unit.team != askingUnit.team)// || unit == askingUnit)
                continue;
            if (range < Pathfinding.GetDistance(nodes[askingUnit.x, askingUnit.y], nodes[unit.x, unit.y]))
                continue;

            if (searchType == UnitSearchType.LOWEST_HP_ALLY_PERC)
            {
                var hpPerc = unit.GetComponent<UnitHealth>().GetHealthPercentage();

                if (hpPerc >= 1 && dontReturnFullHPTargets)
                    continue;
                if (unit == askingUnit && !canBeSelf)
                    continue;

                if (hpPerc < lowest)
                {
                    lowest = hpPerc;
                    u = unit;
                }
            }

            else if (searchType == UnitSearchType.LOWEST_HP_ALLY_ABS)
            {
                if (
                    unit.GetComponent<UnitHealth>().hp < lowest 
                    && unit.GetComponent<UnitHealth>().GetHealthPercentage() < 1 
                    && Pathfinding.GetDistance(nodes[askingUnit.x, askingUnit.y], nodes[unit.x, unit.y]) <= range
                    && (unit != askingUnit || canBeSelf)
                    )
                {
                    lowest = unit.GetComponent<UnitHealth>().hp;
                    u = unit;
                }
            }
        }
        return u;
    }
}
