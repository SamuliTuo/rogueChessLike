using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HPBarSpawner : MonoBehaviour
{
    private GameObject hpBarPrefab;
    private IObjectPool<GameObject> barPool;

    private void Start()
    {
        hpBarPrefab = Resources.Load<GameObject>("HpBarInstance");
        barPool = new ObjectPool<GameObject>(CreateBar, OnTakeBarFromPool, OnReturnBarToPool);
    }

    public GameObject SpawnBar()
    {
        return barPool.Get();
    }


    GameObject CreateBar()
    {
        var instance = Instantiate<GameObject>(hpBarPrefab);
        instance.GetComponent<HpBarInstance>().SetPool(barPool);
        return instance;
    }
    void OnTakeBarFromPool(GameObject bar)
    {
        bar.SetActive(true);
    }
    void OnReturnBarToPool(GameObject bar)
    {
        bar.SetActive(false);
    }
}
