using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootInstance : MonoBehaviour
{
    public int moneyAmount;
    public GameObject item;

    public void InitMoney(int amount)
    {
        this.moneyAmount = amount;
    }

    public void InitItem(GameObject item)
    {
        this.item = item;
    }


    public void LootMe()
    {
        print(this.name+"im looted");
        Destroy(this.gameObject);
    }
}
