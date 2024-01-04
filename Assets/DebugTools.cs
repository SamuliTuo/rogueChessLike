using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    [Header("Caravan:")]
    [Header("  - possible to roll into units:")]
    public bool Squirrel = true;
    public List<UnitAbility> squirrelAbilities = new List<UnitAbility>();
    public bool BearCub = true;
    public bool Penguin = true;
    public bool BlackLion = true;

    /*
    private void Update()
    {
        foreach (var unit in GameManager.Instance.UnitSavePaths.unitsDatas)
        {
            if (unit.unitPrefab.name == "Unit_squirrel")
            {
                foreach (var ability in unit.unitPrefab.GetComponent<UnitAbilityManager>().possibleAbilities)
                {
                    squirrelAbilities.Add(ability);
                }
            }
        }
    }
    */

    public static DebugTools Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}