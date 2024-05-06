using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlUpController_5th : MonoBehaviour
{
    [SerializeField] private Sprite unChosenUpgradeImage = null;
    [SerializeField] private GameObject choiceSlots = null;
    [SerializeField] private Image upgradeChoice1 = null;
    [SerializeField] private Image upgradeChoice2 = null;
    [SerializeField] private Image upgradeChoice3 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_1 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_2 = null;
    [SerializeField] private TextMeshProUGUI abilityUpgradeText_3 = null;

    private List<LibraryAbility> abilityChoices_library = new List<LibraryAbility>();
    private List<UnitAbility> abilityChoices = new List<UnitAbility>();
    private LevelUpPanel lvlUpPanel;
    private UnitData unitLeveling;

    public void InitLevelUpPanel(UnitData unit, LevelUpPanel lvlUpPanel)
    {
        this.lvlUpPanel = lvlUpPanel;
        this.unitLeveling = unit;
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
        abilityChoices.Clear();

        // Get 2 random "Ultimate spells"
        var libraryEntry = GameManager.Instance.UnitLibrary.GetUnit(unitLeveling);
        var ultimateSpells = libraryEntry.ultimateSpells;
        var indexes = GameManager.Instance.GenerateRandomUniqueIntegers(new(2, 2), new(0, ultimateSpells.Count));
        for (int i = 0; i < indexes.Length; i++)
        {
            UnitAbility a = Instantiate(ultimateSpells[indexes[i]].spell);
            a.name = ultimateSpells[indexes[i]].spell.name;
            abilityChoices.Add(a);
            abilityChoices_library.Add(ultimateSpells[indexes[i]]);
        }

        // Get a random spell from the remaining "signature" and "support" spells:
        List<LibraryAbility> remainingNonUltimateSpells = new List<LibraryAbility>();
        for (int i = 0; i < libraryEntry.signatureSpells.Count; i++)
        {
            if (libraryEntry.signatureSpells[i].spell.name != unitLeveling.ability1.name && libraryEntry.signatureSpells[i].spell.name != unitLeveling.ability2.name)
            {
                remainingNonUltimateSpells.Add(libraryEntry.signatureSpells[i]);
            }
        }
        for (int i = 0; i < libraryEntry.supportSpells.Count; i++)
        {
            if (libraryEntry.supportSpells[i].spell.name != unitLeveling.ability2.name)
            {
                remainingNonUltimateSpells.Add(libraryEntry.supportSpells[i]);
            }
        }
        var chooseIndex = Random.Range(0, remainingNonUltimateSpells.Count);
        UnitAbility a2 = Instantiate(remainingNonUltimateSpells[chooseIndex].spell);
        a2.name = remainingNonUltimateSpells[chooseIndex].spell.name;
        abilityChoices.Add(a2);
        abilityChoices_library.Add(remainingNonUltimateSpells[chooseIndex]);


        // Setup the choices
        choiceSlots.gameObject.SetActive(true);

        upgradeChoice1.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(abilityChoices[0], lvlUpPanel, 0);
        upgradeChoice2.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(abilityChoices[1], lvlUpPanel, 1);
        upgradeChoice3.GetComponent<LvlUpPanelChoiceSlot>().SetChoice(abilityChoices[2], lvlUpPanel, 2);

        abilityUpgradeText_1.text = abilityChoices_library[0].name;
        abilityUpgradeText_2.text = abilityChoices_library[1].name;
        abilityUpgradeText_3.text = abilityChoices_library[2].name;

        upgradeChoice1.sprite = abilityChoices_library[0].image;
        upgradeChoice2.sprite = abilityChoices_library[1].image;
        upgradeChoice3.sprite = abilityChoices_library[2].image;
    }

    public void ChooseOption(int choice)
    {
        choiceSlots.gameObject.SetActive(false);
        //icon_signatureSpellToUpgrade1.sprite = abilityChoices[choice].image;
    }
}
