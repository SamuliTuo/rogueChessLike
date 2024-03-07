using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEncounterResponseUnitSlot : MonoBehaviour
{
    public UnitData slottedUnit { get; private set; }

    private Image image;

    public void SlotAnUnit(UnitData unit)
    {
        slottedUnit = unit;
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        print("tänne jäi purkkaa!");
        //image.sprite = GameManager.Instance.UnitSavePaths.GetImg(unit.unitName);
    }

    public void ClearSlot()
    {
        slottedUnit = null;
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        image.sprite = null;
    }
    
}
