using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NudgeUIController : MonoBehaviour
{
    [SerializeField] private Image fill = null;
    [SerializeField] private GameObject ready = null;

    public void UpdateCooldownUI(float fillPerc)
    {
        fill.fillAmount = fillPerc;
        if (fillPerc >= 1)
            ready.SetActive(true);
        else if (ready.activeSelf)
            ready.SetActive(false);
    }
}
