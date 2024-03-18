using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UnitLibrary : MonoBehaviour
{

    [Header("____ Default Stats:  ___________________________________________________________________________________________________________________________________________")]
    [Space(10)]
    public float hp = 150;
    public float damage = 20;
    public float magicDamage = 10;
    [Range(0,1)] public float critChance = 0.2f;
    public float critDamage = 1.4f;
    public float missChance = 0;

    public float moveSpeed = 10;
    public float visibleMoveSpeed = 100;
    public float attackSpeed = 20;
    public float armor = 10;
    public float magicRes = 10;

    [Space(22)]
    [Header("____ Units: ________________________________________________________________________________________________________________________________________________________________________")]
    [Space(10)]
    [SerializeField] public List<UnitInLibrary> playerUnits = new List<UnitInLibrary>();
    [SerializeField] public List<UnitInLibrary> enemyUnits = new List<UnitInLibrary>();
    [SerializeField] public List<UnitInLibrary> boardObjects = new List<UnitInLibrary>();

    
    private void Awake()
    {
        SetAverageStatsToUnits(playerUnits);
        SetAverageStatsToUnits(enemyUnits);
        SetAverageStatsToUnits(boardObjects);
    }

    void SetAverageStatsToUnits(List<UnitInLibrary> units)
    {
        foreach (UnitInLibrary unit in units)
        {
            unit.stats.hp = unit.stats.hp != -1 ? unit.stats.hp : hp;
            unit.stats.damage = unit.stats.damage != -1 ? unit.stats.damage : damage;
            unit.stats.magicDamage = unit.stats.magicDamage != -1 ? unit.stats.magicDamage : magicDamage;
            unit.stats.critChance = unit.stats.critChance != -1 ? unit.stats.critChance : critChance;
            unit.stats.critDamage = unit.stats.critDamage != -1 ? unit.stats.critDamage : critDamage;
            unit.stats.missChance = unit.stats.missChance != -1 ? unit.stats.missChance : missChance;
            unit.stats.attackSpeed = unit.stats.attackSpeed != -1 ? unit.stats.attackSpeed : attackSpeed;
            unit.stats.moveSpeed = unit.stats.moveSpeed != -1 ? unit.stats.moveSpeed : moveSpeed;
            unit.stats.visibleMoveSpeed = unit.stats.visibleMoveSpeed != -1 ? unit.stats.visibleMoveSpeed : visibleMoveSpeed;
            unit.stats.armor = unit.stats.armor != -1 ? unit.stats.armor : armor;
            unit.stats.magicRes = unit.stats.magicRes != -1 ? unit.stats.magicRes : magicRes;
        }
    }

    public UnitInLibrary GetUnit(string prefabName)
    {
        UnitInLibrary unit = null;
        unit = CheckUnitList(playerUnits, prefabName);
        if (unit == null)
        {
            unit = CheckUnitList(enemyUnits, prefabName);
        }
        if (unit == null)
        {
            unit = CheckUnitList(boardObjects, prefabName);
        }
        return unit;
    }
    public UnitInLibrary GetUnitFromListedName(string nameInList)
    {
        UnitInLibrary unit = null;
        unit = CheckUnitListWithListedName(playerUnits, nameInList);
        if (unit == null)
        {
            unit = CheckUnitListWithListedName(enemyUnits, nameInList);
        }
        if (unit == null)
        {
            unit = CheckUnitListWithListedName(boardObjects, nameInList);
        }
        return unit;
    }
    private UnitInLibrary CheckUnitList(List<UnitInLibrary> list, string name)
    {
        foreach (UnitInLibrary unit in list)
        {
            if (unit.prefab == null)
                continue;

            if (unit.prefab.name == name)
                return unit;
        }
        return null;
    }
    private UnitInLibrary CheckUnitListWithListedName(List<UnitInLibrary> list, string name)
    {
        foreach (UnitInLibrary unit in list)
        {
            if (unit.nameInList == name)
            {
                return unit;
            }
        }
        return null;
    }
    public Sprite GetSpellSymbol(UnitAbility ability)
    {
        Sprite img = CheckListForSpell(playerUnits, ability);
        if (img == null)
        {
            CheckListForSpell(enemyUnits, ability);
        }
        return img;
    }
    Sprite CheckListForSpell(List<UnitInLibrary> units, UnitAbility ability)
    {
        if (ability == null) 
            return null;

        foreach (UnitInLibrary unit in units)
        {
            foreach (LibraryAbility item in unit.signatureSpells)
            {
                if (item.spell != null)
                {
                    if (item.spell.name == ability.name)
                        return item.image;
                }
            }
            foreach (LibraryAbility item in unit.supportSpells)
            {
                if (item.spell != null)
                {
                    if (item.spell.name == ability.name)
                        return item.image;
                }
            }
            foreach (LibraryAbility item in unit.ultimateSpells)
            {
                if (item.spell != null)
                {
                    if (item.spell.name == ability.name)
                        return item.image;
                }
            }
        }
        return null;
    }
}




[Serializable]
public class UnitInLibrary
{
    public string nameInList;
    public int id;
    public GameObject prefab;
    public Sprite image;
    public StartingStats stats;
    [Space(15)]
    public List<LibraryAttack> attacks;
    public bool randomizeAttackingOrder;
    public List<LibraryAbility> signatureSpells;
    public List<LibraryAbility> supportSpells;
    public List<LibraryAbility> ultimateSpells;

    public UnitInLibrary(string nameInList, int id, GameObject prefab, Sprite item, List<LibraryAttack> attacks, bool randomizeAttackingOrder, List<LibraryAbility> signatureSpells, List<LibraryAbility> supportSpells, List<LibraryAbility> ultimateSpells)
    {
        this.nameInList = nameInList;
        this.id = id;
        this.prefab = prefab;
        this.image = item;
        this.attacks = attacks;
        this.randomizeAttackingOrder = randomizeAttackingOrder;
        this.signatureSpells = signatureSpells;
        this.supportSpells = supportSpells;
        this.ultimateSpells = ultimateSpells;
    }
    public UnitInLibrary(UnitInLibrary u)
    {
        nameInList = u.nameInList;
        prefab = u.prefab;
        image = u.image;
        attacks = u.attacks;
        randomizeAttackingOrder = u.randomizeAttackingOrder;
        signatureSpells = u.signatureSpells;
        supportSpells = u.supportSpells;
        ultimateSpells = u.ultimateSpells;
    }

    public string GetSavePath()
    {
        var prefabPath = AssetDatabase.GetAssetPath(prefab);
        string string1 = "Assets/Resources/";
        string string2 = ".prefab";
        prefabPath = prefabPath.Replace(string1, "");
        prefabPath = prefabPath.Replace(string2, "");
        return prefabPath;
    }
    public GameObject GetAttackProjectile(Unit_NormalAttack attack)
    {
        Debug.Log(attack);
        foreach (var item in attacks)
            if (item.attack == attack) 
                return item.projectile;

        return null;
    }
    public GameObject GetAbilityProjectile(UnitAbility ability)
    {
        if (ability == null) 
            return null;

        foreach (LibraryAbility item in signatureSpells)
        {
            if (item.spell == null) 
                continue;
            if (item.spell.name == ability.name)
                return item.projectile;
        }
        foreach (LibraryAbility item in supportSpells)
        {
            if (item.spell == null) 
                continue;
            if (item.spell.name == ability.name)
                return item.projectile;
        }
        foreach (LibraryAbility item in ultimateSpells)
        {
            if (item.spell == null) 
                continue;
            if (item.spell.name == ability.name)
                return item.projectile;
        }
        return null;
    }

    public Sprite GetSpellImage(UnitAbility ability)
    {
        foreach (LibraryAbility item in signatureSpells)
        {
            if (item.spell == null) 
                continue;
            if (item.spell.name == ability.name)
                return item.image;
        }
        foreach (LibraryAbility item in supportSpells)
        {
            if (item.spell == null) 
                continue;
            if (item.spell.name == ability.name)
                return item.image;
        }
        foreach (LibraryAbility item in ultimateSpells)
        {
            if (item.spell == null) 
                continue;
            if (item.spell.name == ability.name) 
                return item.image;
        }
        return null;
    }
}

[Serializable]
public class LibraryAttack
{
    public string name;
    public Unit_NormalAttack attack;
    public GameObject projectile;
    public LibraryAttack(string name, Unit_NormalAttack attack, GameObject projectile) { this.name = name;  this.attack = attack; this.projectile = projectile; }
}

[Serializable]
public class LibraryAbility
{
    public string name;
    public UnitAbility spell;
    public GameObject projectile;
    public Sprite image;
    public LibraryAbility(string name, UnitAbility spell, Sprite image) { this.name = name; this.spell = spell; this.image = image; }
}

[Serializable]
public class StartingStats
{
    [Header("Leaving the stat values -1 will default them.")]
    [Space(10)]
    public float hp = -1;
    public float damage = -1;
    public float magicDamage = -1;
    [Range(0, 1f)] public float critChance = -1;
    public float critDamage = -1;
    public float missChance = -1;
    public float moveSpeed = -1;
    public float visibleMoveSpeed = -1;
    public float attackSpeed = -1;
    public float armor = -1;
    public float magicRes = -1;
    public StartingStats(bool useAverageStats, float hp, float dmg, float mgDmg, float critChance, float critDamage, float missChance, float moveSpd, float visibleMoveSpeed, float attSpd, float armor, float mRes) 
    { 
        this.hp = hp; 
        damage = dmg; 
        magicDamage = mgDmg; 
        this.critChance = critChance; 
        this.critDamage = critDamage; 
        this.missChance = missChance; 
        moveSpeed = moveSpd; 
        this.visibleMoveSpeed = visibleMoveSpeed;
        attackSpeed = attSpd; 
        this.armor = armor; 
        this.magicRes = mRes; 
    }
}



