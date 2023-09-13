using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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
        List<UnitAbility> possibleAbils = unitThatsLevelingUp.RemainingPossibleAbilities();
        List<UnitAbility> learnedAbils = unitThatsLevelingUp.LearnedAbilities();
        Shuffle(possibleAbils);

        for (int i = 0; i < 3; i++)
        {
            bool isAbility = false;
            if (unitThatsLevelingUp.HasFreeAbilitySlots() && possibleAbils.Count > 0)
            {
                isAbility = true;
                if (learnedAbils.Count > 0)
                {
                    isAbility = UnityEngine.Random.Range(0, 100) < 50;
                }
            }
            
            if (isAbility)
            {
                AddAbility(possibleAbils, i);
            }
            else
            {
                AddUpgrade(learnedAbils, i);
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

    private void AddAbility(List<UnitAbility> possibleAbils, int i)
    {
        upgradeSlots[i].transform.GetChild(0).gameObject.SetActive(false);
        string spellName = possibleAbils[0].name;
        var clone = Instantiate(possibleAbils[0]);
        clone.name = spellName;
        upgradeSlots[i].SetChoice(clone);
        upgradeSlots[i].gameObject.SetActive(true);
        possibleAbils.RemoveAt(0);
    }

    private void AddUpgrade(List<UnitAbility> learnedAbils, int i)
    {
        var text = upgradeSlots[i].transform.GetChild(0);
        var upgrade = GetRandomAbilityUpgrade(learnedAbils[UnityEngine.Random.Range(0, learnedAbils.Count)]);
        if (upgrade != null)
        {
            upgradeSlots[i].SetChoice(upgrade);
            text.gameObject.SetActive(true);
            text.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.upgradeType;
        }
        else 
        {
            upgradeSlots[i].SetChoice(GetRandomStatUpgrade(UnityEngine.Random.Range(0, 5)));
        }
        upgradeSlots[i].gameObject.SetActive(true);
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
        List<AbilityUpgrade> possibleUpgrades = new List<AbilityUpgrade>();
        if (abi.damage_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "damage", 10));
        if (abi.cooldown_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "cooldown", 0.75f));
        if (abi.castSpeed_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "castSpeed", 0.5f));
        if (abi.flySpeed_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "flySpeed", 6));
        if (abi.reach_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "reach", 1));
        if (abi.bounceCount_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "bounceCount", 1));
        if (abi.bounceRange_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "bounceRange", 1));
        if (abi.bounceDamageAmp_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "bounceDamageAmp", 0.2f));
        if (abi.projectilesPerBounce_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "projectilesPerBounce", 1));
        if (abi.spawnUnitCount_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "spawnUnitCount", 1));

        if (possibleUpgrades.Count > 0)
            return possibleUpgrades[UnityEngine.Random.Range(0, possibleUpgrades.Count)];

        return null;
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
                {
                    print(attack.damage);
                    attack.damage += 3;
                    print(attack.damage);
                }
                break;

            case ("MAGIC"): 
                break;

            case ("ATTSPD"):
                foreach (var attack in slot.slottedUnit.attacks)
                {
                    attack.attackInterval *= 0.8f;
                }
                break;

            case ("HP"): 
                slot.slottedUnit.maxHp += 9; 
                break;

            case ("MOVESPD"): 
                slot.slottedUnit.moveInterval *= 0.8f; 
                break;

            default: 
                break;
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