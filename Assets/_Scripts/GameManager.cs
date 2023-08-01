using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    NONE = 0,
    PRE_BATTLE = 1,
    BATTLE = 2,
    MAP = 3,
    SCENARIO_BUILDER = 4,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public HPBarSpawner HPBars { get; private set; }
    public ParticleSpawner ParticleSpawner { get; private set; }
    public PlayerParty PlayerParty { get; private set; }
    public DamageInstance DamageInstance { get; private set; }
    public SaveSlots SaveSlots { get; private set; }
    public CurrentMap CurrentMap { get; private set; }
    public MapController MapController { get; private set; }
    public SceneManagement SceneManagement { get; private set; }
    public SaveGameManager SaveGameManager { get; private set; }
    public UnitSavePaths UnitSavePaths { get; private set; }
    public ProjectilePools ProjectilePools { get; private set; }

    public Color hpBarTeam0Color = Color.green;
    public Color hpBarTeam1Color = Color.red;
    public Vector3 mapCameraLastPos { get; set; }

    [SerializeField] private GameState startingSceneGameState = GameState.MAP;
    public GameState state { get; private set; }
    public void ChangeGamestate(GameState state)
    {
        this.state = state;
    }

    public Scenario currentScenario;
    public List<MapNode> pathTaken { get; set; }
    ScenarioBuilder builder;
    private Chessboard board;
    private GameObject victoryScreen;
    private GameObject lostScreen;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        state = startingSceneGameState;
        ParticleSpawner = GetComponentInChildren<ParticleSpawner>();
        PlayerParty = GetComponentInChildren<PlayerParty>();
        DamageInstance = GetComponentInChildren<DamageInstance>();
        HPBars = GetComponentInChildren<HPBarSpawner>();
        SaveSlots = GetComponentInChildren<SaveSlots>();
        CurrentMap = GetComponentInChildren<CurrentMap>();
        SceneManagement = GetComponentInChildren<SceneManagement>();
        SaveGameManager = GetComponentInChildren<SaveGameManager>();
        UnitSavePaths = GetComponentInChildren<UnitSavePaths>();
        ProjectilePools = GetComponentInChildren<ProjectilePools>();
        LoadBoardAndMap();
        pathTaken = new List<MapNode>();
    }
    public void LoadBoardAndMap()
    {
        var b = GameObject.Find("Board");
        if (b != null)
            board = b.GetComponent<Chessboard>();
        var m = GameObject.Find("Map");
        if (m != null)
            MapController = m.GetComponent<MapController>();
    }

    void Update()
    {
        if (state == GameState.BATTLE)
            BattleUpdate();

        else if (state == GameState.PRE_BATTLE)
            board.UnitPlacerUpdate();

        //else if (state == GameState.MAP)
            //MapController.MapUpdate();

        else if (state == GameState.SCENARIO_BUILDER)
        {
            if (builder == null)
            {
                builder = ScenarioBuilder.Instance;
            }
            builder.ScenarioBuilderUpdate();
        }
    }

    void BattleUpdate()
    {
        if (board == null)
            if (GameObject.Find("Board") != null)
                board = GameObject.Find("Board").GetComponent<Chessboard>();

        // Unit AI's
        Unit[,] activeUnits = board.GetUnits();
        for (int x = 0; x < activeUnits.GetLength(0); x++)
            for (int y = 0; y < activeUnits.GetLength(1); y++)
                if (activeUnits[x, y] != null)
                    activeUnits[x, y].AI();


        //foreach (var unit in activeUnits)
        //if (unit != null) unit.   

        // CheckHP();
        // Respawn(); (se timeri kun pelaaja kuoli -> spawnaa takas)
        // UpdateTimers(); (statuses etc.)
        //foreach (var unit in activeUnits) 
        //    if (unit != null) 
        //        unit.Move();
        // Attack();
        // Graphics(); (animate, facing, etc.)
        // MoveOtherStuffThanUnits() (bullets etc)
        // Lerps/Tweens(); (joku smoothattu liikutus jossain esim)
        // DealDamages();
        // Explode(); (vaikka bullet osuessaan sein‰‰n)
        // LifeTimers(); (esim bulletin)
        // ClearCollisions();
    }

    public bool IsValidTarget(Unit attacker, Unit target, UnitSearchType search)
    {
        switch (search)
        {
            case UnitSearchType.ENEMIES_ONLY:
                return attacker.team != target.team;
            case UnitSearchType.ALLIES_ONLY:
                return attacker.team == target.team && attacker != target;
            case UnitSearchType.ALLIES_AND_SELF:
                return attacker.team == target.team;
            case UnitSearchType.ONLY_SELF:
                return attacker == target;
            case UnitSearchType.LOWEST_HP_ALLY_PERC:
                return target == Chessboard.Instance.GetLowestTeammate(search, attacker);
            default:
                return true;
        }
    }
    public bool IsValidTarget(int attacker, Unit target, UnitSearchType search)
    {
        if (search == UnitSearchType.ENEMIES_ONLY)
            return attacker != target.team;
        else
            return attacker == target.team;
    }

    public float currentFightCumulatedExperience = 0;
    public void UnitHasDied(Unit unit)
    {
        if (unit.team == 1)
            currentFightCumulatedExperience += unit.experienceWorth;

        bool allUnitsDead = true;
        foreach (var aliveUnit in board.GetUnits())
        {
            if (aliveUnit == null)
            {
                continue;
            }
            if (aliveUnit.team == unit.team && aliveUnit != unit)
            {
                allUnitsDead = false;
                break;
            }
        }
        if (allUnitsDead && state == GameState.BATTLE)
        {
            if (unit.team == 1)
                StartCoroutine(OpenVictoryScreen());
            else
                OpenLossScreen();
            //StartCoroutine("BattleEnd", "MapScene");
        }
    }

    IEnumerator OpenVictoryScreen()
    {
        yield return new WaitForSeconds(1.7f);
        if (victoryScreen == null)
            victoryScreen = GameObject.Find("Canvas").transform.Find("VictoryScreen").gameObject;

        victoryScreen.SetActive(true);
        victoryScreen.GetComponent<VictoryPanel>().InitVictoryScreen();
    }
    void OpenLossScreen()
    {
        if (lostScreen == null)
        {
            lostScreen = GameObject.Find("Canvas").transform.Find("LostScreen").gameObject;
        }
        lostScreen.SetActive(true);
    }


    public IEnumerator BattleEnd(string scene)
    {
        yield return new WaitForEndOfFrame();

        SceneManagement.LoadScene(scene);

        while (SceneManager.GetActiveScene().name != scene)
        {
            yield return null;
        }
        HPBars.Reset();
        ChangeGamestate(GameState.MAP);
    }
    public void ResetPath()
    {
        pathTaken = new List<MapNode>();
    }
}
