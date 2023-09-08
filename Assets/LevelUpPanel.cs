using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private bool abilityChosen, passiveChosen;


    public void InitLevelUpPanel(VictoryScreenUnitSlot slot)
    {
        this.slot = slot;
        unitThatsLevelingUp = slot.slottedUnit;

        abilityChosen = passiveChosen = false;
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
                upgradeSlots[i].transform.GetChild(0).gameObject.SetActive(false);
                string spellName = possibleAbils[0].name;
                var clone = Instantiate(possibleAbils[0]);
                clone.name = spellName;
                upgradeSlots[i].SetChoice(clone);
                upgradeSlots[i].gameObject.SetActive(true);
                possibleAbils.RemoveAt(0);
            }
            // upgrade existing ability
            else if (learnedAbils.Count > 0)
            {
                var text = upgradeSlots[i].transform.GetChild(0);
                var upgrade = GetRandomAbilityUpgrade(learnedAbils[UnityEngine.Random.Range(0, learnedAbils.Count)]);
                text.gameObject.SetActive(true);
                text.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.upgradeName;

                upgradeSlots[i].SetChoice(upgrade);
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
        if (abi.spawnCount > 0 && abi.spawnUnit != "")
        {
            switch (UnityEngine.Random.Range(0, 4))
            {
                case 0: return new AbilityUpgrade(abi, "flySpeed", 10);
                case 1: return new AbilityUpgrade(abi, "bounceAbilityAmount", 1);
                case 2: return new AbilityUpgrade(abi, "reach", 10);
                case 3: return new AbilityUpgrade(abi, "cooldown", 1);
                default: return new AbilityUpgrade(abi, "damage", 10);
            }
        }
        else 
        {
            switch (UnityEngine.Random.Range(0, 5))
            {
                case 0: return new AbilityUpgrade(abi, "damage", 10);
                case 1: return new AbilityUpgrade(abi, "bounceAbilityAmount", 1);
                case 2: return new AbilityUpgrade(abi, "reach", 10);
                case 3: return new AbilityUpgrade(abi, "cooldown", 1);
                case 4: return new AbilityUpgrade(abi, "flySpeed", 5);
                default: return new AbilityUpgrade(abi, "damage", 10);
            }
        }
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



    // Choosing the upgrade

    public bool TryToChooseOption(UnitAbility abi)
    {
        if (abilityChosen)
        {
            Debug.Log("No more upgrade points");
            return false;
        }
        slot.slottedUnit.GiveNewAbility(abi);
        abilityChosen = true;
        CheckIfDone();
        return true;
    }

    public bool TryToChooseOption(AbilityUpgrade upgradeObj)
    {
        if (abilityChosen)
        {
            Debug.Log("No more upgrade points");
            return false;
        }
        slot.slottedUnit.UpgradeAbility(upgradeObj);
        abilityChosen = true;
        CheckIfDone();
        return true;
    }

    public bool TryToChooseOption(string option)
    {
        if (passiveChosen)
        {
            Debug.Log("No more upgrade points");
            return false;
        }

        switch (option)
        {
            case ("DMG"):
                foreach (var attack in slot.slottedUnit.attacks)
                    attack.damage += 3;
                break;

            case ("MAGIC"): break;

            case ("ATTSPD"):
                foreach (var attack in slot.slottedUnit.attacks)
                    attack.attackInterval = attack.attackInterval * 0.9f;
                break;

            case ("HP"): slot.slottedUnit.maxHp += 9; break;

            case ("MOVESPD"): slot.slottedUnit.moveInterval = slot.slottedUnit.moveInterval * 0.9f; break;

            default: break;
        }
        passiveChosen = true;
        CheckIfDone();
        return true;
    }

    void CheckIfDone()
    {
        if (passiveChosen && abilityChosen)
        {
            GetComponentInParent<VictoryPanel>().LevelUpDone(slot);
        }
    }
}