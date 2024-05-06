using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyPanelUnitSlot : MonoBehaviour
{
    public UnitData slottedUnit { get; private set; }
    
    private Image img;
    private Image expBar;
    private Image[] itemSlots = new Image[3];


    public void SlotUnitHere(Sprite emptySlotImage, UnitData unit)
    {
        // Get references
        if (img == null)
        {
            img = GetComponent<Image>();
        }
        if (expBar == null)
        {
            expBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        }
        if (itemSlots[0] == null)
        {
            itemSlots[0] = transform.GetChild(1).GetChild(0).GetComponent<Image>();
            itemSlots[1] = transform.GetChild(1).GetChild(1).GetComponent<Image>();
            itemSlots[2] = transform.GetChild(1).GetChild(2).GetComponent<Image>();
        }

        // Display unit image
        slottedUnit = unit;
        if (unit == null)
        {
            img.sprite = emptySlotImage;
            expBar.transform.parent.gameObject.SetActive(false);
            return;
        }
        img.sprite = GameManager.Instance.UnitLibrary.GetUnit(unit).image;

        // Display Experience
        expBar.transform.parent.gameObject.SetActive(true);
        expBar.fillAmount = unit.currentExperience / unit.nextLevelExperience;

        // Display items
        if (slottedUnit.itemSlots_itemID[0] != -1)
        {
            itemSlots[0].sprite = GameManager.Instance.ItemLibrary.GetItemWithID(slottedUnit.itemSlots_itemID[0]).icon;
            itemSlots[0].gameObject.SetActive(true);
        }
        else
        {
            itemSlots[0].gameObject.SetActive(false);
        }

        if (slottedUnit.itemSlots_itemID[1] != -1)
        {
            itemSlots[1].sprite = GameManager.Instance.ItemLibrary.GetItemWithID(slottedUnit.itemSlots_itemID[1]).icon;
            itemSlots[1].gameObject.SetActive(true);
        }
        else
        {
            itemSlots[1].gameObject.SetActive(false);
        }

        if (slottedUnit.itemSlots_itemID[2] != -1)
        {
            itemSlots[2].sprite = GameManager.Instance.ItemLibrary.GetItemWithID(slottedUnit.itemSlots_itemID[2]).icon;
            itemSlots[2].gameObject.SetActive(true);
        }
        else
        {
            itemSlots[2].gameObject.SetActive(false);
        }
    }
}
