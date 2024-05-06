using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class LvlUpPanelChoiceSlot : MonoBehaviour
{
    private LevelUpPanel lvlUpControl;

    private UnitAugment augment;
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

    // Augment
    public void SetChoice(UnitAugments.Augment augment, LevelUpPanel lvlUpper, int slot)
    {
        currentSlot = slot;
        lvlUpControl = lvlUpper;
        ResetSlots();
        this.augment = augment.augmentType;
        ToggleNewAbilitySign(false);
        GetComponent<Image>().sprite = augment.image;
    }

    // New ability
    public void SetChoice(UnitAbility abi, LevelUpPanel lvlUpper, int slot)
    {
        currentSlot = slot;
        lvlUpControl = lvlUpper;
        ResetSlots();
        this.abi = abi;
        ToggleNewAbilitySign(true);
        GetComponent<Image>().sprite = GameManager.Instance.UnitLibrary.GetSpellSymbol(abi);
    }

    int currentSlot = -1;
    // Upgrade ability (unused)
    public void SetChoice(AbilityUpgrade upgrade, LevelUpPanel lvlUpper, int slot)
    {
        currentSlot = slot;
        lvlUpControl = lvlUpper;
        ResetSlots();
        ToggleNewAbilitySign(false);
        upgradeObj = upgrade;
        GetComponent<Image>().sprite = GameManager.Instance.UnitLibrary.GetSpellSymbol(upgrade.ability);
    }

    // Stat upgrade
    public void SetChoice(LevelUpPanel.StatUpgrade upgrade, LevelUpPanel lvlUpper)
    {
        lvlUpControl = lvlUpper;
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
        print("choosing option "+currentSlot);
        if (lvlUpControl == null)
            return;

        if (this.abi != null)
        {
            print("choose ability");
            lvlUpControl.TryToChooseOption(abi, currentSlot);
        }
        else if (this.upgrade != null)
        {
            print("choose upgrade");
            lvlUpControl.TryToChooseOption(upgrade);
        }
        else if (this.upgradeObj != null)
        {
            print("choose upgrade 2");
            lvlUpControl.TryToChooseOption(upgradeObj, currentSlot);
        }
        else
        {
            print("choose augment");
            lvlUpControl.TryToChooseOption(augment, currentSlot);
        }
    }
}
