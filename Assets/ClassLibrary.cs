using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassLibrary : MonoBehaviour
{
    [Header("A class defines how many stats a unit gains per lvl up. \n\n Every class has total of 8 points in upgrades.")]
    

    public List<Class> classes = new List<Class>();

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
