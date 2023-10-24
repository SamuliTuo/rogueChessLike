using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class AbilityInstance : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    Node targetNode;
    int bouncesRemainingAttack;
    int bouncesRemainingAbility;
    float t;
    float dist;
    float forcedTimer;
    Unit targetUnit;
    Unit shooter;
    bool followUnit = false;
    UnitAbility ability;
    Vector2Int[] path;
    List<Unit> bouncedOn;

    private IObjectPool<GameObject> pool;
    public void SetPool(IObjectPool<GameObject> pool) => this.pool = pool;

    //clone.GetComponent<AbilityInstance>().Init(_ability, startPos, unit.team, _path, _attackTarget);
    public void Init(  //, Vector3 _startPos, Vector3 _endPos, Node _targetNode, float _flySpeed, float _damage, List<int> damagesTeams, float forcedTimer = 0f)
        UnitAbility _ability,
        Vector3 _startPos, 
        Vector2Int[] _path,
        int _attackBounces,
        int _abilityBounces,
        Unit _shooter,
        Unit _targetUnit = null,
        List<Unit> _bouncedOn = null)
    {
        path = _path;
        ability = _ability;
        startPos = _startPos;
        targetUnit = _targetUnit;
        shooter = _shooter;
        forcedTimer = ability.minLifeTime;
        bouncesRemainingAttack = _attackBounces;
        bouncesRemainingAbility = _abilityBounces;

        bouncedOn = _bouncedOn;
        if (ability.onlyOneBouncePerUnit == false)
            bouncedOn = null;

        if (targetUnit != null)
        {
            followUnit = true;
            dist = (startPos - targetUnit.transform.position).magnitude;
        }
        else if (path != null)
        {
            endPos = Chessboard.Instance.GetTileCenter(_path[_path.Length-1].x, _path[_path.Length-1].y) + Vector3.up * 1.5f;
            targetNode = Chessboard.Instance.nodes[_path[_path.Length - 1].x, _path[_path.Length - 1].y];
            followUnit = false;
            dist = (startPos - endPos).magnitude;
        }
        else
        {
            Deactivate();
            return;
        }


        if (ability.centerOnYourself)
        {
            startPos = shooter.transform.position;
            endPos = shooter.transform.position;
        }
        
        t = 0;

        if (ability.projectileType == ProjectileType.RANGED)
        {
            StartCoroutine("ProjectileMotion");
        }
        else if (ability.projectileType == ProjectileType.MELEE)
        {
            StartCoroutine("ProjectileMelee");
        }
    }


    IEnumerator ProjectileMotion()
    {
        // Fly
        while (t < dist || forcedTimer > 0)
        {
            forcedTimer -= Time.deltaTime;
            t += Time.deltaTime * ability.flySpeed;
            if (followUnit && targetUnit != null)
                endPos = targetUnit.transform.position + Vector3.up * 1.5f;

            transform.LookAt(endPos);
            var perc = t / dist;
            transform.position = Vector3.Lerp(startPos, endPos, perc);
            yield return null;
        }

        // Hit target
        if (ability.centerOnYourself)
            targetNode = Chessboard.Instance.nodes[shooter.x, shooter.y];

        else if (followUnit)
            targetNode = Chessboard.Instance.nodes[targetUnit.x, targetUnit.y];

        float damage = shooter.GetMagic() * ability.damage;
        GameManager.Instance.DamageInstance.Activate(targetNode, damage, shooter, ability.validTargets, ability.dmgInstanceType, ability.directHitStatusModifier);
        GameManager.Instance.ParticleSpawner.SpawnParticles(ability.hitParticle, transform.position);

        SpawnAreaDOT();
        SpawnUnits();
        Bounces();
        Deactivate();
    }


    IEnumerator ProjectileMelee()
    {
        var lookAtPos = Chessboard.Instance.GetTileCenter(targetUnit.x, targetUnit.y);
        transform.LookAt(new Vector3(lookAtPos.x, transform.position.y, lookAtPos.z));
        targetNode = Chessboard.Instance.nodes[targetUnit.x, targetUnit.y];
        if (!ability.centerOnYourself)
        {
            transform.position = Chessboard.Instance.GetTileCenter(targetUnit.x, targetUnit.y);
            targetNode = Chessboard.Instance.nodes[targetUnit.x, targetUnit.y];
        }
        else
        {
            transform.position = endPos;
            targetNode = Chessboard.Instance.nodes[shooter.x,shooter.y];
        }
        
        // Hit target
        float damage = shooter.GetMagic() * ability.damage;
        GameManager.Instance.DamageInstance.Activate(targetNode, damage, shooter, ability.validTargets, ability.dmgInstanceType, ability.directHitStatusModifier, ability.hitParticle);
        GameManager.Instance.ParticleSpawner.SpawnParticles(ability.hitParticle, transform.position);
        
        // Stay visible
        while (forcedTimer > 0)
        {
            forcedTimer -= Time.deltaTime;
            yield return null;
        }

        SpawnAreaDOT();
        SpawnUnits();
        Bounces();
        Deactivate();
    }


    void Bounces()
    {
        if (bouncesRemainingAbility > 0 || bouncesRemainingAttack > 0)
        {
            AddTargetToBounceList();
            SpawnBounceAttacks();
            SpawnBounceAbilities();
        }
    }

    void SpawnBounceAttacks()
    {
        if (bouncesRemainingAttack < 1)
            return;

        int i = 0;
        List<BOUNCETARGET> targets = FindTargetsForBounces(ability.bounceAttack_targeting);
        foreach (BOUNCETARGET target in targets)
        {
            if (i >= ability.bounceSpawnCount_atk)
                break;

            if (target.distance >= ability.bounceRange_attack)
                continue;

            i++;
            var projectile = GameManager.Instance.ProjectilePools.SpawnProjectile(
                ability.bounceAttack.projectilePath, transform.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Init(
                ability.bounceAttack, 
                transform.position, 
                null, 
                bouncesRemainingAttack - 1, 
                bouncesRemainingAbility - 1,
                0,
                0,
                0,
                shooter, 
                target.unit,
                bouncedOn);
        }


        bouncesRemainingAttack--;
    }
    void SpawnBounceAbilities()
    {
        if (bouncesRemainingAbility < 1)
            return;

        int i = 0;
        List<BOUNCETARGET> targets = FindTargetsForBounces(ability.bounceAbility_targeting);
        foreach (BOUNCETARGET target in targets)
        {
            if (i >= ability.bounceSpawnCount_ability)
                break;
            if (target.distance >= ability.bounceRange_ability)
                continue;

            i++;
            var projectile = GameManager.Instance.ProjectilePools.SpawnProjectile(ability.bounceAbility.projectilePath, transform.position, Quaternion.identity);
            projectile.GetComponent<AbilityInstance>().Init(
                ability.bounceAbility,
                transform.position,
                null,
                bouncesRemainingAttack - 1,
                bouncesRemainingAbility - 1,
                shooter,
                target.unit,
                bouncedOn);
        }

        bouncesRemainingAttack--;
    }

    void AddTargetToBounceList()
    {
        if (bouncedOn == null)
            bouncedOn = new List<Unit>();

        if (targetUnit != null)
            bouncedOn.Add(targetUnit);
    }

    void SpawnAreaDOT()
    {
        if (ability.spawnAreaDOT == false)
            return;

        float tickDamage = shooter.GetMagic() * ability.tickDamage;
        GameManager.Instance.DamageInstance.ActivateAreaDOT(targetNode, tickDamage, ability.tickIntervalSeconds, ability.intervalCount, shooter, ability.areaDOTValidTargets, ability.dmgInstanceType, ability.areaDOTStatusModifier, ability.hitParticle);
    }
    
    void SpawnUnits()
    {
        if (ability.spawnUnit.Length > 0)
        {
            var freeNodes = Extensions.GetFreeNodesAtAndAround(targetNode.x, targetNode.y);
            for (int i = 0; i < ability.spawnCount; i++)
            {
                if (freeNodes.Count > 0)
                {
                    var rand = Random.Range(0, freeNodes.Count);
                    var node = freeNodes[rand];
                    freeNodes.RemoveAt(rand);
                    var path = GameManager.Instance.UnitSavePaths.GetSavePath(ability.spawnUnit);
                    Chessboard.Instance.SpawnUnit(path, shooter.team, node);
                    Chessboard.Instance.PositionSingleUnit(node.x, node.y, true);
                }
                else
                    break;
            }
        }
    }

    public class BOUNCETARGET
    {
        public Unit unit { get; set; }
        public float distance { get; set; }
    }

    List<BOUNCETARGET> FindTargetsForBounces(UnitSearchType targeting)
    {

        List<BOUNCETARGET> targets = new List<BOUNCETARGET>();

        Unit[,] activeUnits = Chessboard.Instance.GetUnits();

        // Targeting allies
        if (targeting == UnitSearchType.LOWEST_HP_ALLY_PERC || targeting == UnitSearchType.LOWEST_HP_ALLY_ABS)
        {
            var u = Chessboard.Instance.GetLowestTeammate(targeting, targetUnit, ability.bounceRange_ability, false, false);
            if (u != null)
            {
                targets.Add(new BOUNCETARGET
                {
                    unit = u,
                    distance = Chessboard.Instance.Pathfinding.GetDistance(targetNode, Chessboard.Instance.nodes[u.x, u.y])
                });
            }
            return targets;
        }

        // Targeting enemies
        for (int x = 0; x < activeUnits.GetLength(0); x++)
        {
            for (int y = 0; y < activeUnits.GetLength(1); y++)
            {
                var _unit = activeUnits[x, y];
                if (_unit != null && _unit != targetUnit && !bouncedOn.Contains(_unit))
                {
                    if (GameManager.Instance.IsValidTarget(shooter, _unit, targeting))
                    {
                        targets.Add(new BOUNCETARGET { unit = _unit, distance = Chessboard.Instance.Pathfinding.GetDistance(targetNode, Chessboard.Instance.nodes[_unit.x, _unit.y]) });
                    }
                }
            }
        }

        targets = targets.OrderBy(w => w.distance).ToList();

        if (ability.onlyOneBouncePerUnit == false)
            targets.Add(new BOUNCETARGET { unit = targetUnit, distance = 0 });

        return targets;
    }

    void Deactivate()
    {
        if (pool != null)
        {
            pool.Release(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
