using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SaveSlots : MonoBehaviour
{
    public Scenario scenario;
    public List<Scenario> saveSlots = new List<Scenario>();
    public List<Transform> saveSlotsOnCanvas = new List<Transform>();
    public List<Transform> loadSlotsOnCanvas = new List<Transform>();

    private void Awake()
    {
        // Load the saveSlot - Scriptable Objects
        for (int i = 0; i < 10; i++)
        {
            var stringi = "scenarios/saveSlot_0" + i.ToString();
            var blabla = Resources.Load<Scenario>(stringi);
            saveSlots.Add(blabla);
        }
    }


    private void Start()
    {
        if (GameManager.Instance.state == GameState.SCENARIO_BUILDER)
        {
            // Find the slots on the canvas if in builder mode
            var canvasSlots = GameObject.Find("SavingAndLoading").transform;
            saveSlotsOnCanvas.Clear();
            loadSlotsOnCanvas.Clear();
            for (int i = 0; i < 10; i++)
            {
                saveSlotsOnCanvas.Add(canvasSlots.GetChild(2).GetChild(0).GetChild(i));
            }
            for (int i = 0; i < 10; i++)
            {
                loadSlotsOnCanvas.Add(canvasSlots.GetChild(3).GetChild(0).GetChild(i));
            }
        }

        GameManager.Instance.SaveGameManager.LoadScenarios();
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


//public Scenario LoadSlot(ScenarioData data, int slot)
//{
//    Scenario r = null;
//    if (saveSlots.Count == 0)
//    {
//        saveSlots = SaveGameManager.scenarios;
//    }
//    if (saveSlots[slot] != null)
//    {
//        var s = saveSlots[slot];
//        saveSlots[slot] = Resources.Load<Scenario>("scenarios/" + data.scriptedScenarioName);
//        saveSlots[slot].scenarioUnits = data.unitList;
//        r = saveSlots[slot];
//    }
//    return r;
//}
//chatGPTN versioon vaihdettu








//if (SaveGameManager.scenarios.Count > 0)
//{
//    print("nappasin saveSlotit saveGameManagerilta, t: SaveSlots");
//    saveSlots = SaveGameManager.scenarios;
//    return;
//}

//for (int i = 0; i < 10; i++)
//{
//    var stringi = "scenarios/saveSlot_0" + i.ToString();
//    var blabla = Resources.Load<Scenario>(stringi);
//    saveSlots.Add(blabla);
//    SaveGameManager.scenarios.Add(blabla);
//    print("SaveSlot: Luon uutta saveslottia, i = " + i + ", string = " + stringi + ",      saveGemeManager " + i + ": " + SaveGameManager.scenarios[i]);
//    //print("stringi: " + stringi + ", blabla: " + blabla + ", saveSlots[i]: " + saveSlots[i]);
//}
//for (int i = 0; i < saveSlots.Count; i++)
//{
//    print("saveslot " + i + ", " + saveSlots[i].name);
//}
//for (int i = 0; i < SaveGameManager.scenarios.Count; i++)
//{
//    print("sageGameManager scenario " + i + ", " + SaveGameManager.scenarios[i].name);
//}