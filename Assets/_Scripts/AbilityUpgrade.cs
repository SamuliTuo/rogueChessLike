using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbilityUpgrade
{
    public UnitAbility ability;
    public string upgradeType;
    public float upgradeAmount;

    public AbilityUpgrade(UnitAbility ability, string upgradeType, float upgradeAmount)
    {
        this.ability = ability;
        this.upgradeType = upgradeType;
        this.upgradeAmount = upgradeAmount;
    }
}
