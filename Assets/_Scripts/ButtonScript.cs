using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class ButtonScript : MonoBehaviour
{
    private Unit[,] currUnits;

    // Scenariobuilder panels
    public void TerrainPanel()
    {
        if (GameManager.Instance.state == GameState.SCENARIO_BUILDER)
        {
            ScenarioBuilder.Instance.OpenPanel(ScenarioBuilderPanel.TERRAIN);
        }
    }
    public void UnitsPanel()
    {
        if (GameManager.Instance.state == GameState.SCENARIO_BUILDER)
        {
            ScenarioBuilder.Instance.OpenPanel(ScenarioBuilderPanel.UNITS);
        }
    }
    public void ObjectsPanel()
    {
        if (GameManager.Instance.state == GameState.SCENARIO_BUILDER)
        {
            ScenarioBuilder.Instance.OpenPanel(ScenarioBuilderPanel.OBJECTS);
        }
    }

    public void PlayButton()
    {
        if (GameManager.Instance.state != GameState.BATTLE) 
        {
            currUnits = Chessboard.Instance.GetUnits();
            GameManager.Instance.ChangeGamestate(GameState.BATTLE);
        }
    }
    public void StopButton()
    {
        GameManager.Instance.ChangeGamestate(GameState.SCENARIO_BUILDER);
        Chessboard.Instance.RefreshBoard(currUnits);
    }

    //Grid
    public void AddGridX()
    {
        ChangeGridSize(1, 0);
    }
    public void SubtractGridX()
    {
        ChangeGridSize(-1, 0);
    }
    public void AddGridY()
    {
        ChangeGridSize(0, 1);
    }
    public void SubtractGridY()
    {
        ChangeGridSize(0, -1);
    }
    void ChangeGridSize(int changeX, int changeY)
    {
        Chessboard.Instance.AddTileCount(changeX, changeY);
        Vector2Int tileCount = Chessboard.Instance.GetTilecount();

        currUnits = Chessboard.Instance.GetUnits();
        var currNodes = Chessboard.Instance.nodes;
        if (currUnits == null)
        {
            Chessboard.Instance.RefreshBoard(null, currNodes);
            return;
        }

        Unit[,] newUnits = new Unit[tileCount.x, tileCount.y];
        for (int x = 0; x < currUnits.GetLength(0); x++)
        {
            for (int y = 0; y < currUnits.GetLength(1); y++)
            {
                if (currUnits[x, y] != null)
                {
                    var unit = currUnits[x, y];
                    if (x >= tileCount.x || y >= tileCount.y)
                    {
                        currUnits[x, y] = null;
                        GameManager.Instance.currentScenario.RemoveUnit(x, y);
                        unit.GetComponent<UnitHealth>().RemoveHP(Mathf.Infinity);
                    }
                    else
                    {
                        newUnits[x, y] = unit;
                    }
                }
            }
        }
        if (newUnits.Length > 0)
        {
            Chessboard.Instance.RefreshBoard(newUnits, currNodes);
            return;
        }
        Chessboard.Instance.RefreshBoard(null, currNodes);
    }

    public void ChooseGroundType(int type)
    {
        ScenarioBuilder.Instance?.SetToolCurrentNodeType(type);
    }

    //Saving
    string input = "";

    public void OpenSavingPanel()
    {
        ScenarioBuilder.Instance?.OpenPanel(ScenarioBuilderPanel.SAVE);
        GameManager.Instance?.SaveSlots.OpenSaveSlotsUI();
    }
    public void OpenLoadingPanel()
    {
        ScenarioBuilder.Instance?.OpenPanel(ScenarioBuilderPanel.SAVE);
        GameManager.Instance?.SaveSlots.OpenLoadSlotsUI();
    }
    public void OpenSaveConfirm(int slot)
    {
        GetComponentInParent<ScenarioSaving>().currSlot = slot;
    }
    public void ReadStringInput(string s)
    {
        input = s;
    }
    public void ConfirmSave()
    {
        if (input.Length > 0 && input.Length < 25)
        {
            GameManager.Instance.SaveSlots.SaveToSlot(GetComponentInParent<ScenarioSaving>().currSlot, input, ScenarioBuilder.Instance.camSettings.GetScenarioCameraRotation());
            ScenarioBuilder.Instance?.OpenPanel(ScenarioBuilderPanel.TERRAIN);
        }
    }
    public void CancelSave()
    {
        input = "";
        if (GetComponentInParent<ScenarioSaving>() != null)
            GetComponentInParent<ScenarioSaving>().currSlot = -1;
        ScenarioBuilder.Instance?.OpenPanel(ScenarioBuilderPanel.TERRAIN);
    }
    public void CancelLoad()
    {

    }

    public void LoadGame(int slot)
    {
        GameManager.Instance.SaveSlots.LoadFromSlot(slot);
        ScenarioBuilder.Instance?.OpenPanel(ScenarioBuilderPanel.TERRAIN);
    }
    public void MoveOnMap(MapNode node)
    {
        GameManager.Instance.MapController.MoveOnMap(node);
    }

    //Text Encounters
    TextEncounterManager encounters;
    public void ChooseTextResponse(int response)
    {
        if (encounters == null) 
            encounters = GameObject.Find("EncounterManager").GetComponentInChildren<TextEncounterManager>();

        encounters?.ChooseTextResponse(response);
    }
    public void CloseEncounter()
    {
        if (encounters == null)
            encounters = GameObject.Find("EncounterManager").GetComponentInChildren<TextEncounterManager>();

        encounters?.CloseEncounter();
    }
}