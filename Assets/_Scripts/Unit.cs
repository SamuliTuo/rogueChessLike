using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using System.Linq;

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
    NORMAL_ATTACK_SECONDHALF,
    ABILITY,
    ABILITY_SECONDHALF,
    ITEM,
}
public enum UnitTextEncounterCheckableStats
{
    DAMAGE, MAGIC, ATTACK_SPEED, MOVE_SPEED, HEALTH
}

public class Unit : MonoBehaviour
{
    public string unitPath { get; set; }
    public bool isObstacle = false;

    public float damage = 10;
    public float magic = 10;
    public float attackSpeed = 1;
    public float moveSpeed = 1;
    public float turnRate = 1;
    public float critChance = 0;
    public float critDamagePerc = 2;
    public float missChance = 0;
    public float lifeSteal_flat = 0;
    public float lifeSteal_perc = 0;

    public float experienceWorth = 10;
    public float visibleMoveSpeed = 10;
    public float moveInterval = 1;
    public float percentOfAttackTimerSave = 0.8f;
    public Vector3 attackPositionOffset = Vector3.zero;

    public List<Unit_NormalAttack> normalAttacks = new List<Unit_NormalAttack>();
    public bool randomizeAttackOrder = false;
    bool canBeSameAttackTwiceInRow = true;

    [HideInInspector] public float savedAttackTimerAmount = 0f;

    //player team = 0, enemy team = 1, obstacle = 2
    public int team;
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    //public UnitType type;
    [HideInInspector] public Action nextAction = Action.NONE;
    //public bool goingToAttack = false;
    [HideInInspector] public Chessboard board;
    [HideInInspector] public float t = 0;
    [HideInInspector] public float tStun = 0;
    [HideInInspector] public int spawnRotation = 0;

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
        t = moveInterval + Random.Range(0, 0.5f);
        hp = GetComponent<UnitHealth>();
        board = GameObject.Find("Board").GetComponent<Chessboard>();
        pathfinding = board.GetComponent<Pathfinding>();
        ResetPath();
        abilities = GetComponent<UnitAbilityManager>();
        animator = GetComponentInChildren<Animator>();
    }
    
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * visibleMoveSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * visibleMoveSpeed);
    }

    private Animator animator;
    private UnitAbility nextAbility;
    public void AI()
    {
        if (isObstacle || hp.dying)
            return;
        
        // Is stunned
        if (tStun > 0) 
        {
            tStun -= Time.deltaTime;
            if (tStun < 0)
                GameManager.Instance.ParticleSpawner.StopStun(this);
        }
        // Unit activated an action and needs to wait:
        else if (t > 0)  
        { 
            t -= Time.deltaTime; 
        }
        // Unit chose and action to activate:
        else if (nextAction != Action.NONE) 
        {
            ActivateAction();
        }


        // ...otherwise choose what to do next:
        else if (pathPending == false)
        {
            nextAbility = abilities.ConsiderUsingAnAbility();
            pathPending = true;
            ResetPath();

            // Auto-attack / move to attack range:
            if (nextAbility == null)
            {
                PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, normalAttacks[currentAttack].targeting, normalAttacks[currentAttack].attackRange, OnPathFound);
            }

            // Use an ability
            //  - on ally:
            else if (nextAbility.targetSearchType == UnitSearchType.LOWEST_HP_ALLY_ABS || nextAbility.targetSearchType == UnitSearchType.LOWEST_HP_ALLY_PERC)
            {
                var targetUnit = board.GetLowestTeammate(nextAbility.targetSearchType, this);
                if (targetUnit != null)
                {
                    PathRequestManager.RequestFindUnit(new Vector2Int(x, y), this, targetUnit, nextAbility.targetSearchType, nextAbility.reach, OnPathFound);
                    return;
                }
                nextAbility = null;
                PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, normalAttacks[currentAttack].targeting, normalAttacks[currentAttack].attackRange, OnPathFound);
            }
            //  - on self:
            else if (nextAbility.targetSearchType == UnitSearchType.ONLY_SELF)
            {
                nextAction = Action.ABILITY;
                path = new Vector2Int[1];
                path[0] = new Vector2Int(x, y);
                attackTarget = this;
            }
            //  - on enemy:
            else
            {
                PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, nextAbility.targetSearchType, nextAbility.reach, OnPathFound);
            }
        }
    }
        
    //Load unit from PlayerParty:
    public void LoadUnit(UnitData data)
    {
        team = data.team;
        hp.SetMaxHp(data.maxHp);
        damage = data.damage;
        magic = data.magic;
        attackSpeed = data.attackSpeed;
        moveSpeed = data.moveSpeed;
        moveInterval = data.moveInterval;
        //moveInterval = GameManager.Instance.GetMoveIntervalFromMoveSpeed(data.moveSpeed);

        var pos = new Vector2Int(data.spawnPosX, data.spawnPosY);
        if (board.GetUnits()[data.spawnPosX, data.spawnPosY] != null)
        {
            pos = board.GetFirstFreePos();
        }
        SetPosition(board.GetTileCenter(pos.x, pos.y), true);
        normalAttacks = data.attacks;
        abilities.ability_1 = data.ability1;
        abilities.ability_2 = data.ability2;
        abilities.ability_3 = data.ability3;
        abilities.ability_4 = data.ability4;
        ResetPath();
        ResetAI();
    }


    
    void ActivateAction()
    {
        switch (nextAction)
        {
            // Moving
            case Action.MOVE:
                MoveUnit(); 
                break;

            //Attacking
            case Action.NORMAL_ATTACK:
                if (attackTarget == null)
                {
                    ResetAI();
                    break;
                }
                NormalAttack(); 
                break;
            case Action.NORMAL_ATTACK_SECONDHALF:
                if (attackTarget == null)
                {
                    ResetAI();
                    break;
                }
                NormalAttackSecondHalf();
                break;

            //Abilities
            case Action.ABILITY:
                if (attackTarget == null)
                {
                    ResetAI();
                    break;
                }
                abilities.ActivateAbility(nextAbility, attackTarget, path);
                break;
            case Action.ABILITY_SECONDHALF:
                if (attackTarget == null)
                {
                    ResetAI();
                    break;
                }
                abilities.ActivateAbilitySecondHalf(nextAbility, attackTarget, path);
                break;

            default: 
                break;
        }
    }
    

    // Chooses the action
    public void OnPathFound(Vector2Int[] newPath, bool pathSuccesfull, bool _inRangeToAttack, Unit targetUnit = null)
    {
        if (hp.hp <= 0)
            return;

        attackTarget = targetUnit;

        if (pathSuccesfull == false)
        {
            pathPending = false;
            t = 1;
            return;
        }

        path = newPath;
        if (_inRangeToAttack)
        {
            if (t <= 0)
            {
                if (nextAbility != null)
                {
                    nextAction = Action.ABILITY;
                }
                else
                {
                    nextAction = Action.NORMAL_ATTACK;
                }
            }
        }
        else
        {
            nextAction = Action.MOVE;
        }

        t -= savedAttackTimerAmount;
        savedAttackTimerAmount = 0;
    }

    public void GetStunned(float stunDuration)
    {
        print("pls lisää mulle stun-animaatio tai jotain, stunin kesto: " + stunDuration);
        animator?.Play("stun", 0, 0);
        GameManager.Instance.ParticleSpawner.SpawnStun(this);
        ResetAI();
        tStun = stunDuration;
        //t = 0;
    }

    public void GetNudged(bool isChip, Vector3 _nudgeDir)
    {
        Vector3 lookTarget = transform.position + new Vector3(_nudgeDir.x, 0, _nudgeDir.z);// board.GetTileCenter(lookAt.x, lookAt.y);
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }
        rotateCoroutine = StartCoroutine(UnitRotationCoroutine(lookTarget));

        //RotateUnit(forward);
        if (isChip)
        {
            animator?.Play("move1",0,0);
        }
        else
        animator?.Play("attack1", 0, 0);
    }

    void MoveUnit()
    {
        RotateUnit(path[0]);

        if (board.TryMoveUnit(this, path[0]))
        {

            ///
            //////////////////////////WWWWWWWWWWWWWWWW
            //Randomize animation (placeholder)
            if (animator != null)
            {
                animator.speed = 1;
                var rand = Random.Range(0, 100);
                if (rand > 95)
                    animator.Play("move5", 0, 0);
                else if (rand > 87)
                    animator.Play("move2", 0, 0);
                else if (rand > 73)
                    animator.Play("move1", 0, 0);
                else if (rand > 62)
                    animator.Play("move0", 0, 0);
                else if (rand > 51)
                    animator.Play("move3", 0, 0);
                else
                    animator.Play("move4", 0, 0);
            }
            //////////////////////////WWWWWWWWWWWWWWWWW
            ///

            if (path.Length == 0)
            {
                return;
            }
        }
        t = 450 / moveSpeed;
        t += pathfinding.AddTerrainEffects(Chessboard.Instance.nodes[x,y]);
        t += Random.Range(0.0f, 0.2f);
        //t = moveInterval + pathfinding.AddTerrainEffects(Chessboard.Instance.nodes[x, y]) + Random.Range(0.0f, 0.15f);
        ResetAI();
    }

    float AttackSpeedMultiplier()
    {
        return Mathf.Min(0.80f, attackSpeed * 0.01f);
    }

    void NormalAttack()
    {
        t = normalAttacks[currentAttack].attackDuration_firstHalf * (1.0f - AttackSpeedMultiplier());
        if (attackTarget == null)
        {
            savedAttackTimerAmount = normalAttacks[currentAttack].attackDuration_firstHalf * percentOfAttackTimerSave;
            currentAttack = 0;
            ResetAI();
            return;
        }
        savedAttackTimerAmount = 0;

        Vector2Int targetPos = new(target.x, target.y);// path[path.Length - 1];
        RotateUnit(new Vector2Int(attackTarget.x, attackTarget.y));
        if (animator != null)
        {
            animator.speed = 1 + AttackSpeedMultiplier();
            animator?.Play("attack", 0, 0);
        }
        nextAction = Action.NORMAL_ATTACK_SECONDHALF;
    }

    void NormalAttackSecondHalf()
    {
        Vector3 offset = transform.TransformVector(attackPositionOffset);
        Vector3 startPos = transform.position + offset;
        var projectile = GameManager.Instance.ProjectilePools.SpawnProjectile(
            normalAttacks[currentAttack].projectilePath, startPos, Quaternion.identity);
        projectile?.GetComponent<Projectile>().Init(
            normalAttacks[currentAttack], startPos, path, normalAttacks[currentAttack].bounceCount_atk,
            normalAttacks[currentAttack].bounceCount_ability, critChance, critDamagePerc, missChance, this, attackTarget);

        t = normalAttacks[currentAttack].attackDuration_secondHalf * (1.0f - AttackSpeedMultiplier());
        ResetAI();

        // Choosing the next attack
        if (randomizeAttackOrder && normalAttacks.Count > 1)
        {
            if (canBeSameAttackTwiceInRow)
            {
                currentAttack = Random.Range(0, normalAttacks.Count);
            }
            else
            {
                int lastAtt = currentAttack;
                for (int i = 0; i < 10; i++)
                {
                    var r = Random.Range(0, normalAttacks.Count);
                    if (r != lastAtt)
                    {
                        currentAttack = r;
                        break;
                    }
                }
            }
        }
        else
        {
            currentAttack++;
            if (currentAttack >= normalAttacks.Count)
                currentAttack = 0;
        }
    }

    public void ResetAI()
    {
        ResetPath();
        nextAction = Action.NONE;
        nextAbility = null;
        pathPending = false;
    }
    public void ResetPath()
    {
        path = null;
    }

    Coroutine rotateCoroutine = null;
    public void RotateUnit(Vector2Int lookAt)
    {
        Vector3 lookTarget = board.GetTileCenter(lookAt.x, lookAt.y);
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }
        rotateCoroutine = StartCoroutine(UnitRotationCoroutine(lookTarget));
        //transform.LookAt(board.GetTileCenter(lookAt.x, lookAt.y));
    }
    private IEnumerator UnitRotationCoroutine(Vector3 lookTarget)
    {
        Quaternion lookRotation = Quaternion.LookRotation(lookTarget - transform.position);
        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * turnRate;
            yield return null;
        }
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

    public float GetDamage() { return damage; }
    public float GetMagic() { return magic; }
}
