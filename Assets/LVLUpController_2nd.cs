using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LVLUpController_2nd : MonoBehaviour
{
    [SerializeField] private Sprite unChosenUpgradeImage = null;

    [SerializeField] private GameObject choiceSlots = null;
    [SerializeField] private Image upgradeChoice1 = null;
    [SerializeField] private Image upgradeChoice2 = null;
    [SerializeField] private Image upgradeChoice3 = null;

    [SerializeField] private Image icon_signatureSpellToUpgrade1 = null;
    [SerializeField] private Image icon_signatureSpellToUpgrade2 = null;
    [SerializeField] private Image icon_signatureSpellToUpgrade3 = null;

    [SerializeField] private Image abilityUpgradeIcon_1 = null;
    [SerializeField] private Image abilityUpgradeIcon_2 = null;
    [SerializeField] private Image abilityUpgradeIcon_3 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_1 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_2 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_3 = null;

    private AbilityUpgrade upgrade1 = null;
    private AbilityUpgrade upgrade2 = null;
    private AbilityUpgrade upgrade3 = null;

    private bool slotFilled_0 = false;
    private bool slotFilled_1 = false;
    private bool slotFilled_2 = false;

    private LevelUpPanel lvlUpPanel;
    private UnitData unitLeveling;
    private int currentChoice;

    public void InitLevelUpPanel(UnitData unit, LevelUpPanel lvlUpPanel)
    {
        this.lvlUpPanel = lvlUpPanel;
        this.unitLeveling = unit;
        currentChoice = 0;

        icon_signatureSpellToUpgrade1.sprite = unChosenUpgradeImage;
        icon_signatureSpellToUpgrade2.sprite = unChosenUpgradeImage;
        icon_signatureSpellToUpgrade3.sprite = unChosenUpgradeImage;
        abilityUpgradeIcon_1.sprite = GameManager.Instance.AbilityLibrary.GetImg(unit.ability1);
        abilityUpgradeIcon_2.sprite = abilityUpgradeIcon_1.sprite;
        abilityUpgradeIcon_3.sprite = abilityUpgradeIcon_1.sprite;

        //get units 'signature spell' ability
        //abilitySlot_1.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(lvlUpPanel.GetRandomAbilityUpgrades(unit.ability1, 3));
    }

    public void OpenAbilityUpgrades(int slot)
    {
        if (slot == 0 && !slotFilled_0)
        {
            lvlUpPanel.AbilityClicked(slot);
        }
        else if (slot == 1 && !slotFilled_1)
        {
            lvlUpPanel.AbilityClicked(slot);
        }
        else if (slot == 2 && !slotFilled_2)
        {
            lvlUpPanel.AbilityClicked(slot);
        }
    }

    public bool GoToChoice(int choice)
    {
        switch (choice)
        {
            case 0:
                if (slotFilled_0)
                    return false;
                OpenChoices(icon_signatureSpellToUpgrade1, 0);
                slotFilled_0 = true;
                return true;

            case 1:
                if (slotFilled_1)
                    return false;
                OpenChoices(icon_signatureSpellToUpgrade1, 1);
                slotFilled_1 = true;
                return true;

            case 2:
                if (slotFilled_2)
                    return false;
                OpenChoices(icon_signatureSpellToUpgrade1, 2);
                slotFilled_2 = true;
                return true;

            default:
                return false;
        }
    }

    void OpenChoices(Image icon, int slot)
    {
        List<AbilityUpgrade> upgrades = lvlUpPanel.GetRandomAbilityUpgrades(unitLeveling.ability1, 3);

        choiceSlots.gameObject.SetActive(true);

        upgrade1 = upgrades[0];
        upgrade2 = upgrades[1];
        upgrade3 = upgrades[2];

        abilityUpgradeText_1.text = upgrades[0].upgradeType.ToString();
        abilityUpgradeText_2.text = upgrades[1].upgradeType.ToString();
        abilityUpgradeText_3.text = upgrades[2].upgradeType.ToString();

        upgradeChoice1.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgrades[0].upgradeType);
        upgradeChoice2.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgrades[1].upgradeType);
        upgradeChoice3.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgrades[2].upgradeType);
    }

    public void ChooseOption(int choice)
    {
        print("choice: " + choice);
    }
}
