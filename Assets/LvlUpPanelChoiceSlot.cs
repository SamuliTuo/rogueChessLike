using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LvlUpPanelChoiceSlot : MonoBehaviour
{
    private UnitAbility abi;
    private string upgrade;

    public void SetChoice(UnitAbility abi)
    {
        this.abi = abi;
        GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(abi);
    }

    public void SetChoice(Tuple<string, Sprite> upgrade)
    {
        this.upgrade = upgrade.Item1;
        GetComponent<Image>().sprite = upgrade.Item2;
    }

    public void ChooseThis()
    {
        if (this.abi != null)
            GetComponentInParent<LevelUpPanel>().ChooseOption(abi);
        else if (this.upgrade != null)
            GetComponentInParent<LevelUpPanel>().ChooseOption(upgrade);
        
        gameObject.SetActive(false);
    }
}
