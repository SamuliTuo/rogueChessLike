using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    [SerializeField] private List<LvlUpPanelChoiceSlot> upgradeSlots;

    [SerializeField] private Sprite upgradeDMG;
    [SerializeField] private Sprite upgradeMAGIC;
    [SerializeField] private Sprite upgradeATTSPD;
    [SerializeField] private Sprite upgradeHP;
    [SerializeField] private Sprite upgradeMOVEMENTSPD;


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


    private void SetupUpgradeChoices()
    {
        // P H A S E  1 :  ability upgrades
        var possibleAbils = unitThatsLevelingUp.RemainingPossibleAbilities();
        var learnedAbils = unitThatsLevelingUp.LearnedAbilities();
        Shuffle(possibleAbils);

        for (int i = 0; i < 3; i++)
        {
            // new ability
            if (unitThatsLevelingUp.HasFreeAbilitySlots() && possibleAbils.Count > 0)
            {
                upgradeSlots[i].SetChoice(possibleAbils[0]);
                upgradeSlots[i].gameObject.SetActive(true);
                possibleAbils.RemoveAt(0);
            }
            // upgrade existing ability
            else if (learnedAbils.Count > 0)
            {
                var upgrade = GetRandomAbilityUpgrade(learnedAbils[UnityEngine.Random.Range(0, learnedAbils.Count)]);

                //upgradeSlots[i].SetChoice(upgrade);
                //upgradeSlots[i].gameObject.SetActive(true);
            }
        }

        // P H A S E  2 :  stat upgrades
        int[] statUpgrades = GameManager.Instance.GenerateRandomUniqueIntegers(new Vector2Int(3, 3), new Vector2Int(0, 5));
        for (int i = 3; i < 6; i++)
        {
            upgradeSlots[i].SetChoice(GetRandomStatUpgrade(statUpgrades[i - 3]));
            upgradeSlots[i].gameObject.SetActive(true);
        }
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


    AbilityUpgrade GetRandomAbilityUpgrade(UnitAbility abi)
    {
        var upgrade = new AbilityUpgrade(abi, "damage", 10);

        return upgrade;
    }


    Tuple<string, Sprite> GetRandomStatUpgrade(int upgrade)
    {
        switch (upgrade)
        {
            case 0: return new Tuple<string, Sprite>("DMG", upgradeDMG);
            case 1: return new Tuple<string, Sprite>("MAGIC", upgradeMAGIC);
            case 2: return new Tuple<string, Sprite>("ATTSPD", upgradeATTSPD);
            case 3: return new Tuple<string, Sprite>("HP", upgradeHP);
            case 4: return new Tuple<string, Sprite>("MOVESPD", upgradeMOVEMENTSPD);
            default: return null;
        }
    }


    public void ChooseOption(UnitAbility abi)
    {
        if (upgradePoints <= 0)
        {
            Debug.Log("No more upgrade points");
            return;
        }

        slot.slottedUnit.GiveNewAbility(abi);
        RemoveUpgradePointAndCheckIfDone();
    }

    public void ChooseOption(string option)
    {
        if (upgradePoints <= 0)
        {
            Debug.Log("No more upgrade points");
            return;
        }

        switch (option)
        {
            case ("DMG"):
                foreach (var attack in slot.slottedUnit.attacks)
                    attack.damage += 3;
                break;
            case ("MAGIC"):
                break;
            case ("ATTSPD"):
                foreach (var attack in slot.slottedUnit.attacks)
                    attack.attackInterval = attack.attackInterval * 0.9f;
                break;
            case ("HP"):
                slot.slottedUnit.maxHp += 9;
                break;
            case ("MOVESPD"):
                slot.slottedUnit.moveInterval = slot.slottedUnit.moveInterval * 0.9f;
                break;
        }
        RemoveUpgradePointAndCheckIfDone();
    }

    void RemoveUpgradePointAndCheckIfDone()
    {
        upgradePoints--;
        if (upgradePoints == 0)
        {
            GetComponentInParent<VictoryPanel>().LevelUpDone(slot);
        }
    }
}