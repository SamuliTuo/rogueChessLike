using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityInLibrary
{
    public UnitAbility ability;
    public Sprite image;
}
[System.Serializable]
public class UpgradeSymbols
{
    public AbilityUpgradeType type;
    public Sprite image;
}

public class AbilityLibrary : MonoBehaviour
{
    public List <UpgradeSymbols> upgradeSymbols = new List <UpgradeSymbols>();
    public List<AbilityInLibrary> abilitiesInLibrary = new List<AbilityInLibrary>();

    
    public Sprite GetUpgradeSymbol(AbilityUpgradeType type)
    {
        foreach (var item in upgradeSymbols)
        { 
            if (item.type == type)
            {
                return item.image;
            }
        }
        return null;
    }
    //public Sprite GetImg(UnitAbility ability)
    //{
    //    foreach (var item in abilitiesInLibrary)
    //    {
    //        if (item.ability.name == ability.name)
    //        {
    //            return item.image;
    //        }
    //    }
    //    return null;
    //}
}
