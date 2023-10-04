using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PartyMoneyCounter : MonoBehaviour
{
    TextMeshProUGUI moneyCounter;

    public void SetMoneyAmount(int moneyAmount)
    {
        if (moneyCounter == null)
        {
            moneyCounter = GetComponentInChildren<TextMeshProUGUI>();
        }
        moneyCounter.text = moneyAmount.ToString();
    }
}