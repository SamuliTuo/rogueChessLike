using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class ClassLibrary : MonoBehaviour
{
    [Header("A class defines how many stats a unit gains per lvl up. \n\nGive each class a total of 8 points in upgrades. Maybe? idk.")]
    [Space(10)]
    public List<Class> classes = new List<Class>();
    public List<Prefix> prefixes = new List<Prefix>();
    public List<Suffix> suffixes = new List<Suffix>();

    [Header("Global multipliers for actual stats-per level of stat:")]
    public float hpPerPoint = 1.0f;
    public float dmgPerPoint = 1.0f;
    public float magicPerPoint = 1.0f;
    public float moveSpdPerPoint = 1.0f;
    public float attSpdPerPoint = 1.0f;
    public float armorPerPoint = 1.0f;
    public float mgArmorPerPoint = 1.0f;

    public Class GetRandomClass() { return classes[UnityEngine.Random.Range(0, classes.Count)]; }
    public Prefix GetRandomPrefix() { return prefixes[UnityEngine.Random.Range(0, prefixes.Count)]; }
    public Suffix GetRandomSuffix() { return suffixes[UnityEngine.Random.Range(0, suffixes.Count)]; }
    public Class GetRandomClass(Class excluded)
    {
        var l = new List<Class>();
        foreach (var c in classes)
        {
            if (c != excluded)
            {
                l.Add(c);
            }
        }
        return l[UnityEngine.Random.Range(0, l.Count)];
    }
    public Prefix GetRandomPrefix(Prefix excluded)
    {
        var l = new List<Prefix>();
        foreach (var c in prefixes)
        {
            if (c != excluded)
            {
                l.Add(c);
            }
        }
        return l[UnityEngine.Random.Range(0, l.Count)];
    }
    public Suffix GetRandomSuffix(Suffix excluded)
    {
        var l = new List<Suffix>();
        foreach (var c in suffixes)
        {
            if (c != excluded)
            {
                l.Add(c);
            }
        }
        return l[UnityEngine.Random.Range(0, l.Count)];
    }
}


[Serializable]
public class Class
{
    [Header("Gain per lvl:")]
    public string name;
    public Sprite shieldImage;
    public float hp;
    public float dmg;
    public float mgDmg;
    public float moveSpd;
    public float attSpd;
    public float armor;
    public float mgArmor;

    public Class(string name, Sprite shieldImage, float hp, float dmg, float mgDmg, float moveSpd, float attSpd, float armor, float mgArmor)
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

[Serializable]
public class Prefix
{
    [Header("Additional gains per level")]
    public string name;
    public Sprite coatOfArms;
    public float hp;
    public float dmg;
    public float mgDmg;
    public float moveSpd;
    public float attSpd;
    public float armor;
    public float mgArmor;
    public Prefix(string name, Sprite coatOfArms, float hp, float dmg, float mgDmg, float moveSpd, float attSpd, float armor, float mgArmor)
    {
        this.name = name;
        this.coatOfArms = coatOfArms;
        this.hp = hp;
        this.dmg = dmg;
        this.mgDmg = mgDmg;
        this.moveSpd = moveSpd;
        this.attSpd = attSpd;
        this.armor = armor;
        this.mgArmor = mgArmor;
    }
}

[Serializable]
public class Suffix
{
    [Header("Additional gains per level")]
    public string name;
    public Sprite laurels;
    public float hp;
    public float dmg;
    public float mgDmg;
    public float moveSpd;
    public float attSpd;
    public float armor;
    public float mgArmor;
    public Suffix(string name, Sprite laurels, float hp, float dmg, float mgDmg, float moveSpd, float attSpd, float armor, float mgArmor)
    {
        this.name = name;
        this.laurels = laurels;
        this.hp = hp;
        this.dmg = dmg;
        this.mgDmg = mgDmg;
        this.moveSpd = moveSpd;
        this.attSpd = attSpd;
        this.armor = armor;
        this.mgArmor = mgArmor;
    }
}