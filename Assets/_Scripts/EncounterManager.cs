using System.Collections.Generic;
using UnityEngine;
using static MapSettings;

public class EncounterManager : MonoBehaviour
{
    public List<MapFloor> encounters;
    public int currentNodeIndex;

    private CaravanController caravanController;
    private TextEncounterManager textEncounterManager;

    private void Start()
    {
        caravanController = GetComponentInChildren<CaravanController>();
        textEncounterManager = GetComponentInChildren<TextEncounterManager>();
    }

    public void ActivateNode(MapNode node)
    {
        if (node.type == MapNodeType.START_POS) {
            caravanController.OpenCaravan();
        }
        if (node.type == MapNodeType.BATTLE) {
            ActivateBattleNode(node);
        }
        else if (node.type == MapNodeType.END_POS) {
            print("BOSS BATTLEEEEEEEE!!!");
        }
        else if (node.type == MapNodeType.CARAVAN) {
            caravanController.OpenCaravan();
        }
        else if (node.type == MapNodeType.RANDOM_ENCOUNTER) {
            ActivateRandomNode(node);
        }
        /*else if (node.type == MapNodeType.TREASURE) {
            ActivateEncounter();
        }*/
        else if (node.type == MapNodeType.NONE)
        {
            ACTIVATE_EMPTY();
        }
    }

    void ActivateBattleNode(MapNode node)
    {
        GameManager.Instance.currentFightCumulatedExperience = 0;
        GameManager.Instance.mapCameraLastPos = node.transform.position;
        GameManager.Instance.currentScenario = node.encounter.possibleBattleScenarios[Random.Range(0, node.encounter.possibleBattleScenarios.Count)];
        GameManager.Instance.CurrentMap.AddNextNodeOnPath(node);
        GameManager.Instance.SceneManagement.LoadScene("BattleScene");
    }
    
    void ACTIVATE_EMPTY()
    {
        GameManager.Instance.MapController.SetCanMove(true);
    }

    void ActivateRandomNode(MapNode node)
    {
        GameManager.Instance.CurrentMap.AddNextNodeOnPath(node);
        GameManager.Instance.MapController.SetCanMove(false);

        int dieRoll = Random.Range(0, node.encounter.possibleTextScenarios.Count + node.encounter.possibleBattleScenarios.Count);
        if (dieRoll < node.encounter.possibleTextScenarios.Count)
        {
            textEncounterManager.ActivateTextEncounter(node.encounter.possibleTextScenarios[dieRoll]);
        }
        else
        {
            Scenario battle = node.encounter.possibleBattleScenarios[dieRoll - node.encounter.possibleTextScenarios.Count];
            GameManager.Instance.currentFightCumulatedExperience = 0;
            GameManager.Instance.mapCameraLastPos = node.transform.position;
            GameManager.Instance.currentScenario = battle;
            GameManager.Instance.SceneManagement.LoadScene("BattleScene");
        }
    }
}