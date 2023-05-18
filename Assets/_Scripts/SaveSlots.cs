using System.Collections.Generic;
using UnityEngine;

public class SaveSlots : MonoBehaviour
{
    public Scenario scenario;
    public List<Scenario> saveSlots = new List<Scenario>();

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
        GameManager.Instance.SaveGameManager.LoadScenarios();
    }

    public void SaveToSlot(int slot)
    {
        if (slot >= 0 && slot < 10) {
            saveSlots[slot].SaveScenario();
        }
        print(saveSlots.Count);
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
            s.scenarioUnits = data.unitList;
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