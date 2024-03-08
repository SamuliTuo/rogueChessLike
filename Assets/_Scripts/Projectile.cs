using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.Pool;

[System.Serializable]
public enum ProjectileType
{
    RANGED, MELEE
}

public class Projectile : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    Node targetNode; 
    int bouncesRemainingAttack;
    int bouncesRemainingAbility;
    public float bounceDamagePercChangePerJump = 1;
    float t;
    float dist;
    float forcedTimer;
    float damage;
    float critChance;
    float critDamage;
    float missChance;
    Unit targetUnit;
    Unit shooter;
    bool followUnit = false;
    Unit_NormalAttack attack;
    UnitAbility ability;
    Vector2Int[] path;
    List<Unit> bouncedOn;

    private IObjectPool<GameObject> pool;
    public void SetPool(IObjectPool<GameObject> pool) => this.pool = pool;

    public void Init(
        Unit_NormalAttack _attack,
        Vector3 _startPos,
        Vector2Int[] _path,
        int _attackBounces,
        int _abilityBounces,
        float _damage,
        float _critChance,
        float _critDamage,
        float _missChance,
        Unit _shooter,
        Unit _targetUnit = null,
        List<Unit> _bouncedOn = null)
    {
        t = 0;
        path = _path;
        attack = _attack;
        startPos = _startPos;
        targetUnit = _targetUnit;
        shooter = _shooter;
        forcedTimer = attack.minLifeTime;
        damage = _damage;
        critChance = _critChance;
        critDamage = _critDamage;
        missChance = _missChance;
        bouncesRemainingAttack = _attackBounces;
        bouncesRemainingAbility = _abilityBounces;

        bouncedOn = _bouncedOn;
        if (attack.onlyOneBouncePerUnit == false)
            bouncedOn = null;

        if (targetUnit != null)
        {
            followUnit = true;
            dist = (startPos - targetUnit.transform.position).magnitude;
        }
        else if (path != null)
        {
            endPos = Chessboard.Instance.GetTileCenter(_path[_path.Length - 1].x, _path[_path.Length - 1].y) + Vector3.up * 1.5f;
            targetNode = Chessboard.Instance.nodes[_path[_path.Length - 1].x, _path[_path.Length - 1].y];
            followUnit = false;
            dist = (startPos - endPos).magnitude;
        }
        else
        {
            Deactivate();
            return;
        }
        
        if (attack.projectileType == ProjectileType.RANGED)
        {
            StartCoroutine("ProjectileMotion");
        }
        else if (attack.projectileType == ProjectileType.MELEE)
        {
            StartCoroutine("ProjectileMelee");
        }
    }


    IEnumerator ProjectileMotion()
    {
        // Fly
        bool atTarget = false;
        Vector3 lastDir = Vector3.zero;

        while (!atTarget || forcedTimer > 0)
        {
            forcedTimer -= Time.deltaTime;
            var dir = endPos - transform.position;
            if (dir.sqrMagnitude < 0.01f)
            {
                atTarget = true;
                continue;
            }

            //t += Time.deltaTime * attack.attackFlySpeed;
            if (followUnit && targetUnit != null)
                endPos = targetUnit.transform.position + Vector3.up * 1.5f;

            transform.LookAt(endPos);
            //var perc = t / dist;
            dir = lastDir.normalized * 0.86f + dir.normalized;
            transform.position += dir * attack.attackFlySpeed * Time.deltaTime; //Vector3.Lerp(startPos, endPos, perc);
            lastDir = dir;
            yield return null;
        }

        // Hit target
        if (followUnit)
            targetNode = Chessboard.Instance.nodes[targetUnit.x, targetUnit.y];

        // Hit target
        GameManager.Instance.ParticleSpawner.SpawnParticles(attack.hitParticle, transform.position, transform.forward);
        if (Hits())
        {
            if (Crits())
            {
                damage *= critDamage;
            }
            GameManager.Instance.DamageInstance.Activate(targetNode, damage, critChance, critDamage, missChance, shooter, attack.targeting, attack.dmgInstanceType, attack.statusModifiers);
        }
        Bounces();
        Deactivate();
    }


    IEnumerator ProjectileMelee()
    {
        if (targetUnit == null)
        {
            if (path == null || path.Length <= 0)
            {
                yield break;
            }
        }

        Vector2Int targetPos = targetUnit != null ? new(targetUnit.x, targetUnit.y) : path.Length > 0 ? new(path[0].x, path[0].y) : new(-1, -1);
        var lookAtPos = Chessboard.Instance.GetTileCenter(targetPos.x, targetPos.y);
        transform.LookAt(new Vector3(lookAtPos.x, transform.position.y, lookAtPos.z));
        targetNode = Chessboard.Instance.nodes[targetPos.x, targetPos.y];
        GameManager.Instance.ParticleSpawner.SpawnParticles(attack.hitParticle, Chessboard.Instance.GetTileCenter(targetNode.x, targetNode.y), transform.forward);

        // Hit target
        if (Hits())
        {
            if (Crits())
            {
                damage *= critDamage;
            }
            GameManager.Instance.DamageInstance.Activate(targetNode, damage, critChance, critDamage, missChance, shooter, attack.targeting, attack.dmgInstanceType, attack.statusModifiers, attack.hitParticle);
        }

        // Stay visible
        while (forcedTimer > 0)
        {
            forcedTimer -= Time.deltaTime;
            yield return null;
        }

        Bounces();
        Deactivate();
    }

    bool Hits()
    {
        if (missChance > 0)
        {
            if (Random.Range(0.000f, 1.000f) < missChance)
            {
                return false;
            }
        }
        return true;
    }
    bool Crits()
    {
        if (critChance > 0)
        {
            if (Random.Range(0.000f, 1.000f) <= critChance)
            {
                return true;
            }
        }
        return false;
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
        List<BOUNCETARGET> targets = FindTargetsForBounces(attack.bounceSpawnCount_atk, attack.bounceAttack_targeting);
        float bounceDamage = damage * attack.bounceDamagePercChangePerJump;
        foreach (BOUNCETARGET target in targets)
        {
            if (i > attack.bounceSpawnCount_atk)
                break;
                

            if (target.distance >= attack.bounceRange_atk)
                continue;

            i++;
            var projectile = Resources.Load<GameObject>(attack.bounceAttack.projectilePath);
            var clone = Instantiate(projectile, transform.position, Quaternion.identity);
            clone.GetComponent<Projectile>().Init(
                attack.bounceAttack, 
                transform.position,
                null, 
                bouncesRemainingAttack - 1, 
                bouncesRemainingAbility - 1,
                bounceDamage,
                critChance,
                critDamage,
                missChance,
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
        List<BOUNCETARGET> targets = FindTargetsForBounces(attack.bounceSpawnCount_ability, attack.bounceAbility_targeting);
        float bounceDamage = damage * attack.bounceDamagePercChangePerJump;
        foreach (BOUNCETARGET target in targets)
        {
            if (i > attack.bounceSpawnCount_ability)
                break;

            if (target.distance >= attack.bounceRange_ability)
                continue;

            i++;
            var projectile = Resources.Load<GameObject>(ability.bounceAbility.projectilePath);
            var clone = Instantiate(projectile, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            clone.GetComponent<AbilityInstance>().Init(
                attack.bounceAbility, 
                transform.position, 
                null, 
                bouncesRemainingAttack - 1, 
                bouncesRemainingAbility - 1, 
                bounceDamage,
                critChance,
                critDamage,
                missChance,
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







    public class BOUNCETARGET
    {
        public Unit unit { get; set; }
        public float distance { get; set; }
    }
    List<BOUNCETARGET> FindTargetsForBounces(int bounces, UnitSearchType targeting)
    {

        List<BOUNCETARGET> targets = new List<BOUNCETARGET>();
        var r = new List<BOUNCETARGET>();

        Unit[,] activeUnits = Chessboard.Instance.GetUnits();
        for (int x = 0; x < activeUnits.GetLength(0); x++)
        {
            for (int y = 0; y < activeUnits.GetLength(1); y++)
            {
                var _unit = activeUnits[x, y];
                if (_unit != null && _unit != targetUnit && !bouncedOn.Contains(_unit))
                {
                    if (GameManager.Instance.IsValidTarget(shooter.team, _unit, targeting))
                    {
                        targets.Add(new BOUNCETARGET { unit = _unit, distance = Chessboard.Instance.Pathfinding.GetDistance(targetNode, Chessboard.Instance.nodes[_unit.x, _unit.y]) });
                    }
                }
            }
        }

        targets = targets.OrderBy(w => w.distance).ToList();

        for (int i = 0; i < bounces; i++)
        {
            if (i >= targets.Count)
                return r;

            if (targets[i] == null)
                return r;

            r.Add(targets[i]);
        }

        return r;
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
