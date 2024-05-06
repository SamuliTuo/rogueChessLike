using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlUpController_6th : MonoBehaviour
{
    [SerializeField] private Sprite unChosenUpgradeImage = null;

    [SerializeField] private GameObject choiceSlots = null;
    [SerializeField] private Image upgradeChoice1 = null;
    [SerializeField] private Image upgradeChoice2 = null;
    [SerializeField] private Image upgradeChoice3 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_1 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_2 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_3 = null;

    //[SerializeField] private Image icon_signatureSpellToUpgrade1 = null;

    //[SerializeField] private Image abilityUpgradeIcon_1 = null;


    private List<UnitAugments.Augment> upgradeChoices = new List<UnitAugments.Augment>();

    private LevelUpPanel lvlUpPanel;
    private UnitData unitLeveling;

    public void InitLevelUpPanel(UnitData unit, LevelUpPanel lvlUpPanel)
    {
        this.lvlUpPanel = lvlUpPanel;
        this.unitLeveling = unit;

        //icon_signatureSpellToUpgrade1.sprite = unChosenUpgradeImage;

        //abilityUpgradeIcon_1.sprite = GameManager.Instance.UnitLibrary.GetSpellSymbol(unit.ability1);


        //get units 'signature spell' ability
        //abilitySlot_1.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(lvlUpPanel.GetRandomAbilityUpgrades(unit.ability1, 3));
    }

    public void OpenAbilityUpgrades(int slot)
    {
        if (slot == 0)
        {
            lvlUpPanel.AbilityClicked(slot);
        }
    }

    public bool InitUpgradeChoices()
    {
        OpenChoices();
        return true;
    }

    void OpenChoices()
    {
        upgradeChoices.Clear();
        upgradeChoices = GameManager.Instance.UnitAugments.GetRandomAugments(3, unitLeveling.augments);

        choiceSlots.gameObject.SetActive(true);

        upgradeChoice1.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(upgradeChoices[0], lvlUpPanel, 0);
        upgradeChoice2.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(upgradeChoices[1], lvlUpPanel, 1);
        upgradeChoice3.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(upgradeChoices[2], lvlUpPanel, 2);

        abilityUpgradeText_1.text = upgradeChoices[0].name;
        abilityUpgradeText_2.text = upgradeChoices[1].name;
        abilityUpgradeText_3.text = upgradeChoices[2].name;

        upgradeChoice1.sprite = upgradeChoices[0].image;
        upgradeChoice2.sprite = upgradeChoices[1].image;
        upgradeChoice3.sprite = upgradeChoices[2].image;
    }

    public void ChooseOption(int choice)
    {
        choiceSlots.gameObject.SetActive(false);
        //icon_signatureSpellToUpgrade1.sprite = upgradeChoices[choice].image;
    }
}
