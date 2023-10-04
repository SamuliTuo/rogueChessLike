using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyPanelUnitSlot : MonoBehaviour
{
    public UnitData slottedUnit { get; private set; }

    private Image img;
    private Image expBar;


    public void SlotUnitHere(Sprite emptySlotImage, UnitData unit)
    {
        img = GetComponent<Image>();
        expBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        slottedUnit = unit;
        if (unit == null)
        {
            img.sprite = emptySlotImage;
            expBar.transform.parent.gameObject.SetActive(false);
            return;
        }

        img.sprite = GameManager.Instance.UnitSavePaths.GetImg(unit.unitName);
        expBar.transform.parent.gameObject.SetActive(true);
        expBar.fillAmount = unit.currentExperience / unit.nextLevelExperience;
    }
}
