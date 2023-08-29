using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public int team;
    public float currentExperience;
    public float nextLevelExperience;
    public float maxHp;
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
        if (ability1 == null)
            ability1 = abi;
        else if (ability2 == null)
            ability2 = abi;
        else if (ability3 == null)
            ability3 = abi;
        else if (ability4 == null)
            ability4 = abi;
    }

    public bool HasFreeAbilitySlots()
    {
        return (ability1 == null || ability2 == null || ability3 == null || ability4 == null);
    }
    public bool LearnedAbility(UnitAbility a)
    {
        return (ability1 == a || ability2 == a || ability3 == a || ability4 == a);
    }
    public List<UnitAbility> RemainingPossibleAbilities()
    {
        var r = new List<UnitAbility>();
        foreach (var a in possibleAbilities)
        {
            if (!LearnedAbility(a))
            {
                r.Add(a);
            }
        }
        return r;
    }
}   