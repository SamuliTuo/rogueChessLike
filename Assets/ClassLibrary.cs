using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassLibrary : MonoBehaviour
{
    [Header("A class defines how many stats a unit gains per lvl up. \n\n Give each class a total of 8 points in upgrades.")]
    [Space(10)]
    public List<Class> classes = new List<Class>();

    [Header("Multipliers for stats-per level:")]
    public float hpPerPoint = 1.0f;
    public float dmgPerPoint = 1.0f;
    public float magicPerPoint = 1.0f;
    public float moveSpdPerPoint = 1.0f;
    public float attSpdPerPoint = 1.0f;
    public float armorPerPoint = 1.0f;
    public float mgArmorPerPoint = 1.0f;

}

[Serializable]
public class Class
{
    [Header("Gain per lvl:")]
    public string name;
    public float hp;
    public float dmg;
    public float mgDmg;
    public float moveSpd;
    public float attSpd;
    public float armor;
    public float mgArmor;

    public Class(string name, float hp, float dmg, float mgDmg, float moveSpd, float attSpd, float armor, float mgArmor)
    {
        this.name = name;
        this.hp = hp;
        this.dmg = dmg;
        this.mgDmg = mgDmg;
        this.moveSpd = moveSpd;
        this.attSpd = attSpd;
        this.armor = armor;
        this.mgArmor = mgArmor;
    }
}
