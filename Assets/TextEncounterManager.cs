using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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
        SetButtons();
    }

    public void ChooseTextResponse(int button)
    {
        switch (button)
        {
            case 0:
                print("0");
                break;
            case 1:
                print("1");
                break;
            case 2:
                print("2");
                break;
            case 3:
                print("3");
                break;
            default:
                break;
        }
    }

    private void SetButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < currentEncounter.responsesWithRewards.Count)
            {
                buttons[i].SetActive(true);
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentEncounter.responsesWithRewards[i].response;
            }
            else
            {
                buttons[i].SetActive(false);
            }
        }
    }
}
