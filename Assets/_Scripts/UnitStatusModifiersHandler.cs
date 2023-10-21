using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatusModifiersHandler : MonoBehaviour
{
    private Unit unit;
    private UnitHealth health;
    private bool immune = false;
    private bool silenced = false;
    private Coroutine silenceCoroutine = null;
    
    private void Start()
    {
        unit = GetComponent<Unit>();
        health = GetComponent<UnitHealth>();
    }

    // ================== PUBLIC ================== //
    public void AddNewStatusModifiers(UnitStatusModifier statuses)
    {
        if (immune) return;

        if (statuses.slowsMovementSpeed) StartCoroutine(MovementSpeedMod(statuses));
        if (statuses.slowsAttackSpeed) StartCoroutine(AttackSpeedMod(statuses));
        if (statuses.givesCritChance) StartCoroutine(CritChanceMod(statuses));
        if (statuses.givesCritDamage) StartCoroutine(CritDamageMod(statuses));
        if (statuses.givesMissChance) StartCoroutine(MissChance(statuses));
        if (statuses.givesLifesteal_flat) StartCoroutine(LifeStealMod_flat(statuses));
        if (statuses.givesLifesteal_perc) StartCoroutine(LifeStealMod_perc(statuses));
        if (statuses.damagesOverTime) StartCoroutine(DamageOverTime(statuses));
        if (statuses.givesShield) StartCoroutine(health.AddShield(statuses.shieldAmount, statuses.shieldDuration));
        if (statuses.givesImmunity) StartCoroutine(Immunity(statuses));
        if (statuses.stuns) unit.GetStunned(statuses.stunDuration);

        if (statuses.silences)
        {
            if (silenceCoroutine == null)
                StopCoroutine(silenceCoroutine);

            silenceCoroutine = StartCoroutine(Silence(statuses));
        }

        /*if (statuses.cleanses)
        {
            if (silenceCoroutine != null)
            {
                StopCoroutine(silenceCoroutine);
                silenced = false;
            }
            unit.tStun = 0;
        }*/
    }

    private IEnumerator MovementSpeedMod(UnitStatusModifier statuses)
    {
        unit.moveSpeed += statuses.movementSpeedSlow;
        yield return new WaitForSeconds(statuses.movementSpeedSlowDuration);
        unit.moveSpeed -= statuses.movementSpeedSlow;
    }
    private IEnumerator AttackSpeedMod(UnitStatusModifier statuses)
    {
        unit.attackSpeed += statuses.attackSpeedSlow;
        yield return new WaitForSeconds(statuses.attackSpeedSlowDuration);
        unit.attackSpeed -= statuses.attackSpeedSlow;
    }
    private IEnumerator CritChanceMod(UnitStatusModifier statuses)
    {
        unit.critChance += statuses.critChance;
        yield return new WaitForSeconds(statuses.critChanceDuration);
        unit.critChance -= statuses.critChance;
    }
    private IEnumerator CritDamageMod(UnitStatusModifier statuses)
    {
        //...
        print("gaining crit DMG for " + statuses.critDamageDuration + " seconds");
        yield return new WaitForSeconds(statuses.critDamageDuration);
        // TEKEMÄTTÄ!
    }
    private IEnumerator MissChance(UnitStatusModifier statuses)
    {
        unit.missChance += statuses.missChance;
        yield return null;
        unit.missChance -= statuses.missChance;
    }
    private IEnumerator LifeStealMod_flat(UnitStatusModifier statuses)
    {
        unit.lifeSteal_flat += statuses.lifesteal_flat;
        yield return new WaitForSeconds(statuses.lifestealDuration_flat);
        unit.lifeSteal_flat -= statuses.lifesteal_flat;
    }
    private IEnumerator LifeStealMod_perc(UnitStatusModifier statuses)
    {
        unit.lifeSteal_perc += statuses.lifesteal_perc;
        yield return new WaitForSeconds(statuses.lifestealDuration_perc);
        unit.lifeSteal_perc -= statuses.lifesteal_perc;
    }
    private IEnumerator Immunity(UnitStatusModifier statuses)
    {
        immune = true;
        yield return new WaitForSeconds(statuses.immunityDuration);
        immune = false;
    }
    private IEnumerator Silence(UnitStatusModifier statuses)
    {
        silenced = true;
        yield return new WaitForSeconds(statuses.silenceDuration);
        silenced = false;
        silenceCoroutine = null;
    }

    private IEnumerator DamageOverTime(UnitStatusModifier statuses)
    {
        int i = 0;
        while (i < statuses.dotIntervalCount)
        {
            health.RemoveHP(statuses.dotTickDamage);

            // Wait for the next interval:
            float t = 0;
            while (t < statuses.dotTickIntervalSeconds)
            {
                t += Time.deltaTime;
                yield return null;
            }
            i++;
            yield return null;
        }
    }
}