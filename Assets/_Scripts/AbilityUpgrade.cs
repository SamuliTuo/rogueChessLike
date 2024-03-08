using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AbilityUpgradeType
{
    DAMAGE,
    COOLDOWN,
    CAST_SPEED,
    FLY_SPEED,
    REACH,
    BOUNCE_COUNT,
    BOUNCE_RANGE,
    BOUNCE_DMG_AMP,
    PROJECTILES_PER_BOUNCE,
    SPAWN_UNIT_COUNT,
}

public class AbilityUpgrade
{
    public UnitAbility ability;
    public AbilityUpgradeType upgradeType;
    public float upgradeAmount;

    public AbilityUpgrade(UnitAbility ability, AbilityUpgradeType upgradeType, float upgradeAmount)
    {
        this.ability = ability;
        this.upgradeType = upgradeType;
        this.upgradeAmount = upgradeAmount;
    }
}
