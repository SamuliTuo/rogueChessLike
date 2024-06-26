using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewUnitStatusModifier", menuName = "ScriptableObjects/NewUnitStatusModifier", order = 8)]

public class UnitStatusModifier : ScriptableObject
{
    [Header("Give a shield:")]
    public bool givesShield = false;
    public float shieldAmount = 0;
    public float shieldDuration = 0;

    [Header("Stun:")]
    public bool stuns = false;
    public float stunDuration = 0;

    [Header("Silence:")]
    public bool silences = false;
    public float silenceDuration = 0;

    //[Header("Cleanse:")]
    //public bool cleanses = false;

    [Header("Immunity to statuses:")]
    public bool givesImmunity = false;
    public float immunityDuration = 0;

    [Header("(You can use negative values for healing, attSpeed, etc... where it makes sense)")]
    [Header("Damage over time:")]
    public bool burns = false;
    public float burn_tickDamage = 0;
    public float burn_tickIntervalSeconds = 0;
    public int burn_intervalCount = 0;

    [Header("Burning attacks -augment:")]
    public bool burningAttacks = false;
    public float burningAttacks_tickDamage = 0;
    public float burningAttacks_tickInterval = 0;
    public int burningAttacks_intervalCount = 0;

    [Header("Slow movement speed:")]
    public bool slowsMovementSpeed = false;
    public float movementSpeedSlow = 0;
    public float movementSpeedSlowDuration = 0;

    [Header("Slow attack speed:")]
    public bool slowsAttackSpeed = false;
    public float attackSpeedSlow = 0;
    public float attackSpeedSlowDuration = 0;

    [Header("Give attacks miss chance: (1 = 100% miss chance)")]
    public bool givesMissChance = false;
    [Range(0, 1)]
    public float missChance = 0;
    public float missChanceDuration = 0;

    [Header("Give attacks crit chance: (1 = 100% crit chance)")]
    public bool givesCritChance = false;
    [Range(0, 1)]
    public float critChance = 0;
    public float critChanceDuration = 0;

    [Header("Give attacks crit damage: (1 = 100% crit damage)")]
    public bool givesCritDamage = false;
    [Range(0, 1)]
    public float critDamage = 0;
    public float critDamageDuration = 0;

    [Header("Give attacks flat amounts of lifesteal: (1 = 1 hp lifesteal)")]
    public bool givesLifesteal_flat = false;
    public float lifesteal_flat = 0;
    public float lifestealDuration_flat = 0;

    [Header("Give attacks percentage lifesteal: (1 = 100% lifeSteal")]
    public bool givesLifesteal_perc = false;
    [Range(0, 10)]
    public float lifesteal_perc = 0;
    public float lifestealDuration_perc = 0;
}
