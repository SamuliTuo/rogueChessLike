using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    StatUpgrade GetRandomStatUpgrade(int upgrade)
    {
        StatUpgrade r;
        var dieRoll = UnityEngine.Random.Range(0.00f, 100.00f);
        string statName = RandomStatName();

        if (dieRoll > 50)
        {
            // 1x upgrade
            r = new StatUpgrade(AddSpriteToTheStat(statName), statName, 1);
        }
        else if (dieRoll > 30)
        {
            // 1.5x upgrade
            r = new StatUpgrade(AddSpriteToTheStat(statName), statName, 1.5f);
        }
        else if (dieRoll > 12)
        {
            // 1x + 1x upgrades
            var l = new List<string>();
            l.Add(statName);
            string secondStat = RandomStatName(l);
            r = new StatUpgrade(AddSpriteToTheStat(statName), statName, 1, AddSpriteToTheStat(secondStat), secondStat, 1);
        }
        else if (dieRoll > 9)
        {
            // 1.5x + 1x upgrades
            var l = new List<string>();
            l.Add(statName);
            string secondStat = RandomStatName(l);
            r = new StatUpgrade(AddSpriteToTheStat(statName), statName, 1.5f, AddSpriteToTheStat(secondStat), secondStat, 1);
        }
        else if (dieRoll > 6)
        {
            // 2.25x upgrade
            r = new StatUpgrade(AddSpriteToTheStat(statName), statName, 2.25f);
        }
        else
        {
            // skip passive upgrade for 1 extra AP
            r = new StatUpgrade(null, null, 0, null, null, 0, true);
        }
        return r;
    }

    string RandomStatName(List<string> exclude = null)
    {
        List<string> randomList = new List<string>();
        bool add;
        if (exclude != null)
        {
            if (exclude.Count > 0)
            {
                add = true;
                foreach (var stat in exclude)
                    if (stat == "DMG")
                        add = false;
                if (add) randomList.Add("DMG");

                add = true;
                foreach (var stat in exclude)
                    if (stat == "MAGIC")
                        add = false;
                if (add) randomList.Add("MAGIC");

                add = true;
                foreach (var stat in exclude)
                    if (stat == "ATTSPD")
                        add = false;
                if (add) randomList.Add("ATTSPD");

                add = true;
                foreach (var stat in exclude)
                    if (stat == "HP")
                        add = false;
                if (add) randomList.Add("HP");

                add = true;
                foreach (var stat in exclude)
                    if (stat == "MOVESPD")
                        add = false;
                if (add) randomList.Add("MOVESPD");
            }
        }
        else
        {
            randomList.Add("DMG");
            randomList.Add("MAGIC");
            randomList.Add("ATTSPD");
            randomList.Add("HP");
            randomList.Add("MOVESPD");
        }
        return randomList[UnityEngine.Random.Range(0, randomList.Count)];
    }
    Sprite AddSpriteToTheStat(string stat)
    {
        switch (stat)
        {
            case "DMG": return upgradeDMG;
            case "MAGIC": return upgradeMAGIC;
            case "ATTSPD": return upgradeATTSPD;
            case "HP": return upgradeHP;
            case "MOVESPD": return upgradeMOVEMENTSPD;
            default: return null;
        }
    }
    public class StatUpgrade
    {
        public Sprite sprite1;
        public string stat1Name;
        public float stat1Amount;
        public Sprite sprite2;
        public string stat2Name;
        public float stat2Amount;
        public bool skipPassive;

        public StatUpgrade(
            Sprite _sprite1 = null,
            string _stat1Name = null,
            float _stat1Amount = 0,
            Sprite _sprite2 = null,
            string _stat2Name = null,
            float _stat2Amount = 0,
            bool _skipPassive = false
            )
        {
            sprite1 = _sprite1;
            stat1Name = _stat1Name;
            stat1Amount = _stat1Amount;
            sprite2 = _sprite2;
            stat2Name = _stat2Name;
            stat2Amount = _stat2Amount;
            skipPassive = _skipPassive;
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


    [SerializeField] float upgradeAmount_damage = 3.3f;
    [SerializeField] float upgradeAmount_magic = 3.1f;
    [SerializeField] float upgradeAmount_attSpd = 15f;
    [SerializeField] float upgradeAmount_hp = 25f;
    [SerializeField] float upgradeAmount_moveSpd = 10f;


    public bool TryToChooseOption(LevelUpPanel.StatUpgrade option)
    {
        if (passivePoints < 1)
        {
            Debug.Log("No more upgrade points");
            return false;
        }

        if (option.sprite1 != null)
        {
            switch (option.stat1Name)
            {
                case ("DMG"):
                    unitThatsLevelingUp.damage += upgradeAmount_damage * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.DMG, unitThatsLevelingUp.damage);
                    break;

                case ("MAGIC"):
                    unitThatsLevelingUp.magic += upgradeAmount_magic * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.MAGIC, unitThatsLevelingUp.magic);
                    break;

                case ("ATTSPD"):
                    unitThatsLevelingUp.attackSpeed += upgradeAmount_attSpd * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.ATTSPD, unitThatsLevelingUp.attackSpeed);
                    break;

                case ("HP"):
                    unitThatsLevelingUp.maxHp += upgradeAmount_hp * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.HP, unitThatsLevelingUp.maxHp);
                    break;

                case ("MOVESPD"):
                    unitThatsLevelingUp.moveSpeed += upgradeAmount_moveSpd * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.MOVESPD, unitThatsLevelingUp.moveSpeed);
                    break;

                default:
                    break;
            }
        }

        if (option.sprite2 != null)
        {
            switch (option.stat2Name)
            {
                case ("DMG"):
                    unitThatsLevelingUp.damage += upgradeAmount_damage * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.DMG, unitThatsLevelingUp.damage);
                    break;

                case ("MAGIC"):
                    unitThatsLevelingUp.magic += upgradeAmount_magic * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.MAGIC, unitThatsLevelingUp.magic);
                    break;

                case ("ATTSPD"):
                    unitThatsLevelingUp.attackSpeed += upgradeAmount_attSpd * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.ATTSPD, unitThatsLevelingUp.attackSpeed);
                    break;

                case ("HP"):
                    unitThatsLevelingUp.maxHp += upgradeAmount_hp * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.HP, unitThatsLevelingUp.maxHp);
                    break;

                case ("MOVESPD"):
                    unitThatsLevelingUp.moveSpeed += upgradeAmount_moveSpd * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.MOVESPD, unitThatsLevelingUp.moveSpeed);
                    break;

                default:
                    break;
            }
        }

        if (option.skipPassive)
            abilityPoints++;

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