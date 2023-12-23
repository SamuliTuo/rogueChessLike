using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SaveSlots : MonoBehaviour
{
    public int saveSlotsCount;

    public Scenario scenario;
    public List<Scenario> saveSlots = new List<Scenario>();
    public List<Transform> saveSlotsOnCanvas = new List<Transform>();
    public List<Transform> loadSlotsOnCanvas = new List<Transform>();

    private void Awake()
    {
        saveSlots = GetScenarioReferences();
    }


    private void Start()
    {
        if (GameManager.Instance.state == GameState.SCENARIO_BUILDER)
        {
            // Find the slots on the canvas if in builder mode
            var canvasSlots = GameObject.Find("SavingAndLoading").transform;
            saveSlotsOnCanvas.Clear();
            loadSlotsOnCanvas.Clear();
            for (int i = 0; i < saveSlotsCount; i++)
            {
                saveSlotsOnCanvas.Add(canvasSlots.GetChild(2).GetChild(0).GetChild(i));
            }
            for (int i = 0; i < saveSlotsCount; i++)
            {
                loadSlotsOnCanvas.Add(canvasSlots.GetChild(3).GetChild(0).GetChild(i));
            }
        }

        //GameManager.Instance.SaveGameManager.LoadScenarios();
    }


    public void OpenSaveSlotsUI()
    {
        for (int i = 0; i < saveSlotsOnCanvas.Count; i++)
        {
            saveSlotsOnCanvas[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = saveSlots[i].saveName;
        }
    }
    public void OpenLoadSlotsUI() 
    {
        for (int i = 0; i < loadSlotsOnCanvas.Count; i++)
        {
            loadSlotsOnCanvas[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = saveSlots[i].saveName;
        }
    }

    public List<Scenario> GetScenarioReferences()
    {
        var r = new List<Scenario>();
        // Load the saveSlot - Scriptable Objects
        for (int i = 0; i < saveSlotsCount; i++)
        {
            string slotName;
            if (i < 10)
                slotName = "scenarios/saveSlot_0" + i.ToString();
            else
                slotName = "scenarios/saveSlot_" + i.ToString();

            var scenarioScriptable = Resources.Load<Scenario>(slotName);
            
            r.Add(scenarioScriptable);
        }
        return r;
    }

    public void SaveToSlot(int slot, string saveName, Vector3 cameRot)
    {
        if (slot >= 0 && slot < saveSlots.Count) {
            saveSlots[slot].SaveScenario(saveName, cameRot);
            saveSlotsOnCanvas[slot].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = saveName;
        }
        GameManager.Instance.SaveGameManager.SaveScenarios();
    }

    public void LoadFromSlot(int slot)
    {
        //scenario = GameManager.Instance.currentScenario = saveSlots[slot];
        scenario = Resources.Load<Scenario>("scenarios/" + saveSlots[slot].name);
        GameManager.Instance.currentScenario = scenario;
        Chessboard.Instance.RefreshBoard();
    }

    public Scenario LoadSlot(ScenarioData data, int slot)
    {
        if (saveSlots.Count < saveSlotsCount)
        {
            saveSlots = GetScenarioReferences();
        }

        Scenario s = null;
        if (saveSlots[slot] != null)
        {
            s = saveSlots[slot];
            s.saveName = data.saveName;
            s.sizeX = data.boardSizeX;
            s.sizeY = data.boardSizeY;
            s.cameraRotationX = data.cameraRotationX;
            s.cameraRotationY = data.cameraRotationY;
            s.cameraRotationZ = data.cameraRotationZ;
            s.scenarioUnits = data.unitList;
            s.scenarioNodes = data.scenarioNodes;
        }
        return s;
    }
}