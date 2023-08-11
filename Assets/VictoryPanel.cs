using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private float expFillSpeed = 1;
    [SerializeField] private Sprite emptySlotImage = null;
    public Sprite abilityImg_PLACEHOLDER = null;
    public List<VictoryScreenUnitSlot> unitSlots = new List<VictoryScreenUnitSlot>();
    

    public void InitVictoryScreen()
    {
        StartPanel();

        float exp = GameManager.Instance.currentFightCumulatedExperience;
        List<UnitData> units = GameManager.Instance.PlayerParty.partyUnits;
        List<VictoryScreenUnitSlot> slotsInUse = new List<VictoryScreenUnitSlot>();
        foreach (UnitData unitData in units)
        {
            var slot = FirstFreeSlot();
            if (slot != null)
            {
                slot.SlotAnUnit(unitData);
                slot.InitExpBar();
                slotsInUse.Add(slot);
            }
        }
        float expGaind = 80;
        for (int i = 0; i < slotsInUse.Count; i++)
        {
            GiveUnitExp(slotsInUse[i], expGaind);
        }
    }

    public async void GiveUnitExp(VictoryScreenUnitSlot slot, float exp)
    {
        print("giving exp to " + slot.slottedUnit.unitName + ", exp amount: " + exp);
        float startPerc = slot.slottedUnit.CurrentExpPercent();
        float leftoverExp = AddExpAndReturnLeftoverIfLvlUp(slot.slottedUnit, exp);

        await AnimateExpGain(slot, startPerc);

        if (leftoverExp >= 0)
        {
            slot.OpenLvlUpPopUp();


            //leveled UP!
            // 1. present player with options to upgrade the unit (ie. new skill, stats upgrades...)
            // 2. wait for input...
            // 3. give the leftover exp
            // 4. repeat if reached more levels

            /*
            pendingInput = true;
            while (pendingInput)
            {
                await Task.Yield();
            }
            */
            slot.slottedUnit.currentExperience = 0;
        }
    }



    public float AddExpAndReturnLeftoverIfLvlUp(UnitData unit, float experienceGained)
    {
        unit.currentExperience += experienceGained;
        if (unit.currentExperience > unit.nextLevelExperience)
        {
            // lvl++;
            unit.currentExperience -= unit.nextLevelExperience;
            return unit.currentExperience;
        }
        return -1;
    }

    private async Task AnimateExpGain(VictoryScreenUnitSlot slot, float startPerc)
    {
        slot.expBarFill.fillAmount = startPerc;
        while (slot.expBarFill.fillAmount < slot.slottedUnit.CurrentExpPercent())
        {
            slot.expBarFill.fillAmount += Time.deltaTime * expFillSpeed;
            if (slot.expBarFill.fillAmount > slot.slottedUnit.CurrentExpPercent())
                slot.expBarFill.fillAmount = slot.slottedUnit.CurrentExpPercent();

            await Task.Yield();
        }
    }


    bool pendingInput = false;
    public void UpgradeChosen(int button)
    {
        print(button);
        pendingInput = false;
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
