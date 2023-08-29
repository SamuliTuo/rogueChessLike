using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LvlUpPanelChoiceSlot : MonoBehaviour
{
    private UnitAbility abi;
    private string attribute;
    private float attributeChange;

    public void SetChoice(UnitAbility abi)
    {
        this.abi = abi;
        GetComponent<Image>().sprite = GameManager.Instance.AbilityLibrary.GetImg(abi);
    }

    public void SetChoice(string attribute, float attributeChange, Sprite img)
    {
        this.attribute = attribute;
        this.attributeChange = attributeChange;
        GetComponent<Image>().sprite = img;
    }

    public void ChooseThis()
    {
        GetComponentInParent<LevelUpPanel>().ChooseOption(abi);
        gameObject.SetActive(false);
    }
}
