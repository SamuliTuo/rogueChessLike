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
    public GameObject lvlUpSign;

    private Image img;
    private GameObject expBar;
    public bool lvlUpPending { get; set; }
    

    public void Init(Sprite emptyImage)
    {
        img = GetComponent<Image>();
        expBar = transform.GetChild(0).gameObject;
        expBarFill = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        lvlUpPopUp = transform.GetChild(1).gameObject;
        lvlUpSign = transform.GetChild(2).gameObject;
        this.emptyImage = emptyImage;
        img.sprite = emptyImage;
        expBar.SetActive(false);
        ClearSlot();
    }


    public void SlotAnUnit(UnitData unit)
    {
        slottedUnit = unit;
        lvlUpPending = false;
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

    public void SlotLevelUp()
    {
        lvlUpPending = true;
        lvlUpSign.SetActive(true);
    }

    public void OpenLvlUpPopUp()
    {
        if (!lvlUpSign.activeSelf)
            return;

        if (GetComponentInParent<VictoryPanel>().OpenLvlUpPopUp(this))
            lvlUpPending = false; //t‰‰ on placeholderina sille, ett‰ kun on oikeesti tehny level upin hahmolle niin pending vaihtuu "false"
        


        
        /*
        lvlUpPopUp.SetActive(true);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(slottedUnit.ability1);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(slottedUnit.ability2);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(slottedUnit.ability3);
        if (slottedUnit.ability1 != null)
            lvlUpPopUp.transform.GetChild(3).GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(slottedUnit.ability4);
        */
    }


    public void ChooseSkill(int slot)
    {
        lvlUpPopUp.SetActive(false);
        GetComponentInParent<VictoryPanel>().UpgradeChosen(this, slot);
    }


    public bool IsEmpty()
    {
        return slottedUnit == null;
    }
}
