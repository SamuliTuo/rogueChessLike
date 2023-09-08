using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class AbilityUpgrade
{
    public UnitAbility ability;
    public string upgradeName;
    public float upgradeAmount;


    public AbilityUpgrade(UnitAbility ability, string upgradeName, float upgradeAmount)
    {
        this.ability = ability;
        this.upgradeName = upgradeName;
        this.upgradeAmount = upgradeAmount;
    }

}
