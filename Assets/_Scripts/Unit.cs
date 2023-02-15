using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    NONE = 0,
    WORKER = 1,
    MELEE = 2,
    RANGE = 3,
    MAGE = 4,
    SUMMONER = 5,
    COOK = 6,
}

public enum Action
{
    NONE,
    MOVE,
    NORMAL_ATTACK,
    ABILITY,
    ITEM,
}

public class Unit : MonoBehaviour
{
    public float visiblelMoveSpeed = 10;
    public float moveInterval = 1;
    public float percentOfAttackTimerSave = 0.8f;
    public Vector3 attackPositionOffset = Vector3.zero;

    [SerializeField] private List<Unit_NormalAttack> normalAttacks = new List<Unit_NormalAttack>();

    [HideInInspector] public float savedAttackTimerAmount = 0f;
    [HideInInspector] public int team;
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    //public UnitType type;
    [HideInInspector] public Action nextAction = Action.NONE;
    //public bool goingToAttack = false;
    [HideInInspector] public Chessboard board;
    [HideInInspector] public float t = 0;

    // Pathing
    [HideInInspector] public Pathfinding pathfinding;
    [HideInInspector] public Vector2Int target;
    [HideInInspector] public Vector2Int[] path;
    [HideInInspector] public int targetIndex;
    [HideInInspector] public List<Vector2Int> availableMoves = new List<Vector2Int>();
    [HideInInspector] public UnitAbilityManager abilities;

    //private Unit targetUnit;
    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;
    //private Vector2Int targetMoveTile;

    private UnitHealth hp;
    private bool pathPending = false;
    private Unit attackTarget = null;
    private int currentAttack = 0;

    //public virtual void SetTargetMoveTile(Vector2Int tile) { targetMoveTile = tile; }


    private void Start()
    {
        hp = GetComponent<UnitHealth>();
        board = GameObject.Find("Board").GetComponent<Chessboard>();
        pathfinding = board.GetComponent<Pathfinding>();
        ResetPath();
        abilities = GetComponent<UnitAbilityManager>();
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * visiblelMoveSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * visiblelMoveSpeed);
    }

    private UnitAbility nextAbility;
    public void AI()
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
        }
        else if (nextAction != Action.NONE)
        {
            ActivateAction();
        }
        else if (pathPending == false)
        {
            nextAbility = abilities.ConsiderUsingAnAbility();
            pathPending = true;
            ResetPath();

            if (nextAbility == null)
            {
                PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, normalAttacks[currentAttack].targeting, normalAttacks[currentAttack].attackRange, OnPathFound);
                return;
            }
            PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, nextAbility.targetingMode, nextAbility.reach, OnPathFound);
        }
    }


    void ActivateAction()
    {
        switch (nextAction)
        {
            case Action.MOVE:               
                MoveUnit(); 
                break;
            case Action.NORMAL_ATTACK:      
                NormalAttack(); 
                break;
            case Action.ABILITY:
                abilities.ActivateAbility(nextAbility, attackTarget, path); 
                break;
            default: break;
        }
    }

    // Chooses the action
    public void OnPathFound(
        Vector2Int[] newPath, bool pathSuccesfull, bool _inRangeToAttack, Unit targetUnit = null)
    {
        if (hp.hp <= 0)
            return;

        attackTarget = targetUnit;

        if (pathSuccesfull == false)
        {
            pathPending = false;
            return;
        }

        path = newPath;
        if (_inRangeToAttack)
        {
            if (t <= 0)
            {
                if (nextAbility != null)
                {
                    t = nextAbility.castSpeed;
                    nextAction = Action.ABILITY;
                }
                else
                {
                    t = normalAttacks[currentAttack].attackInterval;
                    nextAction = Action.NORMAL_ATTACK;
                }
            }
        }
        else
        {
            t = moveInterval;
            nextAction = Action.MOVE;
        }

        t -= savedAttackTimerAmount;
        savedAttackTimerAmount = 0;
    }

    void MoveUnit()
    {
        if (board.TryMoveUnit(this, path[0]))
        {
            RotateUnit(path[0]);
            if (path.Length == 0)
            {
                return;
            }
        }
        ResetAI();
    }

    void NormalAttack()
    {
        var atk = normalAttacks[currentAttack];
        if (attackTarget == null)
        {
            savedAttackTimerAmount = atk.attackInterval * percentOfAttackTimerSave;
            currentAttack = 0;
            ResetAI();
            return;
        }
        savedAttackTimerAmount = 0;

        var targetPos = path[path.Length - 1];
        RotateUnit(targetPos);
        Vector3 offset = transform.TransformVector(attackPositionOffset);
        Vector3 startPos = transform.position + offset;

        var clone = Instantiate(atk.projectile, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        clone.GetComponent<Projectile>().Init(atk, startPos, path, this, attackTarget);

        currentAttack++;
        if (currentAttack >= normalAttacks.Count)
            currentAttack = 0;

        ResetAI();
    }

    public void ResetAI()
    {
        ResetPath();
        nextAction = Action.NONE;
        pathPending = false;
    }

    public void ResetPath()
    {
        path = null;
    }

    public void RotateUnit(Vector2Int lookAt)
    {
        transform.LookAt(board.GetTileCenter(lookAt.x, lookAt.y));
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref Unit[,] units, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Current pos
        r.Add(new Vector2Int(x, y));

        // Right
        if (x + 1 < tileCountX)
        {
            // Right
            if (units[x + 1, y] == null)
                r.Add(new Vector2Int(x + 1, y));
            else if (units[x + 1, y].team != team)
                r.Add(new Vector2Int(x + 1, y));

            // Top right
            if (y + 1 < tileCountY)
                if (units[x + 1, y + 1] == null)
                    r.Add(new Vector2Int(x + 1, y + 1));
                else if (units[x + 1, y + 1].team != team)
                    r.Add(new Vector2Int(x + 1, y + 1));

            // Bottom right
            if (y - 1 >= 0)
                if (units[x + 1, y - 1] == null)
                    r.Add(new Vector2Int(x + 1, y - 1));
                else if (units[x + 1, y - 1].team != team)
                    r.Add(new Vector2Int(x + 1, y - 1));
        }
        // Left
        if (x - 1 >= 0)
        {
            // Left
            if (units[x - 1, y] == null)
                r.Add(new Vector2Int(x - 1, y));
            else if (units[x - 1, y].team != team)
                r.Add(new Vector2Int(x - 1, y));

            // Top left
            if (y + 1 < tileCountY)
                if (units[x - 1, y + 1] == null)
                    r.Add(new Vector2Int(x - 1, y + 1));
                else if (units[x - 1, y + 1].team != team)
                    r.Add(new Vector2Int(x - 1, y + 1));

            // Bottom left
            if (y - 1 >= 0)
                if (units[x - 1, y - 1] == null)
                    r.Add(new Vector2Int(x - 1, y - 1));
                else if (units[x - 1, y - 1].team != team)
                    r.Add(new Vector2Int(x - 1, y - 1));
        }
        // Up
        if (y + 1 < tileCountY)
            if (units[x, y + 1] == null || units[x, y + 1].team != team)
                r.Add(new Vector2Int(x, y + 1));
        // Down
        if (y - 1 >= 0)
            if (units[x, y - 1] == null || units[x, y - 1].team != team)
                r.Add(new Vector2Int(x, y - 1));

        return r;
    }
    public virtual void SetPosition(Vector3 pos, bool force = false)
    {
        desiredPosition = pos;
        if (force)
            transform.position = desiredPosition;
    }
    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
            transform.localScale = desiredScale;
    }
}
