using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenUnitSlot : MonoBehaviour
{
    public Sprite emptyImage;

    private Unit slottedUnit;
    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    public void Init(Sprite emptyImage)
    {
        this.emptyImage = emptyImage;
        if (img == null)
            img = GetComponent<Image>();

        img.sprite = emptyImage;
    }

    public void SlotAnUnit(Unit unit)
    {
        slottedUnit = unit;
        img.sprite = GameManager.Instance.UnitSavePaths.GetImg(unit);
    }

    public void ClearSlot()
    {
        slottedUnit = null;
        img.sprite =  emptyImage;
    }

    public bool IsEmpty()
    {
        return slottedUnit == null;
    }
}
