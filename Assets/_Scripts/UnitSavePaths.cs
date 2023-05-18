using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSavePaths : MonoBehaviour
{
    [System.Serializable]
    public class UnitPath
    {
        public string unit;
        public string path;
    }
    public List<UnitPath> unitPaths = new List<UnitPath>();

    public string GetPath(string unit)
    {
        //print("trying to get path with: " + unit);
        foreach (var unitPath in unitPaths)
        {
            if (unitPath.unit == unit)
            {
                //print("returning unit path: " + unitPath.path + unit);
                return unitPath.path + unit;
            }
        }
        //print("returning a null");
        return null;
    }

    public void GetUnit(string unit)
    {
        //find the correct unit from unitPaths
        foreach (var unitPath in unitPaths)
        {
            if (unitPath.unit == unit)
            {
                unitPath.unit = unit;
            }
        }

    }
}
