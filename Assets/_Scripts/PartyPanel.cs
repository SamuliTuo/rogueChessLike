using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPanel : MonoBehaviour
{
    public List<PartyPanelUnitSlot> unitSlots = new List<PartyPanelUnitSlot>();
    public UnitData chosenUnit;

    [SerializeField] private Sprite emptySlotImage = null;
    [SerializeField] private UnitStatsPanel statsPanel = null;

    private TextEncounterResponseUnitSlot currentSlot;
    private Item itemBeingGranted;
    private Shop shop;
    private bool openedPanelInMap = false;

    public void OpenPartyPanel()
    {
        openedPanelInMap = true;
        currentSlot = null;
        itemBeingGranted = null;
        SlotUnits();
    }

    public void OpenPartyFromTextResponse(TextEncounterResponseUnitSlot slot)
    {
        openedPanelInMap = false;
        currentSlot = slot;
        itemBeingGranted = null;
        SlotUnits();
    }

    public void OpenPartyPanelForGrantingAnItem(Item item, Shop shop)
    {
        this.shop = shop;
        openedPanelInMap = false;
        currentSlot = null;
        itemBeingGranted = item;
        SlotUnits();
        gameObject.SetActive(true);
    }
    

    public void SlotClicked(int slot)
    {
        if (unitSlots[slot].slottedUnit == null)
        {
            print("slot " + slot + " is null");
            return;
        }

        // Choosing unit for text encounter task:
        if (currentSlot != null)
        {
            chosenUnit = unitSlots[slot].slottedUnit;
            if (currentSlot != null && chosenUnit != null)
            {
                currentSlot.SlotAnUnit(chosenUnit);
                gameObject.SetActive(false);
            }
        }

        // Choosing unit for getting an item:
        if (itemBeingGranted != null)
        {
            statsPanel.OpenUnitStatsPanel(unitSlots[slot].slottedUnit, true, itemBeingGranted, shop);
        }
    }

    void SlotUnits()
    {
        var units = GameManager.Instance.PlayerParty.partyUnits;
        for (int i = 0; i < unitSlots.Count; i++)
        {
            if (units != null && units.Count > i && units[i] != null)
            {
                unitSlots[i].SlotUnitHere(emptySlotImage, units[0].Item1);
            }
            else
            {
                unitSlots[i].SlotUnitHere(emptySlotImage, null);
            }
        }
    }

    public void ClosePartyPanel()
    {
        gameObject.SetActive(false);
        if (itemBeingGranted != null)
        {
            shop.CancelPurchase();
        }
        if (openedPanelInMap)
        {
            GameManager.Instance.MapController.SetCanMove(false);
        }
    }
}