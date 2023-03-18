using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static List<Scenario> scenarios = new List<Scenario>();

    const string SCENARIO_KEY = "/scenario";
    const string SCENARIO_COUNT_KEY = "/scenario.count";

    public void Save()
    {
        string key = SCENARIO_KEY;
        string countKey = SCENARIO_COUNT_KEY;

        SaveSystem.Save(scenarios.Count, countKey);

        for (int i = 0; i < scenarios.Count; i++)
        {
            print(scenarios[i]);
            ScenarioData data = new ScenarioData(scenarios[i]);

            SaveSystem.Save(data, key + i);
            print("Saving. . .  units: " + scenarios[i].scenarioUnits + ", scenario: " + scenarios[i]);
        }
    }

    public void Load()
    {
        string key = SCENARIO_KEY;
        string countKey = SCENARIO_COUNT_KEY;

        int count = SaveSystem.Load<int>(countKey);
        print(count);
        var saveSlots = GameObject.Find("SaveSlots").GetComponent<SaveSlots>();
        scenarios = new List<Scenario>(saveSlots.saveSlots);
        for (int i = 0; i < count; i++)
        {
            ScenarioData data = SaveSystem.Load<ScenarioData>(key + i);
            Scenario s = saveSlots.LoadSlot(data, i);
            //print("data: " + data + ", scen: " + s);
            if (s != null)
                scenarios[i] = s;
        }
        foreach (var scenario in scenarios)
        {
            //print("Loading. . .  units: " + scenario.scenarioUnits + ", scenario: " + scenario);
        }
    }
}



//public void Load()
//{
//    string key = SCENARIO_KEY;
//    string countKey = SCENARIO_COUNT_KEY;

//    int count = SaveSystem.Load<int>(countKey);

//    var saveSlots = GameObject.Find("SaveSlots").GetComponent<SaveSlots>();
//    for (int i = 0; i < count; i++)
//    {
//        ScenarioData data = SaveSystem.Load<ScenarioData>(key + i);
//        Scenario s = saveSlots.LoadSlot(data, i);
//        if (s != null) 
//            scenarios.Add(saveSlots.LoadSlot(data, i));

//        //scenarios.Add(GameManager.Instance.SaveSlots.saveSlots[i]);


//        //Scenario
//        //ScenarioData data = SaveSystem.Load<ScenarioData>(key + i);
//    }
//    foreach (var scenario in scenarios)
//    {
//        print("Loading. . .  units: " + scenario.scenarioUnits + ", scenario: " + scenario);
//    }
//}//chatGPTN versio vaihdettu