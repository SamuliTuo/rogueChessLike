using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParty : MonoBehaviour
{
    public Unit[,] partyUnits { get; private set; }
    public GameObject partyPanel;

    private int saveSlot = 0;

    public void RefreshParty()
    {
        if (Chessboard.Instance != null && partyUnits != null)
        {
            var oldParty = partyUnits;
            partyUnits = new Unit[Chessboard.Instance.GetTilecount().x, Chessboard.Instance.GetTilecount().y];

            for (int x = 0; x < oldParty.GetLength(0); x++)
            {
                for (int y = 0; y < oldParty.GetLength(1); y++)
                {
                    if (oldParty[x, y] != null)
                    {
                        AddUnit(oldParty[x, y]);
                    }
                }
            }
        }
    }

    public void AddUnit(Unit unit)
    {
        if (partyUnits == null)
            partyUnits = new Unit[GameManager.Instance.currentScenario.sizeX, GameManager.Instance.currentScenario.sizeY];

        for (int y = 0; y < partyUnits.GetLength(0); y++)
        {
            for (int x = 0; x < partyUnits.GetLength(1); x++)
            {
                if (partyUnits[x, y] == null)
                {
                    partyUnits[x, y] = unit;
                    return;
                }
            }
        }
    }

    // tätä ei oo testattu vvv
    public void SaveParty()
    {
        List<UnitData> partyData = new List<UnitData>();
        for (int y = 0; y < partyUnits.GetLength(0); y++)
        {
            for (int x = 0; x < partyUnits.GetLength(1); x++)
            {
                if (partyUnits[x, y] != null)
                {
                    partyData.Add(new UnitData(partyUnits[x, y], x, y));
                }
            }
        }
        SaveSystem.Save(partyData, "party" + saveSlot);
    }

    //Load party
    public void LoadParty()
    {
        List<UnitData> partyData = SaveSystem.Load<List<UnitData>>("party" + saveSlot);
        RefreshParty();
        if (partyData != null)
        {
            for (int i = 0; i < partyData.Count; i++)
            {
                UnitData data = partyData[i];
                var path = GameManager.Instance.UnitSavePaths.GetSavePath(data.unitName);
                Unit unit = Chessboard.Instance.SpawnSingleUnit(path, 0);
                AddUnit(unit);
            }
        }
    }

    public void OpenParty()
    {
        // Open the party panel
        partyPanel.SetActive(true);
        GameManager.Instance.MapController.SetCanMove(false);
    }
    public void CloseParty()
    {
        // Close the party panel
        partyPanel.SetActive(false);
        GameManager.Instance.MapController.SetCanMove(true);
    }
}
