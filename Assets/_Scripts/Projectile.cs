using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    RANGED, MELEE
}

public class Projectile : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    Node targetNode;
    float t;
    float dist;
    float forcedTimer;
    Unit targetUnit;
    Unit shooter;
    bool followUnit = false;
    Unit_NormalAttack attack;
    UnitAbility ability;
    Vector2Int[] path;

    public void Init(  //, Vector3 _startPos, Vector3 _endPos, Node _targetNode, float _flySpeed, float _damage, List<int> damagesTeams, float forcedTimer = 0f)
        Unit_NormalAttack _attack,
        Vector3 _startPos, 
        Vector2Int[] _path, 
        Unit _shooter,
        Unit _targetUnit = null) 
    {
        t = 0;
        path = _path;
        attack = _attack;
        startPos = _startPos;
        targetUnit = _targetUnit;
        shooter = _shooter;
        forcedTimer = attack.minLifeTime;

        if (targetUnit != null)
        {
            followUnit = true;
            dist = (startPos - targetUnit.transform.position).magnitude;
        }
        else
        {
            endPos = Chessboard.Instance.GetTileCenter(_path[_path.Length-1].x, _path[_path.Length-1].y) + Vector3.up * 1.5f;
            targetNode = Chessboard.Instance.nodes[_path[_path.Length - 1].x, _path[_path.Length - 1].y];
            followUnit = false;
            dist = (startPos - endPos).magnitude;
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
        while (t < dist || forcedTimer > 0)
        {
            forcedTimer -= Time.deltaTime;
            t += Time.deltaTime * attack.attackFlySpeed;
            if (followUnit && targetUnit != null)
                endPos = targetUnit.transform.position + Vector3.up * 1.5f;

            transform.LookAt(endPos);
            var perc = t / dist;
            transform.position = Vector3.Lerp(startPos, endPos, perc);
            yield return null;
        }

        // Hit target
        if (followUnit)
            targetNode = Chessboard.Instance.nodes[targetUnit.x, targetUnit.y];

        GameManager.Instance.DamageInstance.Activate(targetNode, attack.damage, shooter, attack.targeting, attack.dmgInstanceType);
        GameManager.Instance.ParticleSpawner.SpawnParticles(attack.hitParticle, transform.position);
        
        Destroy(gameObject);
    }


    IEnumerator ProjectileMelee()
    {
        var lookAtPos = Chessboard.Instance.GetTileCenter(path[0].x, path[0].y);
        transform.LookAt(new Vector3(lookAtPos.x, transform.position.y, lookAtPos.z));
        targetNode = Chessboard.Instance.nodes[path[0].x, path[0].y];

        // Hit target
        GameManager.Instance.DamageInstance.Activate(targetNode, attack.damage, shooter, attack.targeting, attack.dmgInstanceType);
        GameManager.Instance.ParticleSpawner.SpawnParticles(attack.hitParticle, transform.position);

        // Stay visible
        while (forcedTimer > 0)
        {
            forcedTimer -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
