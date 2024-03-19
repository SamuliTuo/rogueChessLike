#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

public class LoadExcel : MonoBehaviour
{
    public UnitInLibrary blankUnit;

    private UnitLibrary library;

    [MenuItem("Tools/LoadUnitStats")]
    public void LoadUnitData(UnitLibrary library)
    {
        this.library = library;

        // Read CSV files
        List<Dictionary<string, object>> data = CSVReader.Read("CharacterSheets");
        for (var i = 0; i < data.Count; i++)
        {
            int id = int.Parse(data[i]["id"].ToString(), System.Globalization.NumberStyles.Integer);
            string name = data[i]["name"].ToString();
            float hp = float.Parse(data[i]["hp"].ToString(), CultureInfo.InvariantCulture);
            float physDmg = float.Parse(data[i]["physDmg"].ToString(), CultureInfo.InvariantCulture);
            float magicDmg = float.Parse(data[i]["magicDmg"].ToString(), CultureInfo.InvariantCulture);
            float critChance = float.Parse(data[i]["critChance"].ToString(), CultureInfo.InvariantCulture);
            critChance = Mathf.Clamp(critChance, 0f, 1f);
            float critDmg = float.Parse(data[i]["critDmg"].ToString(), CultureInfo.InvariantCulture);
            float missChance = float.Parse(data[i]["missChance"].ToString(), CultureInfo.InvariantCulture);
            float moveSpd = float.Parse(data[i]["moveSpd"].ToString(), CultureInfo.InvariantCulture);
            float moveSpdVisual = float.Parse(data[i]["moveSpdVisual"].ToString(), CultureInfo.InvariantCulture);
            float attSpd = float.Parse(data[i]["attSpd"].ToString(), CultureInfo.InvariantCulture);
            float armor = float.Parse(data[i]["armor"].ToString(), CultureInfo.InvariantCulture);
            float mres = float.Parse(data[i]["mres"].ToString(), CultureInfo.InvariantCulture);

            AddItem(id, name, hp, physDmg, magicDmg, critChance, critDmg, missChance, moveSpd, moveSpdVisual, attSpd, armor, mres);
        }
    }

    void AddItem(int id, string name, float hp, float physDmg, float magicDmg, float critChance, float critDmg, float missChance, float moveSpd, float moveSpdVisual, float attSpd, float armor, float mres)
    {
        UnitInLibrary unit = CheckIfLibraryHasID(id);

        // Make a new library entry if the ID doesn't exist:
        if (unit == null)
        {
            unit = new UnitInLibrary(blankUnit);
            unit.id = id;
            if (id >= 0 && id < 100)
            {
                library.playerUnits.Add(unit);
            }
            else if (id >= 100 && id < 1000)
            {
                library.enemyUnits.Add(unit);
            }
            else if (id > 1000)
            {
                library.boardObjects.Add(unit);
            }
        }

        unit.nameInList = name;
        unit.stats = new StartingStats(false, hp, physDmg, magicDmg, critChance, critDmg, missChance, moveSpd, moveSpdVisual, attSpd, armor, mres);
    }


    UnitInLibrary CheckIfLibraryHasID(int id)
    {
        foreach (UnitInLibrary p in library.playerUnits)
        {
            if (p.id == id)
            {
                return p;
            }
        }
        foreach (UnitInLibrary e in library.enemyUnits)
        {
            if (e.id == id)
            {
                return e;
            }
        }
        foreach (UnitInLibrary o in library.boardObjects)
        {
            if (o.id == id)
            {
                return o;
            }
        }
        return null;
    }
}
#endif