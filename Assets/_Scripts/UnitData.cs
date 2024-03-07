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
    public float critChance;
    public float critDamage;
    public float missChance;
    public float attackSpeed;
    public float moveSpeed;
    public float moveInterval;
    public float armor;
    public float magicRes;

    public int currentLevel = 1;
    public float currentExperience;
    public float nextLevelExperience;
    public string name;
    public int spawnPosX;
    public int spawnPosY;
    public List<Unit_NormalAttack> attacks = new List<Unit_NormalAttack>();
    public bool randomizeAttacks = false;
    public UnitAbility ability1;
    public UnitAbility ability2;
    public UnitAbility ability3;
    public List<UnitAbility> possibleAbilities = new List<UnitAbility>();

    public UnitData(Unit unit, string name, int posX, int posY, int level = 1)
    {
        nextLevelExperience = 100;
        this.name = name;
        this.currentLevel = level;
        this.damage = unit.GetDamage();
        this.magic = unit.GetMagic();
        this.critChance = unit.critChance;
        this.critDamage = unit.critDamagePerc;
        this.missChance = unit.missChance;
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
    }

    public bool HasFreeAbilitySlots()
    {
        return (ability1 == null || ability2 == null || ability3 == null);
    }

    public bool HasLearnedAbility(UnitAbility a)
    {
        return ((ability1!=null && ability1.name==a.name) || (ability2!=null && ability2.name==a.name) || (ability3!=null && ability3.name==a.name));
    }

    public List<UnitAbility> LearnedAbilities()
    {
        var r = new List<UnitAbility>();
        if (ability1 != null) r.Add(ability1);
        if (ability2 != null) r.Add(ability2);
        if (ability3 != null) r.Add(ability3);
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
        if (ability1 = abi) return 1;
        else if (ability2 = abi) return 2;
        else if (ability3 = abi) return 3;
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
    }

    void GiveAbilityUpgrade(UnitAbility a, AbilityUpgrade upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case AbilityUpgradeType.DAMAGE:
                a.damage = Mathf.Sign(a.damage) * (Mathf.Abs(a.damage) + upgrade.upgradeAmount); 
                break;

            case AbilityUpgradeType.COOLDOWN:
                a.cooldown *= upgrade.upgradeAmount;
                if (a.cooldown < 0.5f)
                {
                    a.cooldown = 0.5f;
                    a.cooldown_upgradeable = false;
                }
                break;

            case AbilityUpgradeType.CAST_SPEED:
                a.castDuration_firstHalf *= upgrade.upgradeAmount;
                if (a.castDuration_firstHalf < 0.1f)
                {
                    a.castDuration_firstHalf = 0.1f;
                    a.castSpeed_upgradeable = false;
                }
                break;

            case AbilityUpgradeType.FLY_SPEED: 
                a.flySpeed += upgrade.upgradeAmount; 
                break;

            case AbilityUpgradeType.REACH: 
                a.reach += (int)upgrade.upgradeAmount; 
                break;

            case AbilityUpgradeType.BOUNCE_COUNT: 
                a.bounceCount_ability += (int)upgrade.upgradeAmount; 
                break;

            case  AbilityUpgradeType.BOUNCE_RANGE: 
                a.bounceRange_ability += (int)upgrade.upgradeAmount; 
                break;

            case AbilityUpgradeType.BOUNCE_DMG_AMP: 
                a.bounceDamagePercChangePerJump += upgrade.upgradeAmount; 
                break;

            case AbilityUpgradeType.PROJECTILES_PER_BOUNCE: 
                a.bounceSpawnCount_ability += (int)upgrade.upgradeAmount; break;

            case AbilityUpgradeType.SPAWN_UNIT_COUNT: 
                a.spawnCount += (int)upgrade.upgradeAmount; 
                break;

            default: 
                break;
        }
    }
}   