using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostGamePanel : MonoBehaviour
{
    public void RestartButton()
    {
        GameManager.Instance.StartCoroutine("BattleEnd", "MapScene");
    }

    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
