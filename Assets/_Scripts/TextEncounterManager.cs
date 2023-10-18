using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextEncounterManager : MonoBehaviour
{
    [SerializeField] private LevelUpPanel levelUpPanel;
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private TextMeshProUGUI encounterText;
    [SerializeField] private Image encounterImage;
    [SerializeField] private List<GameObject> buttons;
    [SerializeField] private GameObject exitButton;

    private TextEncounter currentEncounter;
    private Scenario queuedBattle = null;
    
    public void ActivateTextEncounter(TextEncounter encounter)
    {
        currentEncounter = encounter;
        GameManager.Instance.MapController.SetCanMove(false);
        queuedBattle = null;
        encounterPanel.SetActive(true);
        encounterText.text = currentEncounter.textPrompt;
        encounterImage.sprite = currentEncounter.image;
        SetButtons(encounter);
    }

    public void CloseEncounter()
    {
        if (queuedBattle != null)
        {
            var node = GameManager.Instance.MapController.currentPosition;
            GameManager.Instance.currentFightCumulatedExperience = 0;
            GameManager.Instance.mapCameraLastPos = node.transform.position;
            GameManager.Instance.currentScenario = queuedBattle;
            GameManager.Instance.SceneManagement.LoadScene("BattleScene");
        }
        else
        {
            GameManager.Instance.MapController.SetCanMove(true);
            encounterPanel.SetActive(false);
        }
    }

    public void ChooseTextResponse(int button)
    {
        if (CheckForMoney(button) && CheckForUnit(button))
        {
            if (ActivateResponseAndCheckForSuccess(button))
            {
                GiveReward(button);
            }
            HideResponseButtons();
        }
    }
    int RollDice(int button)
    {
        if (currentEncounter.responses[button].requirements.minimumRoll > 0)
        {
            int roll = Random.Range(1, 21);
            if (roll < currentEncounter.responses[button].requirements.minimumRoll)
            {
                return roll;
            }
        }
        return -100;
    }


    private bool ActivateResponseAndCheckForSuccess(int button)
    {
        int roll = RollDice(button);

        print("Checking a dice-roll. Add a modifier to the roll from chosen units stats somewhere here. kthxbyeee~");
        // Display the modified roll like " [roll] + [modifier] + [modifier]... / minimumroll "    ie. " 11 (+3) / 19 "   or something like this, with diff color or smthg

        if (roll == -100)
        {
            queuedBattle = currentEncounter.responses[button].reward.battleOnFailureOrAnyCase;
            encounterText.text = currentEncounter.responses[button].successPrompt;
            return true;
        }
        else if (roll >= currentEncounter.responses[button].requirements.minimumRoll)
        {
            queuedBattle = currentEncounter.responses[button].reward.battleOnSuccess;
            encounterText.text = "Roll: " + roll + " / " + currentEncounter.responses[button].requirements.minimumRoll + "\n" 
                + currentEncounter.responses[button].successPrompt;
            return true;
        } 
        else
        {
            queuedBattle = currentEncounter.responses[button].reward.battleOnFailureOrAnyCase;
            encounterText.text = "Roll: " + roll + " / " + currentEncounter.responses[button].requirements.minimumRoll + "\n"
                + currentEncounter.responses[button].failPrompt;
            return false;
        }
    }

    private void SetButtons(TextEncounter encounter)
    {
        exitButton.SetActive(false);
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < encounter.responses.Count)
            {
                buttons[i].SetActive(true);
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentEncounter.responses[i].response;
                if (encounter.responses[i].requirements.needsUnit)
                {
                    buttons[i].transform.GetChild(1).gameObject.SetActive(true);
                    buttons[i].GetComponentInChildren<TextEncounterResponseUnitSlot>().ClearSlot();
                }
                else
                {
                    buttons[i].transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            else
            {
                buttons[i].SetActive(false);
            }
        }
    }

    private void HideResponseButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(false);
        }
        exitButton.SetActive(true);
    }

    bool CheckForMoney(int button)
    {
        if (currentEncounter.responses[button].requirements.money != 0)
        {
            if (GameManager.Instance.PlayerParty.partyMoney < currentEncounter.responses[button].requirements.money)
            {
                return false;
            }
            else
            {
                GameManager.Instance.PlayerParty.AddMoney(-currentEncounter.responses[button].requirements.money);
            }
        }
        return true;
    }
    
    bool CheckForUnit(int button)
    {
        if (currentEncounter.responses[button].requirements.needsUnit)
        {
            if (buttons[button].GetComponentInChildren<TextEncounterResponseUnitSlot>().slottedUnit == null)
            {
                return false;
            }
        }
        return true;
    }
    
    void GiveReward(int button)
    {
        // Money
        if (currentEncounter.responses[button].reward.money > 0)
        {
            GameManager.Instance.PlayerParty.AddMoney(currentEncounter.responses[button].reward.money);
        }
        // Experience
        if (currentEncounter.responses[button].requirements.needsUnit && currentEncounter.responses[button].reward.experience > 0)
        {
            if (buttons[button].GetComponentInChildren<TextEncounterResponseUnitSlot>().slottedUnit != null)
            {
                var unit = buttons[button].GetComponentInChildren<TextEncounterResponseUnitSlot>().slottedUnit;
                unit.currentExperience += currentEncounter.responses[button].reward.experience;
                if (unit.currentExperience >= unit.nextLevelExperience)
                {
                    unit.currentExperience -= unit.nextLevelExperience;
                    unit.nextLevelExperience *= 1.5f;
                    levelUpPanel.gameObject.SetActive(true);
                    levelUpPanel.InitLevelUpPanel(unit);
                }
            }
        }
        // New unit
        if (currentEncounter.responses[button].reward.unit.unitName != "" && GameManager.Instance.PlayerParty.IsPartyFull() == false)
        {
            GameManager.Instance.PlayerParty.AddUnit(currentEncounter.responses[button].reward.unit);
        }
    }
}
