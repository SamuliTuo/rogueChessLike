using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private Unit[,] quickSave;
    public void PlayButton()
    {
        if (GameManager.Instance.state != GameState.BATTLE) 
        {
            quickSave = Chessboard.Instance.GetUnits();
            GameManager.Instance.ChangeGamestate(GameState.BATTLE);
        }
    }
    public void StopButton()
    {
        Chessboard.Instance.RefreshBoard(quickSave);
        GameManager.Instance.ChangeGamestate(GameState.SCENARIO_BUILDER);
    }

    public void AddGridX()
    {
        Chessboard.Instance.AddTileCount(1, 0);
    }
    public void SubtractGridX()
    {
        Chessboard.Instance.AddTileCount(-1, 0);
    }
    public void AddGridY()
    {
        Chessboard.Instance.AddTileCount(0, 1);
    }
    public void SubtractGridY()
    {
        Chessboard.Instance.AddTileCount(0, -1);
    }
    public void SaveGame(int slot)
    {
        GameManager.Instance.SaveSlots.SaveToSlot(slot);
    }
    public void LoadGame(int slot)
    {
        GameManager.Instance.SaveSlots.LoadFromSlot(slot);
    }
}
