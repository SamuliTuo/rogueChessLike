using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LvlUpPanelChoiceSlot : MonoBehaviour
{
    private UnitAbility abi;
    private LevelUpPanel.StatUpgrade upgrade;
    private AbilityUpgrade upgradeObj;
    private GameObject newAbilitySign;

    private void ToggleNewAbilitySign(bool state)
    {
        if (newAbilitySign == null)
            newAbilitySign = transform.GetChild(1).gameObject;

        newAbilitySign?.SetActive(state);
    }
    // New ability
    public void SetChoice(UnitAbility abi)
    {
        ResetSlots();
        this.abi = abi;
        ToggleNewAbilitySign(true);
        GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(abi);
    }

    // Upgrade existing ability
    public void SetChoice(AbilityUpgrade upgrade)
    {
        ResetSlots();
        ToggleNewAbilitySign(false);
        upgradeObj = upgrade;
        GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(upgrade.ability);
    }

    // Stat upgrade
    public void SetChoice(LevelUpPanel.StatUpgrade upgrade)
    {
        ResetSlots();
        this.upgrade = upgrade;
        if (upgrade.sprite2 != null)
        {
            print("setting up double upgrade");
            transform.GetChild(3).GetComponent<Image>().sprite = upgrade.sprite2;
            transform.GetChild(2).GetComponent<Image>().sprite = upgrade.sprite1;
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (upgrade.stat1Amount == 1)
        {
            GetComponent<Image>().sprite = upgrade.sprite1;
        }
        else if (upgrade.stat1Amount == 1.5f)
        {
            print("settin up rare upgrade");
            transform.GetChild(0).gameObject.SetActive(true);
            GetComponent<Image>().sprite = upgrade.sprite1;
        }
        else if (upgrade.stat1Amount == 2.25f)
        {
            print("settin up abin upgrade");
            transform.GetChild(1).gameObject.SetActive(true);
            GetComponent<Image>().sprite = upgrade.sprite1;
        }
        else if (upgrade.skipPassive)
        {
            print("set up skip upgrade");
            GetComponent<Image>().sprite = upgrade.skipPassiveSprite;
        }
    }


    void ResetSlots()
    {
        this.abi = null;
        this.upgrade = null;
        this.upgradeObj = null;
    }

    
    public void ChooseThis()
    {
        if (this.abi != null)
        {
            if (GetComponentInParent<LevelUpPanel>().TryToChooseOption(abi))
            {
                gameObject.SetActive(false);
            }
        }
        else if (this.upgrade != null)
        {
            if (GetComponentInParent<LevelUpPanel>().TryToChooseOption(upgrade))
            {
                gameObject.SetActive(false);
            }
        }
        else if (this.upgradeObj != null)
        {
            if (GetComponentInParent<LevelUpPanel>().TryToChooseOption(upgradeObj))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
