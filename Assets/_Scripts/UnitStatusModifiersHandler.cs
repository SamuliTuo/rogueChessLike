using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatusModifiersHandler : MonoBehaviour
{
    private Unit unit;
    private bool immune = false;
    private bool silenced = false;
    private Coroutine immunityCoroutine = null;
    private Coroutine silenceCoroutine = null;
    
    private void Start()
    {
        unit = GetComponent<Unit>();
    }

    // ================== PUBLIC ================== //
    public void AddNewStatusModifiers(UnitStatusModifier statuses)
    {
        if (statuses.slowsMovementSpeed)
            StartCoroutine(MovementSpeedMod(statuses));

        if (statuses.slowsAttackSpeed)
            StartCoroutine(AttackSpeedMod(statuses));

        if (statuses.givesCritChance)
            StartCoroutine(CritChanceMod(statuses));

        if (statuses.givesCritDamage)
            StartCoroutine(CritDamageMod(statuses));

        if (statuses.givesMissChance)
            StartCoroutine(ShieldMod(statuses));

        if (statuses.givesLifesteal)
            StartCoroutine(LifeStealMod(statuses));

        if (statuses.givesImmunity)
        {
            if (immunityCoroutine == null) 
                StopCoroutine(immunityCoroutine);

            immunityCoroutine = StartCoroutine(Immunity(statuses));
        }

        //if (statuses.stuns)
            //unit.GetStunned(statuses.stunDuration);

        if (statuses.silences)
        {
            if (silenceCoroutine == null)
                StopCoroutine(silenceCoroutine);

            silenceCoroutine = StartCoroutine(Silence(statuses));
        }

        if (statuses.cleanses)
        {
            if (silenceCoroutine != null)
            {
                StopCoroutine(silenceCoroutine);
                silenced = false;
            }
            //unit.ClearStun();
                
        }
    }

    private IEnumerator MovementSpeedMod(UnitStatusModifier statuses)
    {
        unit.moveSpeed += statuses.movementSpeedSlow;
        yield return new WaitForSeconds(statuses.movementSpeedSlow);
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
        //unit.critchance += jne
        print("gaining crit chance for " + statuses.critChanceDuration + " seconds");
        yield return new WaitForSeconds(statuses.critChanceDuration);
        // TEKEMÄTTÄ!
    }
    private IEnumerator CritDamageMod(UnitStatusModifier statuses)
    {
        //...
        print("gaining crit DMG for " + statuses.critDamageDuration + " seconds");
        yield return new WaitForSeconds(statuses.critDamageDuration);
        // TEKEMÄTTÄ!
    }
    private IEnumerator ShieldMod(UnitStatusModifier statuses)
    {
        //
        print("gaining shield for " + statuses.shieldDuration + " seconds");
        yield return new WaitForSeconds(statuses.shieldDuration);
        // TEKEMÄTTÄ!
    }
    private IEnumerator LifeStealMod(UnitStatusModifier statuses)
    {
        //
        print("gaining crit DMG for " + statuses.lifestealDuration + " seconds");
        yield return new WaitForSeconds(statuses.lifestealDuration);
        // TEKEMÄTTÄ!
    }
    private IEnumerator Immunity(UnitStatusModifier statuses)
    {
        immune = true;
        yield return new WaitForSeconds(statuses.immunityDuration);
        immune = false;
        immunityCoroutine = null;
    }
    private IEnumerator Silence(UnitStatusModifier statuses)
    {
        silenced = true;
        yield return new WaitForSeconds(statuses.silenceDuration);
        silenced = false;
        silenceCoroutine = null;
    }
}
