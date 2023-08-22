using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    public Sprite abilityImg_PLACEHOLDER = null;
    public List<VictoryScreenUnitSlot> unitSlots = new List<VictoryScreenUnitSlot>();

    private float expFillMaxSpeed = 1.5f;
    [SerializeField] private Sprite emptySlotImage = null;
    [SerializeField] private LevelUpPanel lvlUpPanel = null;

    private List<VictoryScreenUnitSlot> slotsInUse = new List<VictoryScreenUnitSlot>();
    bool allReady = false;

    public void InitVictoryScreen()
    {
        allReady = false;
        StartPanel();
        float exp = GameManager.Instance.currentFightCumulatedExperience;
        exp = 120; /////////////
        List<UnitData> units = GameManager.Instance.PlayerParty.partyUnits;

        slotsInUse.Clear();
        foreach (UnitData unitData in units)
        {
            var slot = FirstFreeSlot();
            if (slot == null)
                continue;

            slot.SlotAnUnit(unitData);
            slot.InitExpBar();
            slotsInUse.Add(slot);
        }

        if (slotsInUse.Count == 0)
        {
            allReady = true;
            return;
        }

        StartCoroutine(StartExpCoroutines(exp));
    }
    IEnumerator StartExpCoroutines(float exp)
    {
        for (int i = 0; i < slotsInUse.Count; i++)
        {
            StartCoroutine(ExperienceGainCoroutine(slotsInUse[i], exp));
            yield return new WaitForSeconds(0.3f);
        }
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


    private IEnumerator ExperienceGainCoroutine(VictoryScreenUnitSlot slot, float exp)
    {
        float t = 0;
        float gainSpeed = 0;
        float startPerc = slot.slottedUnit.CurrentExpPercent();
        float leftoverExp = AddExpAndReturnLeftoverIfLvlUp(slot.slottedUnit, exp);

        slot.expBarFill.fillAmount = startPerc;
        float targetPerc = slot.slottedUnit.CurrentExpPercent();
        if (leftoverExp >= 0)
            targetPerc = 1;

        // lerp the exp gain animation
        while (slot.expBarFill.fillAmount < targetPerc)
        {
            if (t < 1)
            { 
                t += Time.deltaTime * 0.3f;
                float perc = t * t;
                gainSpeed = Mathf.Lerp(0.2f, 1, perc);
                if (t >= 1)
                    t = gainSpeed = 1;
            }
            slot.expBarFill.fillAmount += Time.deltaTime * gainSpeed * expFillMaxSpeed;
            yield return null;
        }

        // Unit leveled up!
        if (leftoverExp >= 0)
        {
            slot.SlotLevelUp();
            while (slot.lvlUpPending)
            {
                yield return null;
            }


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

        slot.expBarFill.fillAmount = slot.slottedUnit.CurrentExpPercent();
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


    public void UpgradeChosen(VictoryScreenUnitSlot slot, int button)
    {
        print("fix meeeeee better pls " + button);
        // 1. get the unit
        // 2. get the upgrade
        // 3. apply the upgrade
        // 4. close the popup
        // 5. check if there are more upgrades to be done


        foreach (var s in slotsInUse)
        {
            if (s.lvlUpPopUp.activeSelf)
            {
                allReady = false;
                break;
            }
            allReady = true;
        }
        if (allReady)
        {
            // avaa exit-nappi
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

    public bool OpenLvlUpPopUp(VictoryScreenUnitSlot slot)
    {
        if (lvlUpPanel.gameObject.activeSelf)
            return false;

        lvlUpPanel.gameObject.SetActive(true);
        lvlUpPanel.InitLevelUpPanel(slot);

        return true;
    }
}
