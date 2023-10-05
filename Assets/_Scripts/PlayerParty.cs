using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerParty : MonoBehaviour
{
    public List<UnitData> partyUnits { get; set; }
    //public GameObject partyPanel;
    public int partyMoney = 1000;

    private PartyMoneyCounter moneyManager;
    private int saveSlot = 0;

    public void RefreshParty()
    {
        if (Chessboard.Instance != null && partyUnits != null)
        {
            var oldParty = partyUnits;
            partyUnits = new List<UnitData>();

            for (int i = 0; i < oldParty.Count; i++)
            {
                AddUnit(oldParty[i]);
            }
        }
    }

    public Vector2Int GetFirstFreePartyPos()
    {
        if (partyUnits == null)
            partyUnits = new List<UnitData>();

        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                bool isFree = true;
                for (int i = 0; i < partyUnits.Count; i++)
                {
                    if (partyUnits[i].spawnPosX == x && partyUnits[i].spawnPosY == y)
                    {
                        isFree = false;
                        break;
                    }
                }
                if (isFree)
                    return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(-1, -1);
    }

    public void AddUnit(UnitData unit)
    {
        if (partyUnits == null)
            partyUnits = new List<UnitData>();

        var spawnPos = GetFirstFreePartyPos();
        unit.spawnPosX = spawnPos.x;
        unit.spawnPosY = spawnPos.y;
        partyUnits.Add(unit);
    }

    // tätä ei oo testattu vvv
    public void SaveParty()
    {
        /*List<UnitData> partyData = new List<UnitData>();
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
        */
    }

    //Load party
    public void LoadParty()
    {
        /*List<UnitData> partyData = SaveSystem.Load<List<UnitData>>("party" + saveSlot);
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
        }*/
    }
    /*
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
    }*/

    public void AddMoney(int amount)
    {
        partyMoney += amount;
        if (GameManager.Instance.state == GameState.MAP)
        {
            RefreshMoneyCounter();
        }
    }

    public void RefreshMoneyCounter()
    {
        if (moneyManager == null)
        {
            moneyManager = GameObject.Find("Canvas/money").GetComponent<PartyMoneyCounter>();
        }
        moneyManager.SetMoneyAmount(partyMoney);
    }
}