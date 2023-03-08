using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    public DamageInstance DamageInstance { get; private set; }
    public SaveSlots SaveSlots { get; private set; }
    public MapController MapController { get; private set; }

    public Color hpBarTeam0Color = Color.green;
    public Color hpBarTeam1Color = Color.red;
    [Space(5)]
    public bool spawnMage = true;
    public bool spawnWarrior = true;
    public bool spawnHealer = true;
    public bool spawnRanger = true;
    public GameState state { get; private set; }
    public Scenario currentScenario;
    ScenarioBuilder builder;

    private Chessboard board;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        state = GameState.MAP;
        ParticleSpawner = GetComponentInChildren<ParticleSpawner>();
        DamageInstance = GetComponentInChildren<DamageInstance>();
        HPBars = GetComponentInChildren<HPBarSpawner>();
        SaveSlots = GetComponentInChildren<SaveSlots>();
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

        else if (state == GameState.MAP)
            MapController.MapUpdate();

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

    public void ChangeGamestate(GameState state)
    {
        this.state = state;
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
}
