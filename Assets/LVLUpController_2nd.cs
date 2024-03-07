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
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_1 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_2 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_3 = null;

    [SerializeField] private Image icon_signatureSpellToUpgrade1 = null;

    [SerializeField] private Image abilityUpgradeIcon_1 = null;
    

    private List<AbilityUpgrade> upgradeChoices = new List<AbilityUpgrade>();

    private bool slotFilled_0 = false;
    private bool slotFilled_1 = false;
    private bool slotFilled_2 = false;

    private LevelUpPanel lvlUpPanel;
    private UnitData unitLeveling;
    private int slotBeingUpgraded;

    public void InitLevelUpPanel(UnitData unit, LevelUpPanel lvlUpPanel)
    {
        this.lvlUpPanel = lvlUpPanel;
        this.unitLeveling = unit;
        slotBeingUpgraded = -1;

        icon_signatureSpellToUpgrade1.sprite = unChosenUpgradeImage;
        //icon_signatureSpellToUpgrade2.sprite = unChosenUpgradeImage;
        //icon_signatureSpellToUpgrade3.sprite = unChosenUpgradeImage;
        abilityUpgradeIcon_1.sprite = GameManager.Instance.UnitLibrary.GetSpellSymbol(unit.ability1);


        //get units 'signature spell' ability
        //abilitySlot_1.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(lvlUpPanel.GetRandomAbilityUpgrades(unit.ability1, 3));
    }

    public void OpenAbilityUpgrades(int slot)
    {
        if (slot == 0 && !slotFilled_0)
        {
            lvlUpPanel.AbilityClicked(slot);
        }
        //else if (slot == 1 && !slotFilled_1)
        //{
        //    lvlUpPanel.AbilityClicked(slot);
        //}
        //else if (slot == 2 && !slotFilled_2)
        //{
        //    lvlUpPanel.AbilityClicked(slot);
        //}
    }

    public bool InitUpgradeChoices(int choice)
    {
        switch (choice)
        {
            case 0:
                if (slotFilled_0)
                    return false;
                OpenChoices(icon_signatureSpellToUpgrade1, 1);
                slotFilled_0 = true;
                return true;

            //case 1:
            //    if (slotFilled_1)
            //        return false;
            //    OpenChoices(icon_signatureSpellToUpgrade1, 2);
            //    slotFilled_1 = true;
            //    return true;

            //case 2:
            //    if (slotFilled_2)
            //        return false;
            //    OpenChoices(icon_signatureSpellToUpgrade1, 3);
            //    slotFilled_2 = true;
            //    return true;

            default:
                return false;
        }
    }

    void OpenChoices(Image icon, int slot)
    {
        slotBeingUpgraded = slot;
        upgradeChoices.Clear();
        upgradeChoices = lvlUpPanel.GetRandomAbilityUpgrades(unitLeveling.ability1, 3);

        choiceSlots.gameObject.SetActive(true);

        upgradeChoice1.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(upgradeChoices[0], lvlUpPanel, slot);
        //upgradeChoice2.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(upgradeChoices[1], lvlUpPanel, slot);
        //upgradeChoice3.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(upgradeChoices[2], lvlUpPanel, slot);

        abilityUpgradeText_1.text = upgradeChoices[0].upgradeType.ToString();
        //abilityUpgradeText_2.text = upgradeChoices[1].upgradeType.ToString();
        //abilityUpgradeText_3.text = upgradeChoices[2].upgradeType.ToString();

        upgradeChoice1.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgradeChoices[0].upgradeType);
        //upgradeChoice2.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgradeChoices[1].upgradeType);
        //upgradeChoice3.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgradeChoices[2].upgradeType);
    }

    public void ChooseOption(int choice)
    {
        choiceSlots.gameObject.SetActive(false);

        //
        slotFilled_0 = true;
        icon_signatureSpellToUpgrade1.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgradeChoices[choice - 1].upgradeType);
        //switch (slotBeingUpgraded)
        //{
        //    case 1: 
        //        slotFilled_0 = true;
        //        icon_signatureSpellToUpgrade1.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgradeChoices[choice - 1].upgradeType);
        //        break;
        //    case 2: 
        //        slotFilled_1 = true;
        //        icon_signatureSpellToUpgrade2.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgradeChoices[choice - 1].upgradeType);
        //        break;
        //    case 3: 
        //        slotFilled_2 = true;
        //        icon_signatureSpellToUpgrade3.sprite = GameManager.Instance.AbilityLibrary.GetUpgradeSymbol(upgradeChoices[choice - 1].upgradeType); 
        //        break;
        //    default: break;
        //}
        //CheckIfDone();
    }
}
