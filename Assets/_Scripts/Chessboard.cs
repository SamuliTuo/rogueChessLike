using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Chessboard : MonoBehaviour
{
    [SerializeField] private int TILE_COUNT_X = 20;
    [SerializeField] private int TILE_COUNT_Y = 20;
    public Vector2Int GetTilecount()
    {
        return new Vector2Int(TILE_COUNT_X, TILE_COUNT_Y);
    }

    [Header("Art Stuff")]
    [SerializeField] private GameObject tilePrefab_basic;
    [SerializeField] private GameObject tilePrefab_swamp;
    [SerializeField] private GameObject tilePrefab_hole;
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
    public float GetYOffset() { return yOffset; }
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

        //GenerateGrid(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        GenerateGrid(GameManager.Instance.currentScenario);
        Pathfinding = GetComponent<Pathfinding>();
    }
    private void Start()
    {
        platform_team0 = Resources.Load<GameObject>("units/_platforms/platform_team0");
        platform_team1 = Resources.Load<GameObject>("units/_platforms/platform_team1");

        SpawnScenarioUnits(GameManager.Instance.currentScenario);
        SpawnPlayerParty();
        PositionAllUnits();
        Camera.main.GetComponent<CameraManager>()?.SetupBattleCamera(GetBoardSize(), tileSize);
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
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Tile", "Empty", "Swamp", "Hover", "Highlight")))
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
                /*
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
                */
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer(nodes[currentHover.x, currentHover.y].tileTypeLayerName);
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
                        //HighlightTiles();
                    }
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
                tiles[currentHover.x, currentHover.y].layer =
                    (ContainsValidMove(ref availableMoves, currentHover)) ?
                        LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer(nodes[currentHover.x, currentHover.y].tileTypeLayerName);
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
    private void GenerateGrid()//Basic version
    {
        TILE_COUNT_X = 10;
        TILE_COUNT_Y = 10;
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        nodes = new Node[TILE_COUNT_X, TILE_COUNT_Y];
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y, tileMat, "Tile");
    }
    private void GenerateGrid(Scenario scenario, Node[,] quickSaveNodes = null)
    {
        TILE_COUNT_X = scenario.sizeX;
        TILE_COUNT_Y = scenario.sizeY;
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        nodes = new Node[TILE_COUNT_X, TILE_COUNT_Y];

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                // Using this ... when changing the map on the fly.
                if (quickSaveNodes != null)
                {
                    if (x < quickSaveNodes.GetLength(0) && y < quickSaveNodes.GetLength(1))
                    {
                        tiles[x, y] = GenerateSingleTile(tileSize, x, y, tileMat, quickSaveNodes[x, y].tileTypeLayerName, quickSaveNodes[x, y].walkable);
                    }
                    else
                    {
                        tiles[x, y] = GenerateSingleTile(tileSize, x, y, tileMat, "Tile");
                    }
                }
                // ... when loading a scenario.
                else if (scenario.scenarioNodes != null)
                {
                    bool found = false;
                    foreach (var item in scenario.scenarioNodes)
                    {
                        if (item.x == x && item.y == y)
                        {
                            tiles[x, y] = GenerateSingleTile(tileSize, x, y, tileMat, item.terrainLayer, item.walkable == 1 ? true : false);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        tiles[x, y] = GenerateSingleTile(tileSize, x, y, tileMat, "Tile");
                    }
                }
                // ... when it's a fresh board.
                else
                {
                    tiles[x, y] = GenerateSingleTile(tileSize, x, y, tileMat, "Tile");
                }
            }
        }
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y, Material material, string layer, bool walkable = true)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        GameObject graphics = null;
        switch (layer)
        {
            case "Tile":
                graphics = Instantiate(tilePrefab_basic);
                break;
            case "Swamp":
                graphics = Instantiate(tilePrefab_swamp);
                break;
            case "Empty":
                graphics = Instantiate(tilePrefab_hole);
                break;
            default:
                break;
        }
        graphics.transform.SetParent(tileObject.transform, false);
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

        tileObject.layer = LayerMask.NameToLayer(layer);
        nodes[x, y] = new Node(walkable, x, y, layer);

        tileObject.AddComponent<BoxCollider>().size = new Vector3(tileSize, 0.1f, tileSize);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        graphics.transform.localPosition = new Vector3((x + 0.5f) * tileSize, yOffset, (y + 0.5f) * tileSize);
        graphics.transform.localScale = new Vector3(tileSize, tileSize, tileSize);

        return tileObject;
    }

    public void ChangeTileGraphics(int x, int y, string layer, bool walkable)
    {
        if (x < 0 || x >= TILE_COUNT_X || y < 0 || y >= TILE_COUNT_Y) return;
        if (tiles[x, y] == null) return;
        Destroy(tiles[x, y]);
        tiles[x, y] = GenerateSingleTile(tileSize, x, y, tileMat, layer, walkable);
    }

    public void AddTileCount(int x, int y)
    {
        currentHover = -Vector2Int.one;
        TILE_COUNT_X = Mathf.Max(1, TILE_COUNT_X + x);
        TILE_COUNT_Y = Mathf.Max(1, TILE_COUNT_Y + y);
        GameManager.Instance.currentScenario.sizeX = TILE_COUNT_X;
        GameManager.Instance.currentScenario.sizeY = TILE_COUNT_Y;
        //Camera.main.GetComponent<CameraManager>()?.SetupBattleCamera(new(TILE_COUNT_X, TILE_COUNT_Y), tileSize);
    }

    public void RefreshBoard(Unit[,] quickSaveUnits = null, Node[,] quickSaveNodes = null)
    {
        if (activeUnits != null)
        {
            foreach (var unit in activeUnits)
            {
                if (unit == null) continue;
                unit.GetComponent<UnitHealth>().Die();
            }
        }
        foreach (var tile in tiles)
        {
            if (tile == null) continue;
            Destroy(tile.gameObject);
        }

        nodes = null;
        activeUnits = null;
        tiles = null;
        GenerateGrid(GameManager.Instance.currentScenario, quickSaveNodes); 
        //GenerateGrid(tileSize, TILE_COUNT_X, TILE_COUNT_Y);

        if (quickSaveUnits != null)
        {
            SpawnAllUnits(quickSaveUnits);
        }
        else
        {
            SpawnScenarioUnits(GameManager.Instance.currentScenario);
        }

        Camera.main.GetComponent<CameraManager>()?.RefreshCamera(new(TILE_COUNT_X, TILE_COUNT_Y), tileSize);
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
    private bool IsOfType(int x, int y, string type)
    {
        if (x < TILE_COUNT_X && x >= 0 && y < TILE_COUNT_Y && y >= 0) {
            return nodes[x,y].tileTypeLayerName == type;
        }
        return false;
    }
    public bool IsNeighbourOfType(int x, int y, string type)
    {
        return (IsOfType(x-1, y, type) || IsOfType(x+1, y, type) || IsOfType(x, y-1, type) || IsOfType(x, y+1, type));
    }
    public void SpawnUnit(GameObject unit, int team, Vector2Int pos)
    {
        if (activeUnits[pos.x, pos.y] != null)
        {
            var emptyPos = GetFirstFreePos();
            if (emptyPos != errorVector)
                SpawnUnit(unit, team, emptyPos);

            return;
        }
        activeUnits[pos.x, pos.y] = SpawnSingleUnit(unit, team);
    }
    public void SpawnUnit(string unit, int team, Vector2Int pos)
    {
        if (activeUnits[pos.x, pos.y] != null)
        {
            var emptyPos = GetFirstFreePos();
            if (emptyPos != errorVector)
                SpawnUnit(unit, team, emptyPos);

            return;
        }
        activeUnits[pos.x, pos.y] = SpawnSingleUnit(unit, team);
    }
    private void SpawnScenarioUnits(Scenario scenario)
    {
        activeUnits = new Unit[TILE_COUNT_X, TILE_COUNT_Y];
        if (scenario.scenarioUnits == null)
            scenario.scenarioUnits = new List<Scenario.ScenarioUnit>();

        foreach (var unit in scenario.scenarioUnits)
        {
            if (unit.spawnPosX >= 0 && unit.spawnPosX < TILE_COUNT_X)
                if (unit.spawnPosY >= 0 && unit.spawnPosY < TILE_COUNT_Y)
                {
                    //print(unit.unit);
                    //print(GameManager.Instance.UnitSavePaths);
                    var path = GameManager.Instance.UnitSavePaths.GetSavePath(unit.unit);
                    var clone = SpawnSingleUnit(path, unit.team);
                    clone.GetComponent<UnitAbilityManager>().StartAbilities();
                    activeUnits[unit.spawnPosX, unit.spawnPosY] = clone;
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

    Vector2Int errorVector = new Vector2Int(-1, -1);
    private void SpawnPlayerParty()
    {
        List<UnitData> partyUnits = GameManager.Instance.PlayerParty.partyUnits;
        if (partyUnits == null)
            return;

        for (int i = 0; i < partyUnits.Count; i++)
        {
            var path = GameManager.Instance.UnitSavePaths.GetSavePath(partyUnits[i].unitName);
            var clone = SpawnSingleUnit(path, 0);
            clone.team = partyUnits[i].team;
            clone.damage = partyUnits[i].damage;
            clone.magic = partyUnits[i].magic;
            clone.attackSpeed = partyUnits[i].attackSpeed;
            clone.moveSpeed = partyUnits[i].moveSpeed;
            clone.moveInterval = CalculateMoveInterval(partyUnits[i].moveInterval, partyUnits[i].moveSpeed);
            clone.GetComponent<UnitHealth>().SetMaxHp(partyUnits[i].maxHp);
            clone.normalAttacks.Clear();
            foreach (var attack in partyUnits[i].attacks)
            {
                clone.normalAttacks.Add(attack);
            }
            var cloneAbils = clone.GetComponent<UnitAbilityManager>();
            cloneAbils.ability_1 = partyUnits[i].ability1;
            cloneAbils.ability_2 = partyUnits[i].ability2;
            cloneAbils.ability_3 = partyUnits[i].ability3;
            cloneAbils.ability_4 = partyUnits[i].ability4;
            cloneAbils.StartAbilities();

            activeUnits[partyUnits[i].spawnPosX, partyUnits[i].spawnPosY] = clone;
        }
    }
    public float CalculateMoveInterval(float interval, float moveSpeed)
    {
        float r = interval - (interval * moveSpeed * 0.01f * 0.75f); // (moveSpeed * interval) * interval;
        return r;
    }

    public Vector2Int GetFirstFreePos()
    {
        for (int y = 0; y < TILE_COUNT_Y; y++)
            for (int x = 0; x < TILE_COUNT_X; x++)
                if (activeUnits[x, y] == null)
                    return new Vector2Int(x, y);

        return errorVector;
    }
    public Unit SpawnSingleUnit(GameObject _unit, int team)
    {
        Unit unit = Instantiate(_unit, transform).GetComponent<Unit>();
        unit.team = team;
        //AddPlatform(unit);
        return unit;
    }
    public Unit SpawnSingleUnit(string unitPath, int team)
    {
        GameObject _unit = Resources.Load<GameObject>(unitPath);
        Unit unit = Instantiate(_unit, transform).GetComponent<Unit>();
        unit.team = team;
        unit.unitPath = unitPath;
        //AddPlatform(unit);
        return unit;
    }
    public Unit SpawnSingleUnit(UnitData data)
    {
        string path = GameManager.Instance.UnitSavePaths.GetSavePath(data.unitName);
        Unit u = Instantiate(Resources.Load<GameObject>(path), transform).GetComponent<Unit>();
        u.team = data.team;
        u.unitPath = path;
        //AddPlatform(u);
        return u;
        
    }
    private void AddPlatform(Unit unit)
    {
        if (unit.team == 0)
            Instantiate(platform_team0, unit.transform);
        else
            Instantiate(platform_team1, unit.transform);
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
            if (activeUnits[node.x, node.y] == null && node.walkable)
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
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer(nodes[availableMoves[i].x, availableMoves[i].y].tileTypeLayerName);

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
