using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlUpController_4th : MonoBehaviour
{
    public bool prefixChosen = false;

    [SerializeField] private Sprite unChosenUpgradeImage = null;

    [SerializeField] private GameObject choiceSlots = null;
    [SerializeField] private Image upgradeChoice1 = null;
    [SerializeField] private Image upgradeChoice2 = null;
    [SerializeField] private Image upgradeChoice3 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_1 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_2 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_3 = null;

    [SerializeField] private Image classPrefixCoatOfArms = null;
    [SerializeField] private TextMeshProUGUI prefixName = null;
    [SerializeField] private Image prefixRerollButton = null;
    private int prefixRerollsRemaining;

    private List<UnitAugments.Augment> augmentChoices = new List<UnitAugments.Augment>();

    private LevelUpPanel lvlUpPanel;
    private UnitData unitLeveling;

    public void InitLevelUpPanel(UnitData unit, LevelUpPanel lvlUpPanel)
    {
        prefixChosen = false;
        this.lvlUpPanel = lvlUpPanel;
        this.unitLeveling = unit;
        classPrefixCoatOfArms.sprite = unit.unitClassPrefix.coatOfArms;
        prefixName.text = unit.unitClassPrefix.name;
        prefixRerollsRemaining = 2;
    }

    public void ChoosePrefix()
    {
        prefixRerollsRemaining = 0;
        prefixRerollButton.gameObject.SetActive(false);
        prefixChosen = true;
    }

    public void RerollPrefix()
    {
        if (prefixRerollsRemaining <= 0)
        {
            return;
        }

        var newPrefix = GameManager.Instance.ClassLibrary.GetRandomPrefix(unitLeveling.unitClassPrefix);
        unitLeveling.unitClassPrefix = newPrefix;
        classPrefixCoatOfArms.sprite = newPrefix.coatOfArms;
        prefixName.text = newPrefix.name;
        prefixRerollsRemaining--;
        lvlUpPanel.unitStatsPanel.SetClass();

        if (prefixRerollsRemaining <= 0)
        {
            prefixChosen = true;
            prefixRerollButton.gameObject.SetActive(false);
        }
    }
}
