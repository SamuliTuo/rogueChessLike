using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LvlUpPanelChoiceSlot : MonoBehaviour
{
    private UnitAbility abi;
    private string upgrade;
    private AbilityUpgrade upgradeObj;

    // New ability
    public void SetChoice(UnitAbility abi)
    {
        ResetSlots();
        this.abi = abi;
        GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(abi);
    }

    // Upgrade existing ability
    public void SetChoice(AbilityUpgrade upgrade)
    {
        ResetSlots();
        upgradeObj = upgrade;
        GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(upgrade.ability);
    }

    // Stat upgrade
    public void SetChoice(Tuple<string, Sprite> upgrade)
    {
        ResetSlots();
        this.upgrade = upgrade.Item1;
        GetComponent<Image>().sprite = upgrade.Item2;
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
