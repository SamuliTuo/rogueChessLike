using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [HideInInspector] public UnitInLibrary libraryData = null;
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

    public List<Tuple<Unit_NormalAttack, GameObject>> normalAttacks = new List<Tuple<Unit_NormalAttack, GameObject>>();
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
    private int currentAttackIndex = 0;

    //public virtual void SetTargetMoveTile(Vector2Int tile) { targetMoveTile = tile; }

    private void Start()
    {
        t = moveSpeed + UnityEngine.Random.Range(0, 1f);
        hp = GetComponent<UnitHealth>();
        board = GameObject.Find("Board").GetComponent<Chessboard>();
        pathfinding = board.GetComponent<Pathfinding>();
        ResetPath();
        abilities = GetComponent<UnitAbilityManager>();
        animator = GetComponentInChildren<Animator>();
        damage = team == 0 ? damage * DebugTools.Instance.playerDamagePercentage : damage;
        magic = team == 0 ? magic * DebugTools.Instance.playerDamagePercentage : magic;
    }
    
    private void Update()
    {
        if (isPushed)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * visibleMoveSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * visibleMoveSpeed);
    }

    private Animator animator;
    private Tuple<UnitAbility, int> nextAbility;
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

            //  A T T A C K  / move to attack range:
            if (nextAbility == null && normalAttacks.Count > 0)
            {
                if (normalAttacks.Count == 0)
                {
                    Debug.Log("!!!!!!!! Unit " + this.name + " has no attacks !!!!!!");
                    return;
                }
                // Check if still in range of target:
                if (attackTarget != null)
                {
                    if (attackTarget.team != team && pathfinding.IsSpecificUnitInRangeFromNode(attackTarget, x, y, normalAttacks[currentAttackIndex].Item1.attackRange))
                    {
                        nextAction = Action.NORMAL_ATTACK;
                    }
                    else
                    {
                        PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, normalAttacks[currentAttackIndex].Item1.targeting, normalAttacks[currentAttackIndex].Item1.attackRange, OnPathFound);
                    }
                }
                else
                {
                    PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, normalAttacks[currentAttackIndex].Item1.targeting, normalAttacks[currentAttackIndex].Item1.attackRange, OnPathFound);
                }
            }

            //  A B I L I T Y :
            //  - on ally:
            else if (nextAbility.Item1.targetSearchType == UnitSearchType.LOWEST_HP_ALLY_ABS || nextAbility.Item1.targetSearchType == UnitSearchType.LOWEST_HP_ALLY_PERC)
            {
                var targetUnit = board.GetLowestTeammate(nextAbility.Item1.targetSearchType, this);
                if (targetUnit != null)
                {
                    PathRequestManager.RequestFindUnit(new Vector2Int(x, y), this, targetUnit, nextAbility.Item1.targetSearchType, nextAbility.Item1.reach, OnPathFound);
                    return;
                }
                nextAbility = null;
                PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, normalAttacks[currentAttackIndex].Item1.targeting, normalAttacks[currentAttackIndex].Item1.attackRange, OnPathFound);
            }
            //  - on self:
            else if (nextAbility.Item1.targetSearchType == UnitSearchType.ONLY_SELF)
            {
                nextAction = Action.ABILITY;
                path = new Vector2Int[1];
                path[0] = new Vector2Int(x, y);
                attackTarget = this;
            }
            //  - on enemy:
            else
            {
                if (attackTarget != null)
                {
                    if (attackTarget.team != team && pathfinding.IsSpecificUnitInRangeFromNode(attackTarget, x, y, nextAbility.Item1.reach))
                    {
                        nextAction = Action.ABILITY;
                    }
                    else
                    {
                        PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, nextAbility.Item1.targetSearchType, nextAbility.Item1.reach, OnPathFound);
                    }
                }
                else
                {
                    PathRequestManager.RequestFindClosestEnemy(new Vector2Int(x, y), this, nextAbility.Item1.targetSearchType, nextAbility.Item1.reach, OnPathFound);
                }
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

        normalAttacks.Clear();
        for (int i = 0; i < data.attacks.Count; i++)
        {
            normalAttacks.Add(new (data.attacks[i], libraryData.attacks[i].projectile));
            print(data.attacks[i]+",         "+ libraryData.attacks[i].projectile);
        }

        abilities.abilities.Clear();
        abilities.abilities[0] = data.ability1;
        abilities.abilities[1] = data.ability2;
        abilities.abilities[2] = data.ability3;
        ResetPath();
        ResetAI();
    }

    
    void ActivateAction()
    {
        switch (nextAction)
        {
            case Action.MOVE:
                MoveUnit(); 
                break;

            // Attacking
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

            // Abilities
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

    public void GetStunned(float stunDuration, bool animation = true)
    {
        //print("pls lisää mulle stun-animaatio tai jotain, stunin kesto: " + stunDuration);
        if (animation)
        {
            animator?.Play("stun", 0, 0);
        }
        GameManager.Instance.ParticleSpawner.SpawnStun(this);
        ResetAI();
        tStun = stunDuration;
        //t = 0;
    }
    public void EndStun()
    {
        tStun = 0;
    }

    public void GetNudged(bool isChip, Vector3 _nudgeDir)
    {
        //Vector3 lookTarget = transform.position + new Vector3(_nudgeDir.x, 0, _nudgeDir.z);// board.GetTileCenter(lookAt.x, lookAt.y);
        //if (rotateCoroutine != null)
        //{
        //    StopCoroutine(rotateCoroutine);
        //}
        //rotateCoroutine = StartCoroutine(UnitRotationCoroutine(lookTarget));

        //RotateUnit(forward);
        if (isChip)
        {
            animator?.Play("move0",0,0);
        }
        else
        {
            animator?.Play("stun", 0, 0);
        }
    }

    [SerializeField] private float pushMaxSpeed = 10;
    private bool isPushed = false;
    public void GetPushed(int targetX, int targetY, float magnitude, bool chip, bool dying)
    {
        isPushed = true;
        StartCoroutine(PushedCoroutine(targetX, targetY, magnitude, chip, dying));
    }

    private float nudgeSpeed = 13;
    private float chipSpeed = 9;
    private float fallSpeed = 3;
    private float bezierHandleHeight = 2;
    IEnumerator PushedCoroutine(int targetX, int targetY, float magnitude, bool chip, bool dying)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = board.GetTileCenter(targetX, targetY);
        float timer = 0;
        if (chip)
        {   // chip
            Vector3 p2 = Vector3.Lerp(startPos + Vector3.up * bezierHandleHeight, endPos + Vector3.up * bezierHandleHeight, 0.2f);
            Vector3 p3 = Vector3.Lerp(startPos + Vector3.up * bezierHandleHeight, endPos + Vector3.up * bezierHandleHeight, 0.8f);

            while (timer < magnitude)
            {
                float perc = timer / magnitude;
                perc = Mathf.Sin(perc * Mathf.PI * 0.5f);
                transform.position = HelperUtilities.CalculateCubicBezierPoint(perc, startPos, p2, p3, endPos);
                timer += Time.deltaTime * chipSpeed;
                yield return null;
            }
        }
        else
        {   // nudge
            while (timer < magnitude)
            {
                float perc = timer / magnitude;
                perc = Mathf.Sin(perc * Mathf.PI * 0.5f);
                transform.position = Vector3.Lerp(startPos, endPos, perc);
                timer += Time.deltaTime * nudgeSpeed;
                yield return null;
            }
        }
        
        if (dying)
        {
            StartCoroutine(FallCoroutine());
        }
        else
        {
            isPushed = false;
        }
    }

    IEnumerator FallCoroutine()
    {
        var startpos = transform.position;
        var endpos = transform.position + Vector3.down * 3;
        float timer = 0;
        while (timer < 1)
        {
            float perc = 1f - Mathf.Cos(timer * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startpos, endpos, perc);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.1f, perc);
            timer += Time.deltaTime * fallSpeed;
            yield return null;
        }
        hp.RemoveHP(9999999, true);
    }

    void MoveUnit()
    {
        RotateUnit(path[0]);

        if (board.TryMoveUnit(this, path[0]))
        {
            if (team == 0) GameManager.Instance.ParticleSpawner.SpawnParticles(ParticleType.JUMP_CLOUDS, transform.position, transform.forward);

            ///
            //////////////////////////WWWWWWWWWWWWWWWW
            //Randomize animation (placeholder)
            //print("fix move animations here!");
            if (animator != null)
            {
                animator.speed = 1;
                var rand = UnityEngine.Random.Range(0, 100);
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
        t = moveSpeed;
        t += pathfinding.AddTerrainEffects(Chessboard.Instance.nodes[x,y]);
        t += UnityEngine.Random.Range(0.0f, 0.2f);
        //t = moveInterval + pathfinding.AddTerrainEffects(Chessboard.Instance.nodes[x, y]) + Random.Range(0.0f, 0.15f);
        ResetAI();
    }

    float AttackSpeedMultiplier()
    {
        return Mathf.Min(0.80f, attackSpeed * 0.01f);
    }

    void NormalAttack()
    {
        t = normalAttacks[currentAttackIndex].Item1.attackDuration_firstHalf * (1.0f - AttackSpeedMultiplier());
        if (attackTarget == null)
        {
            savedAttackTimerAmount = normalAttacks[currentAttackIndex].Item1.attackDuration_firstHalf * percentOfAttackTimerSave;
            currentAttackIndex = 0;
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
        float _damage = GetAttackDmg();

        var projectile = GameManager.Instance.ProjectilePools.SpawnProjectile(
            normalAttacks[currentAttackIndex].Item2, startPos, Quaternion.identity);
        projectile?.GetComponent<Projectile>().Init(
            normalAttacks[currentAttackIndex].Item1, startPos, path, normalAttacks[currentAttackIndex].Item1.bounceCount_atk,
            normalAttacks[currentAttackIndex].Item1.bounceCount_ability, _damage, critChance, critDamagePerc, missChance, this, attackTarget);

        t = normalAttacks[currentAttackIndex].Item1.attackDuration_secondHalf * (1.0f - AttackSpeedMultiplier());
        ResetAI();

        // Choosing the next attack
        if (randomizeAttackOrder && normalAttacks.Count > 1)
        {
            if (canBeSameAttackTwiceInRow)
            {
                currentAttackIndex = UnityEngine.Random.Range(0, normalAttacks.Count);
            }
            else
            {
                int lastAtt = currentAttackIndex;
                for (int i = 0; i < 10; i++)
                {
                    var r = UnityEngine.Random.Range(0, normalAttacks.Count);
                    if (r != lastAtt)
                    {
                        currentAttackIndex = r;
                        break;
                    }
                }
            }
        }
        else
        {
            currentAttackIndex++;
            if (currentAttackIndex >= normalAttacks.Count)
                currentAttackIndex = 0;
        }
    }

    float GetAttackDmg()
    {
        if (!normalAttacks[currentAttackIndex].Item1.usesMagic)
            return GetDamage() * normalAttacks[currentAttackIndex].Item1.damage;
        else
            return GetMagic() * normalAttacks[currentAttackIndex].Item1.damage;
        
    }
    public float GetAbilityDmg(UnitAbility _ability)
    {
        if (_ability.usesMagic)
            return GetMagic() * _ability.damage;
        else
            return GetDamage() * _ability.damage;
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

    public virtual List<Vector2Int> GetAvailableMoves(ref Unit[,] _units, int _tileCountX, int _tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Current pos
        r.Add(new Vector2Int(x, y));

        // Right
        if (x + 1 < _tileCountX)
        {
            // Right
            if (_units[x + 1, y] == null)
                r.Add(new Vector2Int(x + 1, y));
            else if (_units[x + 1, y].team != team)
                r.Add(new Vector2Int(x + 1, y));

            // Top right
            if (y + 1 < _tileCountY)
                if (_units[x + 1, y + 1] == null)
                    r.Add(new Vector2Int(x + 1, y + 1));
                else if (_units[x + 1, y + 1].team != team)
                    r.Add(new Vector2Int(x + 1, y + 1));

            // Bottom right
            if (y - 1 >= 0)
                if (_units[x + 1, y - 1] == null)
                    r.Add(new Vector2Int(x + 1, y - 1));
                else if (_units[x + 1, y - 1].team != team)
                    r.Add(new Vector2Int(x + 1, y - 1));
        }
        // Left
        if (x - 1 >= 0)
        {
            // Left
            if (_units[x - 1, y] == null)
                r.Add(new Vector2Int(x - 1, y));
            else if (_units[x - 1, y].team != team)
                r.Add(new Vector2Int(x - 1, y));

            // Top left
            if (y + 1 < _tileCountY)
                if (_units[x - 1, y + 1] == null)
                    r.Add(new Vector2Int(x - 1, y + 1));
                else if (_units[x - 1, y + 1].team != team)
                    r.Add(new Vector2Int(x - 1, y + 1));

            // Bottom left
            if (y - 1 >= 0)
                if (_units[x - 1, y - 1] == null)
                    r.Add(new Vector2Int(x - 1, y - 1));
                else if (_units[x - 1, y - 1].team != team)
                    r.Add(new Vector2Int(x - 1, y - 1));
        }
        // Up
        if (y + 1 < _tileCountY)
            if (_units[x, y + 1] == null || _units[x, y + 1].team != team)
                r.Add(new Vector2Int(x, y + 1));
        // Down
        if (y - 1 >= 0)
            if (_units[x, y - 1] == null || _units[x, y - 1].team != team)
                r.Add(new Vector2Int(x, y - 1));
        
        return r;
    }
    public virtual void SetPosition(Vector3 _pos, bool _force = false)
    {
        desiredPosition = _pos;
        if (_force)
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
