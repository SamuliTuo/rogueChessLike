using System.Collections;
using System.Collections.Generic;
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
           abilitiesWithCooldown.Add(ability_1, false);
        if (ability_2 != null)
           abilitiesWithCooldown.Add(ability_2, false);
        if (ability_3 != null)
           abilitiesWithCooldown.Add(ability_3, false);
        if (ability_4 != null)
           abilitiesWithCooldown.Add(ability_4, false);
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
        abilitiesWithCooldown[_ability] = true;
        StartCoroutine("AbilityCooldown", _ability);

        
        if (_attackTarget == null)
        {
            thisUnit.savedAttackTimerAmount = _ability.castSpeed * thisUnit.percentOfAttackTimerSave;
            thisUnit.ResetAI();
            return;
        }
        thisUnit.savedAttackTimerAmount = 0;

        if (_path != null)
        {
            var targetPos = _path[_path.Length - 1];
            thisUnit.RotateUnit(targetPos);
        }

        Vector3 offset = transform.TransformVector(thisUnit.attackPositionOffset);
        Vector3 startPos = transform.position + offset;

        GameObject clone = Instantiate(_ability.projectile, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        clone.GetComponent<AbilityInstance>().Init(_ability, startPos, _path, thisUnit, _attackTarget);

        thisUnit.ResetAI();
    }

    IEnumerator AbilityCooldown(UnitAbility _ability)
    {
        var t = Time.time;
        while (Time.time < t + _ability.cooldown)
        {
            yield return null;
        }
        abilitiesWithCooldown[_ability] = false;
    }
}
