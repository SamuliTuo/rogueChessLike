using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    public Sprite abilityImg_PLACEHOLDER = null;
    public List<VictoryScreenUnitSlot> unitSlots = new List<VictoryScreenUnitSlot>();

    [SerializeField] private float expFillSpeed = 1.5f;
    [SerializeField] private Sprite emptySlotImage = null;
    [SerializeField] private LevelUpPanel lvlUpPanel = null;

    private List<VictoryScreenUnitSlot> slotsInUse = new List<VictoryScreenUnitSlot>();
    bool allReady = false;
    
    public void InitVictoryScreen()
    {
        allReady = false;
        StartPanel();
        float exp = GameManager.Instance.currentFightCumulatedExperience;
        exp = Random.Range(75, 166); /////////////
        print("Giving flat " + exp + " exp for all units for testing.");
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
    { // 'Tis for staggering the start of exp-gains between units.
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
                t += Time.deltaTime * expFillSpeed;
                float perc = t * t;
                gainSpeed = Mathf.Lerp(0.2f, 1, perc);
                if (t >= 1)
                    t = gainSpeed = 1;
            }
            slot.expBarFill.fillAmount += Time.deltaTime * gainSpeed;
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
            slot.slottedUnit.currentExperience = 0;
        }

        // End lvl up, repeat if there is leftover exp
        slot.expBarFill.fillAmount = slot.slottedUnit.CurrentExpPercent();
        if (leftoverExp > 0)
        {
            StartCoroutine(ExperienceGainCoroutine(slot, leftoverExp));
        }
    }


    public float AddExpAndReturnLeftoverIfLvlUp(UnitData unit, float experienceGained)
    {
        unit.currentExperience += experienceGained;
        if (unit.currentExperience >= unit.nextLevelExperience)
        {
            // lvl++;
            unit.currentExperience -= unit.nextLevelExperience;
            unit.nextLevelExperience *= 1.5f;
            return unit.currentExperience;
        }
        return -1;
    }


    public void UpgradeChosen(VictoryScreenUnitSlot slot, int button)
    {
        print("fix meeeeee" + button);
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

    public void OpenLvlUpPopUp(VictoryScreenUnitSlot slot)
    {
        if (lvlUpPanel.gameObject.activeSelf)
            return;

        lvlUpPanel.gameObject.SetActive(true);
        lvlUpPanel.InitLevelUpPanel(slot);
    }
    public void LevelUpDone(VictoryScreenUnitSlot slot)
    {
        lvlUpPanel.gameObject.SetActive(false);

        slot.lvlUpSign.SetActive(false);
        slot.lvlUpPending = false;
    }
}
