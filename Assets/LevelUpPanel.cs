using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    [SerializeField] private List<LvlUpPanelChoiceSlot> upgradeSlots;

    private VictoryScreenUnitSlot slot;
    private UnitData unitThatsLevelingUp = null;
    private int upgradePoints = 0;


    public void InitLevelUpPanel(VictoryScreenUnitSlot slot)
    {
        this.slot = slot;
        unitThatsLevelingUp = slot.slottedUnit;
        upgradePoints = 2;
        SetupUpgradeChoices();
    }

    public void ChooseOption(UnitAbility abi)
    {
        if (upgradePoints > 0)
        {
            slot.slottedUnit.GiveNewAbility(abi);
            upgradePoints--;
            if (upgradePoints == 0)
            {
                GetComponentInParent<VictoryPanel>().LevelUpDone(slot);
            }
        }
        else
        {
            Debug.Log("No more upgrade points");
        }
    }


    private void SetupUpgradeChoices()
    {
        var possibleAbils = unitThatsLevelingUp.RemainingPossibleAbilities();
        Shuffle(possibleAbils);
        
        for (int i = 0; i < 6; i++)
        {
            if (unitThatsLevelingUp.HasFreeAbilitySlots() && possibleAbils.Count > 0)
            {
                upgradeSlots[i].SetChoice(possibleAbils[0]);
                upgradeSlots[i].gameObject.SetActive(true);
                possibleAbils.RemoveAt(0);
            }
            else
            {
                var upgrade = GetRandomStatUpgrade();
            }
        }
        //upgradeSlot_1.gameObject.SetActive(false); upgradeSlot_2.gameObject.SetActive(false); upgradeSlot_3.gameObject.SetActive(false); upgradeSlot_4.gameObject.SetActive(false); upgradeSlot_5.gameObject.SetActive(false); upgradeSlot_6.gameObject.SetActive(false);
        //upgradeSlot_1.SetChoice(unitThatsLevelingUp.possibleAbilities[Random.Range(0, unitThatsLevelingUp.possibleAbilities.Count)]); upgradeSlot_2.SetChoice(unitThatsLevelingUp.possibleAbilities[Random.Range(0, unitThatsLevelingUp.possibleAbilities.Count)]); upgradeSlot_3.SetChoice(unitThatsLevelingUp.possibleAbilities[Random.Range(0, unitThatsLevelingUp.possibleAbilities.Count)]); upgradeSlot_4.SetChoice(unitThatsLevelingUp.possibleAbilities[Random.Range(0, unitThatsLevelingUp.possibleAbilities.Count)]); upgradeSlot_5.SetChoice(unitThatsLevelingUp.possibleAbilities[Random.Range(0, unitThatsLevelingUp.possibleAbilities.Count)]); upgradeSlot_6.SetChoice(unitThatsLevelingUp.possibleAbilities[Random.Range(0, unitThatsLevelingUp.possibleAbilities.Count)]);
    }

    void Shuffle<T>(List<T> inputList)
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            T temp = inputList[i];
            int randomIndex = UnityEngine.Random.Range(i, inputList.Count);
            inputList[i] = inputList[randomIndex];
            inputList[randomIndex] = temp;
        }
    }

    Tuple<string, float, Sprite> GetRandomStatUpgrade()
    {
        Tuple<string, float, Sprite> r = null;

        //r.Item1

        return r;
    }
}
