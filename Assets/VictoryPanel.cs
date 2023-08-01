using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private Sprite emptySlotImage = null;
    public List<VictoryScreenUnitSlot> unitSlots = new List<VictoryScreenUnitSlot>();

    public void InitVictoryScreen()
    {
        StartPanel();

        float exp = GameManager.Instance.currentFightCumulatedExperience;
        Unit[,] units = GameManager.Instance.PlayerParty.partyUnits;
        for (int x = 0; x < units.GetLength(0); x++)
        {
            for (int y = 0; y < units.GetLength(1); y++)
            {
                if (units[x, y] != null)
                {
                    var slot = FirstFreeSlot();
                    if (slot != null)
                    {
                        slot.SlotAnUnit(units[x, y]);
                        //give unit exp
                        //check if leveled
                        //present options when leveled
                    }
                        

                }
            }
        }

        /*//Test: give player a new spell
        var units = GameManager.Instance.PlayerParty.partyUnits;
        Unit unit = null;
        for (int x = 0; x < units.GetLength(0); x++)
            for (int y = 0; y < units.GetLength(1); y++)
                if (units[x, y] != null) 
                {
                    unit = units[x, y];
                    break;
                }

        if (unit == null)
            return;

        var abilities = unit.GetComponent<UnitAbilityManager>();
        GameManager.Instance.PlayerParty.GetComponent<PlayerPartyUpgrades>().GiveUnitNewSpell(unit, abilities.possibleAbilities[Random.Range(0, abilities.possibleAbilities.Count)], abilities.GetFreeSlot());
        */
    }

    private void StartPanel()
    {
        var panels = transform.Find("units_panel");
        unitSlots.Clear();

        for (int i = 0; i < panels.childCount; i++)
        {
            unitSlots.Add(panels.GetChild(i).GetComponent<VictoryScreenUnitSlot>());
            unitSlots[i].Init(emptySlotImage);
        }
    }

    // Button
    public void Proceed()
    {
        GameManager.Instance.StartCoroutine("BattleEnd", "MapScene");
    }


    private VictoryScreenUnitSlot FirstFreeSlot()
    {
        for (var i = 0; i < unitSlots.Count; i++)
        {
            if (unitSlots[i].IsEmpty())
            {
                return unitSlots[i];
            }
        }
        return null;
    }
}
