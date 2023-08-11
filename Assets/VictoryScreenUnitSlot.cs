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
    
    private void Start()
    {
        img = GetComponent<Image>();
        expBar = transform.GetChild(0).gameObject;
        expBarFill = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        lvlUpPopUp = transform.GetChild(1).gameObject;
    }

    public void Init(Sprite emptyImage)
    {
        Start();
        this.emptyImage = emptyImage;
        img.sprite = emptyImage;
        expBar.SetActive(false);
    }

    public void SlotAnUnit(UnitData unit)
    {
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
        img.sprite =  emptyImage;
    }

    public void OpenLvlUpPopUp()
    {
        lvlUpPopUp.SetActive(true);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(0).GetComponent<Image>().sprite = GetComponentInParent<VictoryPanel>().abilityImg_PLACEHOLDER;
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(1).GetComponent<Image>().sprite = GetComponentInParent<VictoryPanel>().abilityImg_PLACEHOLDER;
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(2).GetComponent<Image>().sprite = GetComponentInParent<VictoryPanel>().abilityImg_PLACEHOLDER;
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(3).GetComponent<Image>().sprite = GetComponentInParent<VictoryPanel>().abilityImg_PLACEHOLDER;
    }

    public void ChooseSkill()
    {
        print("asdasd");
        GetComponentInParent<VictoryPanel>().UpgradeChosen(0);
    }

    public bool IsEmpty()
    {
        return slottedUnit == null;
    }
}
