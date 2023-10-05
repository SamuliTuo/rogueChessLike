using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public int team;

    public float maxHp;
    public float damage;
    public float magic;
    public float attackSpeed;
    public float moveSpeed;
    public float moveInterval;

    public float currentExperience;
    public float nextLevelExperience;
    public string unitName;
    public int spawnPosX;
    public int spawnPosY;
    public List<Unit_NormalAttack> attacks = new List<Unit_NormalAttack>();
    public UnitAbility ability1;
    public UnitAbility ability2;
    public UnitAbility ability3;
    public UnitAbility ability4;
    public List<UnitAbility> possibleAbilities = new List<UnitAbility>();

    public UnitData(Unit unit, int posX, int posY)
    {
        nextLevelExperience = 100;
        unitName = unit.name;
        this.damage = unit.GetDamage();
        this.magic = unit.GetMagic();
        this.attackSpeed = unit.attackSpeed;
        //this.moveSpeed = unit.moveSpeed;
        moveSpeed = unit.moveSpeed;
        moveInterval = unit.moveInterval;// CalculateMoveInterval(unit.moveInterval, moveSpeed);
        team = unit.team;
        spawnPosX = posX;
        spawnPosY = posY;
        maxHp = unit.GetComponent<UnitHealth>().GetMaxHp();
        this.possibleAbilities = unit.GetComponent<UnitAbilityManager>().possibleAbilities;
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
        switch (upgrade.upgradeType)
        {
            case "damage": 
                a.damage = Mathf.Sign(a.damage) * (Mathf.Abs(a.damage) + upgrade.upgradeAmount); break;

            case "cooldown":
                a.cooldown *= upgrade.upgradeAmount;
                if (a.cooldown < 0.5f)
                {
                    a.cooldown = 0.5f;
                    a.cooldown_upgradeable = false;
                }
                break;

            case "castSpeed":
                a.castDuration_firstHalf *= upgrade.upgradeAmount;
                if (a.castDuration_firstHalf < 0.1f)
                {
                    a.castDuration_firstHalf = 0.1f;
                    a.castSpeed_upgradeable = false;
                }
                break;

            case "flySpeed": a.flySpeed += upgrade.upgradeAmount; break;

            case "reach": a.reach += (int)upgrade.upgradeAmount; break;

            case "bounceCount": a.bounceCount_ability += (int)upgrade.upgradeAmount; break;

            case "bounceRange": a.bounceRange_ability += (int)upgrade.upgradeAmount; break;

            case "bounceDamageAmp": a.bounceDamagePercChangePerJump += upgrade.upgradeAmount; break;

            case "projectilesPerBounce": a.bounceSpawnCount_ability += (int)upgrade.upgradeAmount; break;

            case "spawnUnitCount": a.spawnCount += (int)upgrade.upgradeAmount; break;

            default:
                break;
        }
    }
}   