using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAbilityManager : MonoBehaviour
{
    public List<UnitAbility> abilities = new List<UnitAbility>();
    public List<GameObject> projectiles = new List<GameObject>();

    public List<UnitAbility> possibleAbilities = new List<UnitAbility>();

    private Dictionary<Tuple<UnitAbility, int>, bool> abilitiesWithCooldown = new Dictionary<Tuple<UnitAbility, int>, bool>();
    private Unit unit;
    private UnitHealth hp;
    private Animator animator;

    public void StartAbilities()
    {
        unit = GetComponent<Unit>();
        hp = GetComponent<UnitHealth>();
        abilitiesWithCooldown.Clear();
        for (int i = 0; i < 3; i++) 
        {
            if (abilities.Count == 0 || i >= abilities.Count || abilities[i] == null)
            {
                hp.StartCoroutine(hp.SetSkillSymbol(null, i));
                continue;
            }
            StartAbility(i, abilities[i]);
            hp.StartCoroutine(hp.SetSkillSymbol(abilities[i], i));
        }
        animator = GetComponentInChildren<Animator>();
    }

    void StartAbility(int i, UnitAbility _ability)
    {
        abilitiesWithCooldown.Add(new Tuple<UnitAbility, int>(_ability, i), true);
        GameManager.Instance.ProjectilePools.CreatePool(projectiles[i]);
        hp.StartCoroutine(AbilityCooldown(new Tuple<UnitAbility, int>(_ability, i), _ability.cooldown, _ability.startCooldownMultiplier));
    }

    public Tuple<UnitAbility, int> ConsiderUsingAnAbility()
    {
        foreach (KeyValuePair<Tuple<UnitAbility, int>, bool> item in abilitiesWithCooldown)
        {
            if (item.Value == false)
            {
                return item.Key;
            }
        }
        return null;
    }

    //public int GetFreeSlot()
    //{
    //    if (ability_1 == null) return 0;
    //    if (ability_2 == null) return 1;
    //    if (ability_3 == null) return 2;
    //    return -1;
    //}

    public int additionalPhases = 0;
    public void ActivateAbility(Tuple<UnitAbility, int> _ability, Unit _attackTarget, Vector2Int[] _path)
    {
        unit.t = _ability.Item1.castDuration_firstHalf;

        if (_attackTarget == null)
        {
            unit.savedAttackTimerAmount = _ability.Item1.castDuration_firstHalf * unit.percentOfAttackTimerSave;
            unit.ResetAI();
            return;
        }
        unit.nextAction = Action.ABILITY_SECONDHALF;
        unit.savedAttackTimerAmount = 0;

        if (_ability.Item1.additionalDamagePhases > 0)
        {
            additionalPhases = _ability.Item1.additionalDamagePhases;
        }
        else
        {
            additionalPhases = 0;
        }

        if (animator != null)
        {
            animator.speed = 1;
            animator.Play("ability", 0, 0);
        }
        
        /*if (_path != null && _path.Length > 0)
        {
            var targetPos = _path[_path.Length - 1];
            //thisUnit.RotateUnit(targetPos);
        }*/
        unit.RotateUnit(new Vector2Int(_attackTarget.x, _attackTarget.y));
    }

    public void ActivateAbilitySecondHalf(Tuple<UnitAbility, int> _ability, Unit _attackTarget, Vector2Int[] _path)
    {
        Vector3 offset = transform.TransformVector(unit.attackPositionOffset);
        Vector3 startPos = transform.position + offset;

        var projectile = GameManager.Instance.ProjectilePools.SpawnProjectile(
            projectiles[_ability.Item2], startPos, Quaternion.identity);

        projectile?.GetComponent<AbilityInstance>().Init(
            _ability.Item1, startPos, _path, _ability.Item1.bounceCount_atk,
            _ability.Item1.bounceCount_ability, unit.GetAbilityDmg(_ability.Item1), unit.critChance, unit.critDamagePerc, unit.missChance, unit, _attackTarget);

        if (additionalPhases > 0)
        {
            unit.t = _ability.Item1.additionalDamagePhaseDuration;
            additionalPhases--;
        }
        else
        {
            unit.t = _ability.Item1.castDuration_secondHalf;
            if (_ability.Item1.cooldown > 0)
            {
                abilitiesWithCooldown[_ability] = true;
                StartCoroutine(AbilityCooldown(_ability, _ability.Item1.cooldown));
            }
            else
            {
                abilitiesWithCooldown[_ability] = false;
            }
            unit.ResetAI();
        }
    }

    IEnumerator AbilityCooldown(Tuple<UnitAbility, int> _ability, float _cooldown, float _startMultiplier = 0)
    {
        float t = _cooldown * _startMultiplier;
        while (t < _cooldown)
        {
            hp?.RefreshSkillCooldownUISlot(_ability.Item2, t / _cooldown);
            if (GameManager.Instance.state == GameState.BATTLE)
            {
                t += Time.deltaTime;
            }
            yield return null;
        }
        abilitiesWithCooldown[_ability] = false;
    }
}
