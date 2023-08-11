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
    }

    public float CurrentExpPercent()
    {
        return currentExperience / nextLevelExperience;
    }
}   