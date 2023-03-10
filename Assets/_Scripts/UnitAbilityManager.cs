using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAbilityManager : MonoBehaviour
{
    [SerializeField] private UnitAbility ability_1 = null;
    [SerializeField] private UnitAbility ability_2 = null;
    [SerializeField] private UnitAbility ability_3 = null;
    [SerializeField] private UnitAbility ability_4 = null;

    private Dictionary<UnitAbility, bool> abilitiesWithCooldown = new Dictionary<UnitAbility, bool>();
    private Unit thisUnit;

    private void Awake()
    {
        thisUnit = GetComponent<Unit>();
        abilitiesWithCooldown.Clear();

        if (ability_1 != null)
            InitAbility(ability_1);
        if (ability_2 != null)
            InitAbility(ability_2);
        if (ability_3 != null)
            InitAbility(ability_3);
        if (ability_4 != null)
           InitAbility(ability_4);
    }
    void InitAbility(UnitAbility _ability)
    {
        abilitiesWithCooldown.Add(_ability, true);
        StartCoroutine(AbilityCooldown(_ability, _ability.cooldown * 0.5f));

    }

    public UnitAbility ConsiderUsingAnAbility()
    {
        foreach (KeyValuePair<UnitAbility, bool> item in abilitiesWithCooldown)
        {
            if (item.Value == false)
            {
                return item.Key;
            }
        }
        return null;
    }

    public void ActivateAbility(UnitAbility _ability, Unit _attackTarget, Vector2Int[] _path)
    {
        if (_attackTarget == null)
        {
            thisUnit.savedAttackTimerAmount = _ability.castSpeed * thisUnit.percentOfAttackTimerSave;
            thisUnit.ResetAI();
            return;
        }
        thisUnit.savedAttackTimerAmount = 0;

        if (_path != null && _path.Length > 0)
        {
            var targetPos = _path[_path.Length - 1];
            thisUnit.RotateUnit(targetPos);
        }

        Vector3 offset = transform.TransformVector(thisUnit.attackPositionOffset);
        Vector3 startPos = transform.position + offset;

        abilitiesWithCooldown[_ability] = true;
        StartCoroutine(AbilityCooldown(_ability, _ability.cooldown));

        GameObject clone = Instantiate(_ability.projectile, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        clone.GetComponent<AbilityInstance>().Init(_ability, startPos, _path, _ability.bounceCount_atk, _ability.bounceCount_ability, thisUnit, _attackTarget);

        thisUnit.ResetAI();
    }

    IEnumerator AbilityCooldown(UnitAbility _ability, float _cooldown)
    {
        var t = Time.time;
        while (Time.time < t + _cooldown)
        {
            yield return null;
        }
        abilitiesWithCooldown[_ability] = false;
    }
}
