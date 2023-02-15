using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public void GoToBattleMode()
    {
        GameManager.Instance.ChangeGamestate(GameState.BATTLE);
    }
}
