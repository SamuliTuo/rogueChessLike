using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenUnitSlot : MonoBehaviour
{
    public UnitData slottedUnit;
    public Sprite emptyImage;
    public Image expBarFill;
    public GameObject lvlUpPopUp;

    private Image img;
    private GameObject expBar;
    

    public void Init(Sprite emptyImage)
    {
        img = GetComponent<Image>();
        expBar = transform.GetChild(0).gameObject;
        expBarFill = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        lvlUpPopUp = transform.GetChild(1).gameObject;
        this.emptyImage = emptyImage;
        img.sprite = emptyImage;
        expBar.SetActive(false);
        ClearSlot();
    }


    public void SlotAnUnit(UnitData unit)
    {
        print("unit: " + unit + ",  unitName: " + unit.unitName);
        slottedUnit = unit;
        img.sprite = GameManager.Instance.UnitSavePaths.GetImg(unit.unitName);
    }


    public void InitExpBar()
    {
        expBar.SetActive(true);
        expBarFill.fillAmount = slottedUnit.CurrentExpPercent();
    }


    public void ClearSlot()
    {
        slottedUnit = null;
        img.sprite = emptyImage;
    }


    public void OpenLvlUpPopUp()
    {
        lvlUpPopUp.SetActive(true);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(slottedUnit.ability1);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(slottedUnit.ability2);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(slottedUnit.ability3);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(3).GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(slottedUnit.ability4);
    }


    public void ChooseSkill(int slot)
    {
        GetComponentInParent<VictoryPanel>().UpgradeChosen(slot);
    }


    public bool IsEmpty()
    {
        return slottedUnit == null;
    }
}
