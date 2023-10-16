using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPanel : MonoBehaviour
{
    public List<PartyPanelUnitSlot> unitSlots = new List<PartyPanelUnitSlot>();
    public UnitData chosenUnit;

    [SerializeField] private Sprite emptySlotImage = null;

    private TextEncounterResponseUnitSlot currentSlot;

    public void OpenPartyPanel()
    {
        currentSlot = null;
        SlotUnits();
    }

    public void OpenPartyFromTextResponse(TextEncounterResponseUnitSlot slot)
    {
        currentSlot = slot;
        SlotUnits();
    }

    public void ChooseUnit(int slot)
    {
        if (unitSlots[slot].slottedUnit == null)
        {
            return;
        }
        chosenUnit = unitSlots[slot].slottedUnit;
        if (currentSlot != null && chosenUnit != null)
        {
            currentSlot.SlotAnUnit(chosenUnit);
            gameObject.SetActive(false);
        }
    }

    void SlotUnits()
    {
        var units = GameManager.Instance.PlayerParty.partyUnits;
        for (int i = 0; i < unitSlots.Count; i++)
        {
            if (units != null && units.Count > i && units[i] != null)
            {
                unitSlots[i].SlotUnitHere(emptySlotImage, units[0]);
            }
            else
            {
                unitSlots[i].SlotUnitHere(emptySlotImage, null);
            }
        }
    }
}