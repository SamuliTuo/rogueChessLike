using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MapSettings;
using UnityEngine.UI;
using TMPro;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private TextMeshProUGUI encounterText;
    [SerializeField] private Image encounterImage;


    public List<MapFloor> encounters;
    public int currentNodeIndex;

    private CaravanController caravanController;
    private MapNode currentNode;

    private void Start()
    {
        caravanController = GetComponentInChildren<CaravanController>();
    }

    public void ActivateNode(MapNode node)
    {
        currentNode = node;
        if (node.type == MapNodeType.START_POS)
        {
            caravanController.OpenCaravan();
        }
        if (node.type == MapNodeType.BATTLE)
        {
            GameManager.Instance.currentFightCumulatedExperience = 0;
            GameManager.Instance.mapCameraLastPos = node.transform.position;
            GameManager.Instance.currentScenario = node.encounter.battleScenario;
            GameManager.Instance.CurrentMap.AddNextNodeOnPath(node);
            GameManager.Instance.ChangeGamestate(GameState.PRE_BATTLE);
            GameManager.Instance.SceneManagement.LoadScene("BattleScene");
        }
        else if (node.type == MapNodeType.END_POS)
        {
            print("BOSS BATTLEEEEEEEE!!!");
        }
        else if (node.type == MapNodeType.TREASURE)
        {
            ActivateEncounter();
        }
        else if (node.type == MapNodeType.CARAVAN)
        {
            caravanController.OpenCaravan();
        }
        else
        {
            ACTIVATE_EMPTY();
        }
    }

    void ActivateEncounter()
    {
        GameManager.Instance.MapController.SetCanMove(false);
        encounterText.text = currentNode.encounter.encounterText;
        encounterImage.sprite = currentNode.encounter.encounterImage;
        encounterPanel.SetActive(true);
    }

    void ACTIVATE_EMPTY()
    {
        GameManager.Instance.MapController.SetCanMove(true);
    }


    // The idea was to use these for the non-combat encounters
    void TriggerEncounter(MapNodeType encounter)
    {
        
    }

    private void EndEncounter(MapFloor encounter)
    {
        // Handle the logic for ending the encounter, including rewards and penalties
        // Unload the encounter scene and return to the map
    }
}