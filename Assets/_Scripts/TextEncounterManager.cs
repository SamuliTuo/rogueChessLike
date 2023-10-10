using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextEncounterManager : MonoBehaviour
{
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private TextMeshProUGUI encounterText;
    [SerializeField] private Image encounterImage;
    [SerializeField] private List<GameObject> buttons;

    private TextEncounter currentEncounter;

    
    public void ActivateTextEncounter(TextEncounter encounter)
    {
        currentEncounter = encounter;
        GameManager.Instance.MapController.SetCanMove(false);
        encounterPanel.SetActive(true);
        encounterText.text = currentEncounter.textPrompt;
        encounterImage.sprite = currentEncounter.image;
        SetButtons(encounter);
    }

    public void ChooseTextResponse(int button)
    {
        if (CheckForMoney(button) && CheckForUnit(button))
        {
            // Give REWARD
            bool success = RollDice(button); //tsekkaa voititko, häviökin aktivoi napin joten tätä ei tsekata ylemmässä tsekis
            DisplayResponsePrompt(button, success);
            GiveReward(button);
            CloseEncounter();
        }
    }

    public void DisplayResponsePrompt(int response, bool success)
    {
        if (success)
            encounterText.text = currentEncounter.responses[response].successPrompt;
        else
            encounterText.text = currentEncounter.responses[response].failPrompt;
    }

    private void SetButtons(TextEncounter encounter)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < encounter.responses.Count)
            {
                buttons[i].SetActive(true);
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentEncounter.responses[i].response;
                if (encounter.responses[i].requirements.needsUnit)
                {
                    buttons[i].transform.GetChild(1).gameObject.SetActive(true);
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

    private void CloseEncounter()
    {
        GameManager.Instance.MapController.SetCanMove(true);
        encounterPanel.SetActive(false);
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

    bool RollDice(int button)
    {
        if (currentEncounter.responses[button].requirements.minimumRoll > 0)
        {
            int roll = Random.Range(1, 21);
            if (roll < currentEncounter.responses[button].requirements.minimumRoll)
            {
                return false;
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
                buttons[button].GetComponentInChildren<TextEncounterResponseUnitSlot>().slottedUnit.currentExperience
                    += currentEncounter.responses[button].reward.experience;
            }
        }
        // New unit
        if (currentEncounter.responses[button].reward.unit.unitName != "" && GameManager.Instance.PlayerParty.IsPartyFull() == false)
        {
            GameManager.Instance.PlayerParty.AddUnit(currentEncounter.responses[button].reward.unit);
        }
    }
}
