using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] private GameObject moneySmall = null;
    [SerializeField] private GameObject moneyMedium = null;
    [SerializeField] private GameObject moneyLarge = null;

    [SerializeField] private float upForce = 0;
    [SerializeField] private float rightForce = 0;
    [SerializeField] private float backForce = 0;

    public void SpawnMoney(int amount, Vector3 location)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject clone;
            if (amount < 5)
                clone = Instantiate(moneySmall);
            else if (amount < 10)
                clone = Instantiate(moneyMedium);
            else
                clone = Instantiate(moneyLarge);

            clone.transform.position = location + Vector3.up;
            clone.GetComponent<Rigidbody>().AddForce(Vector3.up * upForce * Random.Range(0.50f, 1.00f) + Vector3.right * rightForce * Random.Range(-1.00f, 1.00f) + Vector3.back * backForce * Random.Range(-1.00f, 1.00f), ForceMode.Impulse);
            clone.GetComponent<LootInstance>().InitMoney(amount);
        }
    }

    public void SpawnLoot(GameObject loot)
    {
        var clone = Instantiate(loot);
        clone.GetComponent<LootInstance>().InitItem(loot);
    }
}
