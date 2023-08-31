using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUpgrade : MonoBehaviour
{
    UnitAbility ability;
    string upgradeName;
    float upgradeAmount;


    public AbilityUpgrade(UnitAbility ability, string upgradeName, float upgradeAmount)
    {
        this.ability = ability;
        this.upgradeName = upgradeName;
        this.upgradeAmount = upgradeAmount;
    }

}
