using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    public UnitStatsPanel unitStatsPanel;
    [SerializeField] private List<LvlUpPanelChoiceSlot> abilitySlots;
    [SerializeField] private List<LvlUpPanelChoiceSlot> passiveSlots;

    [SerializeField] private LVLUpController_2nd LVLUpPanel_2nd = null;
    [SerializeField] private LvlUpController_3rd LVLUpPanel_3rd = null;
    [SerializeField] private LvlUpController_4th LVLUpPanel_4th = null;
    [SerializeField] private LvlUpController_5th LVLUpPanel_5th = null;
    [SerializeField] private LvlUpController_6th LVLUpPanel_6th = null;
    [SerializeField] private LvlUpController_7th LVLUpPanel_7th = null;
    [SerializeField] private LvlUpController_8th LVLUpPanel_8th = null;

    [SerializeField] private Sprite upgradeDMG;
    [SerializeField] private Sprite upgradeMAGIC;
    [SerializeField] private Sprite upgradeATTSPD;
    [SerializeField] private Sprite upgradeHP;
    [SerializeField] private Sprite upgradeMOVEMENTSPD;
    [SerializeField] private Sprite upgradeExtraSP;

    private UnitData unitLeveling = null;
    private int abilityPoints, passivePoints;
    private int abilityClicked, optionChosen;

    public void InitLevelUpPanel(UnitData unit)
    {
        unitLeveling = unit;
        RaiseStats();
        StartCorrectLevelUpPattern();
        /*
        abilityPoints = 1;
        passivePoints = 1;

        SetupUpgradeChoices_Passives();
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitThatsLevelingUp);
        */
    }

    void RaiseStats()
    {
        if (unitLeveling == null)
            return;

        print("Stats got increased, add a visual element telling how much of each stat");
        unitLeveling.maxHp += unitLeveling.unitClass.hp;
        unitLeveling.damage += unitLeveling.unitClass.dmg;
        unitLeveling.magic += unitLeveling.unitClass.mgDmg;
        unitLeveling.moveSpeed += unitLeveling.unitClass.moveSpd;
        unitLeveling.attackSpeed += unitLeveling.unitClass.attSpd;
        unitLeveling.armor += unitLeveling.unitClass.armor;
        unitLeveling.magicRes += unitLeveling.unitClass.mgArmor;
    }

    // Level 1 : Get a random spell and class

    void StartCorrectLevelUpPattern()
    {
        switch (unitLeveling.currentLevel)
        {
            // What level is the unit?
            case 1: StartCoroutine(LVLUp_2nd()); break;
            case 2: StartCoroutine(LVLUp_3rd()); break;
            case 3: StartCoroutine(LVLUp_4th()); break;
            case 4: StartCoroutine(LVLUp_5th()); break;
            case 5: StartCoroutine(LVLUp_6th()); break;
            case 6: StartCoroutine(LVLUp_7th()); break;
            case 7: StartCoroutine(LVLUp_8th()); break;
            //case 7: StartCoroutine(LVLUp_8th()); break;
            //case 8: StartCoroutine(LVLUp_9th()); break;
        }
    }

    // Augment
    IEnumerator LVLUp_2nd()
    {
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitLeveling);
        LVLUpPanel_2nd.gameObject.SetActive(true);
        LVLUpPanel_2nd.InitLevelUpPanel(unitLeveling, this);

        // Wait until players clicks one of the upgrade-slots
        abilityClicked = -1;
        while (abilityClicked == -1)
        {
            yield return null;
        }

        // Open the choices and wait until player chooses one
        LVLUpPanel_2nd.InitUpgradeChoices();
        optionChosen = -1;
        while (optionChosen == -1)
        {
            yield return null;
        }
        LVLUpPanel_2nd.ChooseOption(optionChosen);
        unitStatsPanel.gameObject.SetActive(false);
        LVLUpPanel_2nd.gameObject.SetActive(false);
        unitLeveling.currentLevel++;
        GetComponentInParent<VictoryPanel>().LevelUpDone(unitLeveling);
        gameObject.SetActive(false);
    }

    // Support ability
    IEnumerator LVLUp_3rd()
    {
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitLeveling);
        LVLUpPanel_3rd.gameObject.SetActive(true);
        LVLUpPanel_3rd.InitLevelUpPanel(unitLeveling, this);

        abilityClicked = -1;
        while (abilityClicked == -1)
        {
            yield return null;
        }

        LVLUpPanel_3rd.InitUpgradeChoices();
        optionChosen = -1;
        while (optionChosen == -1)
        {
            yield return null;
        }
        LVLUpPanel_3rd.ChooseOption(optionChosen);
        unitStatsPanel.gameObject.SetActive(false);
        LVLUpPanel_3rd.gameObject.SetActive(false);
        unitLeveling.currentLevel++;
        GetComponentInParent<VictoryPanel>().LevelUpDone(unitLeveling);
        gameObject.SetActive(false);
    }

    // Class prefix
    IEnumerator LVLUp_4th()
    {
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitLeveling);
        unitStatsPanel.AddClassPrefix();
        LVLUpPanel_4th.gameObject.SetActive(true);
        LVLUpPanel_4th.InitLevelUpPanel(unitLeveling, this);
        // Wait until players has chosen the class-prefix
        while (LVLUpPanel_4th.prefixChosen == false)
        {
            yield return null;
        }
        unitStatsPanel.gameObject.SetActive(false);
        LVLUpPanel_4th.gameObject.SetActive(false);
        unitLeveling.currentLevel++;
        GetComponentInParent<VictoryPanel>().LevelUpDone(unitLeveling);
        gameObject.SetActive(false);
    }

    // Ultimate ability
    IEnumerator LVLUp_5th()
    {
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitLeveling);
        LVLUpPanel_5th.gameObject.SetActive(true);
        LVLUpPanel_5th.InitLevelUpPanel(unitLeveling, this);

        // Wait until players clicks one of the upgrade-slots
        abilityClicked = -1;
        while (abilityClicked == -1)
        {
            yield return null;
        }

        // Open the choices and wait until player chooses one
        LVLUpPanel_5th.InitUpgradeChoices();
        optionChosen = -1;
        while (optionChosen == -1)
        {
            yield return null;
        }
        LVLUpPanel_5th.ChooseOption(optionChosen);
        unitStatsPanel.gameObject.SetActive(false);
        LVLUpPanel_5th.gameObject.SetActive(false);
        unitLeveling.currentLevel++;
        GetComponentInParent<VictoryPanel>().LevelUpDone(unitLeveling);
        gameObject.SetActive(false);
    }

    // Augment
    IEnumerator LVLUp_6th()
    {
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitLeveling);
        LVLUpPanel_6th.gameObject.SetActive(true);
        LVLUpPanel_6th.InitLevelUpPanel(unitLeveling, this);

        abilityClicked = -1;
        while (abilityClicked == -1)
        {
            yield return null;
        }

        LVLUpPanel_6th.InitUpgradeChoices();
        optionChosen = -1;
        while (optionChosen == -1)
        {
            yield return null;
        }
        LVLUpPanel_6th.ChooseOption(optionChosen);
        unitStatsPanel.gameObject.SetActive(false);
        LVLUpPanel_6th.gameObject.SetActive(false);
        unitLeveling.currentLevel++;
        GetComponentInParent<VictoryPanel>().LevelUpDone(unitLeveling);
        gameObject.SetActive(false);
    }

    // Class suffix
    IEnumerator LVLUp_7th()
    {
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitLeveling);
        unitStatsPanel.AddClassSuffix();
        LVLUpPanel_7th.gameObject.SetActive(true);
        LVLUpPanel_7th.InitLevelUpPanel(unitLeveling, this);
        // Wait until players has chosen the class-prefix
        while (LVLUpPanel_7th.suffixChosen == false)
        {
            yield return null;
        }
        unitStatsPanel.gameObject.SetActive(false);
        LVLUpPanel_7th.gameObject.SetActive(false);
        unitLeveling.currentLevel++;
        GetComponentInParent<VictoryPanel>().LevelUpDone(unitLeveling);
        gameObject.SetActive(false);
    }

    // Augment
    IEnumerator LVLUp_8th()
    {
        unitStatsPanel.gameObject.SetActive(true);
        unitStatsPanel.OpenUnitStatsPanel(unitLeveling);
        LVLUpPanel_8th.gameObject.SetActive(true);
        LVLUpPanel_8th.InitLevelUpPanel(unitLeveling, this);

        // Wait until players clicks one of the upgrade-slots
        abilityClicked = -1;
        while (abilityClicked == -1)
        {
            yield return null;
        }

        // Open the choices and wait until player chooses one
        LVLUpPanel_8th.InitUpgradeChoices();
        optionChosen = -1;
        while (optionChosen == -1)
        {
            yield return null;
        }
        LVLUpPanel_8th.ChooseOption(optionChosen);
        unitStatsPanel.gameObject.SetActive(false);
        LVLUpPanel_8th.gameObject.SetActive(false);
        unitLeveling.currentLevel++;
        GetComponentInParent<VictoryPanel>().LevelUpDone(unitLeveling);
        gameObject.SetActive(false);
    }


    public void AbilityClicked(int slot)
    {
        abilityClicked = slot;
    }

    void EndLevelUp()
    {
        unitLeveling.currentLevel++;
        unitLeveling = null;
    }

    public void TryToChooseOption(AbilityUpgrade upgradeObj, int slot)
    {
        optionChosen = slot;
        /*
        if (abilityPoints < 1)
        {
            return false;
        }*/
        unitLeveling.UpgradeAbility(upgradeObj);
        //abilityPoints--;
        //CheckIfDone();
        //return true;
    }
    public void TryToChooseOption(UnitAugment augment, int slot)
    {
        optionChosen = slot;
        print("adding augment +" + augment + " to " + unitLeveling.name);
        unitLeveling.AddAugment(augment);
    }

    public void TryToChooseOption(UnitAbility abi, int slot)
    {
        unitLeveling.GiveNewAbility(abi);
        unitStatsPanel.SetSpellslots();
        optionChosen = slot;
        //CheckIfDone();
        //return true;
    }

































    private void SetupUpgradeChoices_Abilities()
    {
        foreach (var ab in abilitySlots) 
            ab.gameObject.SetActive(true);

        foreach (var passive in passiveSlots) 
            passive.gameObject.SetActive(false);

        // P H A S E  1 :  ability upgrades
        //List<UnitAbility> possibleAbils = unitLeveling.RemainingPossibleAbilities();
        List<UnitAbility> learnedAbils = unitLeveling.LearnedAbilities();
        //Shuffle(possibleAbils);

        //for (int i = 0; i < abilitySlots.Count; i++)
        //{
        //    bool isAbility = false;
        //    if (unitLeveling.HasFreeAbilitySlots() && possibleAbils.Count > 0)
        //    {
        //        isAbility = true;
        //        if (learnedAbils.Count > 0)
        //        {
        //            isAbility = UnityEngine.Random.Range(0, 100) < 50;
        //        }
        //    }

        //    if (isAbility)
        //    {
        //        AddAbility(possibleAbils, i);
        //    }
        //    else
        //    {
        //        AddUpgrade(learnedAbils, i);
        //    }
        //}
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
            passiveSlots[i].SetChoice(GetRandomStatUpgrade(statUpgrades[i]), this);
            passiveSlots[i].gameObject.SetActive(true);
        }
    }

    private void AddAbility(List<UnitAbility> possibleAbils, int i)
    {
        abilitySlots[i].transform.GetChild(0).gameObject.SetActive(false);
        string spellName = possibleAbils[0].name;
        var clone = Instantiate(possibleAbils[0]);
        clone.name = spellName;
        abilitySlots[i].SetChoice(clone, this, i);
        abilitySlots[i].gameObject.SetActive(true);
        possibleAbils.RemoveAt(0);
    }

    private void AddUpgrade(List<UnitAbility> learnedAbils, int i)
    {
        var text = abilitySlots[i].transform.GetChild(0);
        var upgrade = GetRandomAbilityUpgrades(learnedAbils[UnityEngine.Random.Range(0, learnedAbils.Count)]);
        if (upgrade != null)
        {
            //abilitySlots[i].SetChoice(upgrade);
            text.gameObject.SetActive(true);
            //text.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.upgradeType;
        }
        else
        {
            abilitySlots[i].SetChoice(GetRandomStatUpgrade(UnityEngine.Random.Range(0, 5)), this);
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

    public List<AbilityUpgrade> GetRandomAbilityUpgrades(UnitAbility abi, int count = 1)
    {
        List<AbilityUpgrade> possibleUpgrades = new List<AbilityUpgrade>();

        if (abi.damage_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.DAMAGE, 0.1f));
        if (abi.cooldown_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.COOLDOWN, 0.75f));
        if (abi.castSpeed_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.CAST_SPEED, 0.5f));
        if (abi.flySpeed_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.FLY_SPEED, 6));
        if (abi.reach_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.REACH, 1));
        if (abi.bounceCount_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.BOUNCE_COUNT, 1));
        if (abi.bounceRange_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.BOUNCE_RANGE, 1));
        if (abi.bounceDamageAmp_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.BOUNCE_DMG_AMP, 0.2f));
        if (abi.projectilesPerBounce_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.PROJECTILES_PER_BOUNCE, 1));
        if (abi.spawnUnitCount_upgradeable) 
            possibleUpgrades.Add(new AbilityUpgrade(abi, AbilityUpgradeType.SPAWN_UNIT_COUNT, 1));

        if (possibleUpgrades.Count <= 0)
            return null;
        var r = new List<AbilityUpgrade>();
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, possibleUpgrades.Count);
            r.Add(possibleUpgrades[rand]);
            possibleUpgrades.Remove(possibleUpgrades[rand]);
        }
        return r;
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
            r = new StatUpgrade(null, null, 0, null, null, 0, true, upgradeExtraSP);
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
        public Sprite skipPassiveSprite;

        public StatUpgrade(
            Sprite _sprite1 = null,
            string _stat1Name = null,
            float _stat1Amount = 0,
            Sprite _sprite2 = null,
            string _stat2Name = null,
            float _stat2Amount = 0,
            bool _skipPassive = false,
            Sprite _skipPassiveSprite = null
            )
        {
            sprite1 = _sprite1;
            stat1Name = _stat1Name;
            stat1Amount = _stat1Amount;
            sprite2 = _sprite2;
            stat2Name = _stat2Name;
            stat2Amount = _stat2Amount;
            skipPassive = _skipPassive;
            skipPassiveSprite = _skipPassiveSprite;
        }
    }



    // Choosing the upgrade



    [SerializeField] float upgradeAmount_damage = 3.3f;
    [SerializeField] float upgradeAmount_magic = 3.1f;
    [SerializeField] float upgradeAmount_attSpd = 15f;
    [SerializeField] float upgradeAmount_hp = 25f;
    [SerializeField] float upgradeAmount_moveSpd = 10f;


    public bool TryToChooseOption(LevelUpPanel.StatUpgrade option)
    {
        print("trying to choose stat upgrade");
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
                    unitLeveling.damage += upgradeAmount_damage * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.DMG, unitLeveling.damage);
                    break;

                case ("MAGIC"):
                    unitLeveling.magic += upgradeAmount_magic * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.MAGIC, unitLeveling.magic);
                    break;

                case ("ATTSPD"):
                    unitLeveling.attackSpeed += upgradeAmount_attSpd * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.ATTSPD, unitLeveling.attackSpeed);
                    break;

                case ("HP"):
                    unitLeveling.maxHp += upgradeAmount_hp * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.HP, unitLeveling.maxHp);
                    break;

                case ("MOVESPD"):
                    unitLeveling.moveSpeed += upgradeAmount_moveSpd * option.stat1Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.MOVESPD, unitLeveling.moveSpeed);
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
                    unitLeveling.damage += upgradeAmount_damage * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.DMG, unitLeveling.damage);
                    break;

                case ("MAGIC"):
                    unitLeveling.magic += upgradeAmount_magic * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.MAGIC, unitLeveling.magic);
                    break;

                case ("ATTSPD"):
                    unitLeveling.attackSpeed += upgradeAmount_attSpd * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.ATTSPD, unitLeveling.attackSpeed);
                    break;

                case ("HP"):
                    unitLeveling.maxHp += upgradeAmount_hp * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.HP, unitLeveling.maxHp);
                    break;

                case ("MOVESPD"):
                    unitLeveling.moveSpeed += upgradeAmount_moveSpd * option.stat2Amount;
                    unitStatsPanel.SetSlider(UnitStatSliderTypes.MOVESPD, unitLeveling.moveSpeed);
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
        print("checking if levle up done");
        if (abilityPoints > 0 && passivePoints < 1)
        {
            SetupUpgradeChoices_Abilities();
        }
        if (passivePoints < 1 && abilityPoints < 1)
        {
            unitStatsPanel.gameObject.SetActive(false);
            if (GameManager.Instance.state == GameState.BATTLE)
            {
                GetComponentInParent<VictoryPanel>().LevelUpDone(unitLeveling);
            }
            else if (GameManager.Instance.state == GameState.MAP) 
            {
                gameObject.SetActive(false);
            }
        }
    }
}