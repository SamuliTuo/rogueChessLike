using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitAndSavePath
{
    public string savePath;
    public GameObject unitPrefab;
    public Sprite image;
}

public class UnitSavePaths : MonoBehaviour
{
    [Header("Name has to match the unitPrefab's name")]
    public List<UnitAndSavePath> unitsDatas = new List<UnitAndSavePath>();

    public string GetSavePath(string unitName)
    {
        foreach (var ud in unitsDatas)
        {
            if (ud.unitPrefab.name == unitName)
            {
                return ud.savePath + unitName;
            }
        }
        return null;
    }

    public string GetName(string path)
    {
        foreach (var unitPath in unitsDatas)
        {
            if (unitPath.savePath == path)
            {
                return unitPath.unitPrefab.name;
            }
        }
        return null;
    }

    public Sprite GetImg(Unit unit)
    {
        //find the correct unit from spawnableUnits
        foreach (var ud in unitsDatas)
        {
            if (ud.unitPrefab == unit.gameObject)
            {
                return ud.image;
            }
        }
        return null;
    }
    public Sprite GetImg(string unitName)
    {
        foreach (var ud in unitsDatas)
        {
            if (ud.unitPrefab.name == unitName)
            {
                return ud.image;
            }
        }
        return null;
    }
    
}
