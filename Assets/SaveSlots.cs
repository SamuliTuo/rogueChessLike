using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlots : MonoBehaviour
{
    private List<Scenario> saveSlots = new List<Scenario>();

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            var stringi = "scenarios/saveSlot_0" + i.ToString();
            var blabla = Resources.Load<Scenario>(stringi);
            saveSlots.Add(blabla);
            //print("stringi: " + stringi + ", blabla: " + blabla + ", saveSlots[i]: " + saveSlots[i]);
        }
    }

    public void SaveToSlot(int slot)
    {
        if (slot >= 0 && slot < 10) {
            saveSlots[slot].SaveScenario();
        }
    }
    public void LoadFromSlot(int slot)
    {
        GameManager.Instance.currentScenario = saveSlots[slot];
        Chessboard.Instance.RefreshBoard();
    }
}
