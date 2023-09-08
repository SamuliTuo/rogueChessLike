using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public int team;
    public float currentExperience;
    public float nextLevelExperience;
    public float maxHp;
    public float moveInterval;
    public string unitName;
    public int spawnPosX;
    public int spawnPosY;
    public List<Unit_NormalAttack> attacks;
    public UnitAbility ability1;
    public UnitAbility ability2;
    public UnitAbility ability3;
    public UnitAbility ability4;
    public List<UnitAbility> possibleAbilities = new List<UnitAbility>();

    public UnitData(Unit unit, int posX, int posY)
    {
        nextLevelExperience = 100;
        unitName = unit.name;
        moveInterval = unit.moveInterval;
        team = unit.team;
        spawnPosX = posX;
        spawnPosY = posY;
        attacks = unit.normalAttacks;
        maxHp = unit.GetComponent<UnitHealth>().GetMaxHp();
        var abilities = unit.GetComponent<UnitAbilityManager>();
        ability1 = abilities.ability_1;
        ability2 = abilities.ability_2;
        ability3 = abilities.ability_3;
        ability4 = abilities.ability_4;
        this.possibleAbilities = abilities.possibleAbilities;
    }

    public float CurrentExpPercent()
    {
        return currentExperience / nextLevelExperience;
    }

    public void GiveNewAbility(UnitAbility abi)
    {
        if      (ability1 == null) ability1 = abi;
        else if (ability2 == null) ability2 = abi;
        else if (ability3 == null) ability3 = abi;
        else if (ability4 == null) ability4 = abi;
    }

    public bool HasFreeAbilitySlots()
    {
        return (ability1 == null || ability2 == null || ability3 == null || ability4 == null);
    }

    public bool HasLearnedAbility(UnitAbility a)
    {
        return ((ability1!=null && ability1.name==a.name) || (ability2!=null && ability2.name==a.name) || (ability3!=null && ability3.name==a.name) || (ability4!=null && ability4.name==a.name));
    }

    public List<UnitAbility> LearnedAbilities()
    {
        var r = new List<UnitAbility>();
        if (ability1 != null) r.Add(ability1);
        if (ability2 != null) r.Add(ability2);
        if (ability3 != null) r.Add(ability3);
        if (ability4 != null) r.Add(ability4);
        return r;
    }

    public List<UnitAbility> RemainingPossibleAbilities()
    {
        var r = new List<UnitAbility>();
        foreach (var a in possibleAbilities)
        {
            if (!HasLearnedAbility(a))
            {
                r.Add(a);
            }
        }
        return r;
    }
    
    public int AbilityIsInSlot(UnitAbility abi)
    {
        if      (ability1 = abi) return 1;
        else if (ability2 = abi) return 2;
        else if (ability3 = abi) return 3;
        else if (ability4 = abi) return 4;
        else return -1;
    }

    public void UpgradeAbility(AbilityUpgrade upgrade)
    {
        if (ability1 == upgrade.ability)
            GiveAbilityUpgrade(ability1, upgrade);
        else if (ability2 == upgrade.ability)
            GiveAbilityUpgrade(ability2, upgrade);
        else if (ability3 == upgrade.ability)
            GiveAbilityUpgrade(ability3, upgrade);
        else if (ability4 == upgrade.ability)
            GiveAbilityUpgrade(ability4, upgrade);
    }

    void GiveAbilityUpgrade(UnitAbility a, AbilityUpgrade upgrade)
    {
        switch (upgrade.upgradeName)
        {
            case "damage": a.damage += upgrade.upgradeAmount; break;

            case "bounceAbilityAmount": a.bounceCount_ability += (int)upgrade.upgradeAmount; break;

            case "reach": a.reach += (int)upgrade.upgradeAmount; break;

            case "cooldown":
                a.cooldown -= upgrade.upgradeAmount;
                if (a.cooldown < 0.5f) a.cooldown = 0.5f;
                break;

            case "spawnCount": a.spawnCount += (int)upgrade.upgradeAmount; break;

            case "flySpeed": a.flySpeed += upgrade.upgradeAmount; break;

            default:
                break;
        }
    }
}   