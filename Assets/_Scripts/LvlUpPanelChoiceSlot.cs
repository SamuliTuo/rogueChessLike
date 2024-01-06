using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class LvlUpPanelChoiceSlot : MonoBehaviour
{
    private UnitAbility abi;
    private LevelUpPanel.StatUpgrade upgrade;
    private AbilityUpgrade upgradeObj;
    private GameObject newAbilitySign;

    private GameObject passive_epic, passive_rare, passive_doubleL, passive_doubleR;
    private Image bigImage;

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
        GetPassiveGameObjects();
        ResetSlots();
        this.upgrade = upgrade;
        if (upgrade.sprite2 != null)
        {
            passive_doubleR.GetComponent<Image>().sprite = upgrade.sprite2;
            passive_doubleL.GetComponent<Image>().sprite = upgrade.sprite1;
            passive_doubleR.SetActive(true);
            passive_doubleL.SetActive(true);
        }
        else if (upgrade.stat1Amount == 1)
        {
            bigImage.sprite = upgrade.sprite1;
        }
        else if (upgrade.stat1Amount == 1.5f)
        {
            passive_rare.gameObject.SetActive(true);
            bigImage.sprite = upgrade.sprite1;
        }
        else if (upgrade.stat1Amount == 2.25f)
        {
            passive_epic.gameObject.SetActive(true);
            bigImage.sprite = upgrade.sprite1;
        }
        else if (upgrade.skipPassive)
        {
            bigImage.sprite = upgrade.skipPassiveSprite;
        }
    }

    void GetPassiveGameObjects()
    {
        bigImage = GetComponent<Image>();
        bigImage.sprite = null;
        passive_rare = transform.GetChild(0).gameObject;
        passive_epic = transform.GetChild(1).gameObject;
        passive_doubleL = transform.GetChild(2).gameObject;
        passive_doubleR = transform.GetChild(3).gameObject;
        passive_rare.SetActive(false);
        passive_epic.SetActive(false);
        passive_doubleL.SetActive(false);
        passive_doubleR.SetActive(false);
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
