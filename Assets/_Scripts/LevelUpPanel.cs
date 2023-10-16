using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelUpPanel : MonoBehaviour
{
    [SerializeField] private UnitStatsPanel unitStatsPanel;
    [SerializeField] private List<LvlUpPanelChoiceSlot> abilitySlots;
    [SerializeField] private List<LvlUpPanelChoiceSlot> passiveSlots;

    [SerializeField] private Sprite upgradeDMG;
    [SerializeField] private Sprite upgradeMAGIC;
    [SerializeField] private Sprite upgradeATTSPD;
    [SerializeField] private Sprite upgradeHP;
    [SerializeField] private Sprite upgradeMOVEMENTSPD;

    private UnitData unitThatsLevelingUp = null;
    private int abilityPoints, passivePoints;


    public void InitLevelUpPanel(UnitData unit)
    {
        unitThatsLevelingUp = unit;
        abilityPoints = 1;
        passivePoints = 2;

        SetupUpgradeChoices_Passives();
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitThatsLevelingUp);
    }


    private void SetupUpgradeChoices_Abilities()
    {
        foreach (var ab in abilitySlots) ab.gameObject.SetActive(true);
        foreach (var passive in passiveSlots) passive.gameObject.SetActive(false);

        // P H A S E  1 :  ability upgrades
        List<UnitAbility> possibleAbils = unitThatsLevelingUp.RemainingPossibleAbilities();
        List<UnitAbility> learnedAbils = unitThatsLevelingUp.LearnedAbilities();
        Shuffle(possibleAbils);

        for (int i = 0; i < abilitySlots.Count; i++)
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
    }

    void SetupUpgradeChoices_Passives()
    {
        foreach (var ab in abilitySlots) ab.gameObject.SetActive(false);
        foreach (var passive in passiveSlots) passive.gameObject.SetActive(true);
        // P H A S E  2 :  stat upgrades
        int[] statUpgrades = GameManager.Instance.GenerateRandomUniqueIntegers(new Vector2Int(passiveSlots.Count, passiveSlots.Count), new Vector2Int(0, 5));
        //foreach (var su in statUpgrades) print(su.ToString());
        for (int i = 0; i < passiveSlots.Count; i++)
        {
            passiveSlots[i].SetChoice(GetRandomStatUpgrade(statUpgrades[i]));
            passiveSlots[i].gameObject.SetActive(true);
        }
    }

    private void AddAbility(List<UnitAbility> possibleAbils, int i)
    {
        abilitySlots[i].transform.GetChild(0).gameObject.SetActive(false);
        string spellName = possibleAbils[0].name;
        var clone = Instantiate(possibleAbils[0]);
        clone.name = spellName;
        abilitySlots[i].SetChoice(clone);
        abilitySlots[i].gameObject.SetActive(true);
        possibleAbils.RemoveAt(0);
    }

    private void AddUpgrade(List<UnitAbility> learnedAbils, int i)
    {
        var text = abilitySlots[i].transform.GetChild(0);
        var upgrade = GetRandomAbilityUpgrade(learnedAbils[UnityEngine.Random.Range(0, learnedAbils.Count)]);
        if (upgrade != null)
        {
            abilitySlots[i].SetChoice(upgrade);
            text.gameObject.SetActive(true);
            text.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.upgradeType;
        }
        else 
        {
            abilitySlots[i].SetChoice(GetRandomStatUpgrade(UnityEngine.Random.Range(0, 5)));
        }
        abilitySlots[i].gameObject.SetActive(true);
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
        if (abi.damage_upgradeable) possibleUpgrades.Add(new AbilityUpgrade(abi, "damage", 0.1f));
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
        if (abilityPoints < 1)
        {
            Debug.Log("No more upgrade points");
            return false;
        }
        unitThatsLevelingUp.GiveNewAbility(abi);
        unitStatsPanel.SetSpellslots();
        abilityPoints--;
        CheckIfDone();
        return true;
    }

    public bool TryToChooseOption(AbilityUpgrade upgradeObj)
    {
        if (abilityPoints < 1)
        {
            Debug.Log("No more upgrade points");
            return false;
        }
        unitThatsLevelingUp.UpgradeAbility(upgradeObj);
        abilityPoints--;
        CheckIfDone();
        return true;
    }



    public bool TryToChooseOption(string option)
    {
        if (passivePoints < 1)
        {
            Debug.Log("No more upgrade points");
            return false;
        }

        switch (option)
        {
            case ("DMG"):
                unitThatsLevelingUp.damage += 3.3f;
                unitStatsPanel.SetSlider(UnitStatSliderTypes.DMG, unitThatsLevelingUp.damage);
                break;

            case ("MAGIC"):
                unitThatsLevelingUp.magic += 3.1f;
                unitStatsPanel.SetSlider(UnitStatSliderTypes.MAGIC, unitThatsLevelingUp.magic);
                break;

            case ("ATTSPD"):
                unitThatsLevelingUp.attackSpeed += 15;
                unitStatsPanel.SetSlider(UnitStatSliderTypes.ATTSPD, unitThatsLevelingUp.attackSpeed);
                break;

            case ("HP"): 
                unitThatsLevelingUp.maxHp += 25f;
                unitStatsPanel.SetSlider(UnitStatSliderTypes.HP, unitThatsLevelingUp.maxHp);
                break;

            case ("MOVESPD"): 
                unitThatsLevelingUp.moveSpeed += 10;
                unitStatsPanel.SetSlider(UnitStatSliderTypes.MOVESPD, unitThatsLevelingUp.moveSpeed);
                break;

            default: 
                break;
        }
        passivePoints--;
        CheckIfDone();
        return true;
    }

    void CheckIfDone()
    {
        if (abilityPoints == 1 && passivePoints < 1)
        {
            SetupUpgradeChoices_Abilities();
        }
        if (passivePoints < 1 && abilityPoints < 1)
        {
            unitStatsPanel.gameObject.SetActive(false);
            if (GameManager.Instance.state == GameState.BATTLE)
            {
                GetComponentInParent<VictoryPanel>().LevelUpDone(unitThatsLevelingUp);
            }
            else if (GameManager.Instance.state == GameState.MAP) 
            {
                gameObject.SetActive(false);
            }
        }
    }
}