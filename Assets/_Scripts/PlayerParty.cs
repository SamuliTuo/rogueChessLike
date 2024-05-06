using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerParty : MonoBehaviour
{
    public List<Tuple<UnitData, UnitInLibrary>> partyUnits { get; set; }
    //public GameObject partyPanel;
    public int partyMoney = 1000;
    public int maxPartySize = 10;

    private PartyMoneyCounter moneyManager;

    public void RefreshParty()
    {
        if (Chessboard.Instance != null && partyUnits != null)
        {
            var oldParty = partyUnits;
            partyUnits = new List<Tuple<UnitData, UnitInLibrary>>();

            for (int i = 0; i < oldParty.Count; i++)
            {
                AddUnit(oldParty[i].Item1, oldParty[i].Item2);
            }
        }
    }

    public Vector2Int GetFirstFreePartyPos()
    {
        if (partyUnits == null)
            partyUnits = new List<Tuple<UnitData, UnitInLibrary>>();

        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                bool isFree = true;
                for (int i = 0; i < partyUnits.Count; i++)
                {
                    if (partyUnits[i].Item1.spawnPosX == x && partyUnits[i].Item1.spawnPosY == y)
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

    public void AddUnit(UnitData unit, UnitInLibrary libraryEntry)
    {
        if (partyUnits == null)
            partyUnits = new List<Tuple<UnitData, UnitInLibrary>>();

        var spawnPos = GetFirstFreePartyPos();
        unit.spawnPosX = spawnPos.x;
        unit.spawnPosY = spawnPos.y;
        partyUnits.Add(new (unit, libraryEntry));
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

    public bool IsPartyFull()
    {
        if (partyUnits.Count >= maxPartySize)
        {
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        partyMoney += amount;
        RefreshMoneyCounter();

        // idk why it checked for MAP-state when after AND during battles money is gained.
        //if (GameManager.Instance.state == GameState.MAP)
        //{
        //    RefreshMoneyCounter();
        //}
    }

    public bool TryToUseMoney(int amount)
    {
        if (partyMoney >= amount)
        {
            partyMoney -= amount;
            RefreshMoneyCounter();
            return true;
        }
        return false;
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