using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitDataLoader : EditorWindow
{
    [MenuItem("Tools/UnitDataLoader")]
    public static void ShowWindow()
    {
        GetWindow<UnitDataLoader>("UnitDataLoader");
    }

    void OnGUI()
    {
        GUILayout.Label("Reload unit database", EditorStyles.boldLabel);
        if (GUILayout.Button("Reload Items"))
        {
            var obj = GameObject.Find("Units");
            obj.GetComponent<LoadExcel>().LoadUnitData(obj.GetComponent<UnitLibrary>());
        }
    }
}
