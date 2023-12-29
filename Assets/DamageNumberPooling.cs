using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DamageNumberPooling : MonoBehaviour
{
    private DamageNumberInstance numberObject;
    private IObjectPool<DamageNumberInstance> numberPool;


    private void Start()
    {
        numberObject = GetComponentInChildren<DamageNumberInstance>();
        numberPool = new ObjectPool<DamageNumberInstance>(CreateNumber, OnTakeNumberFromPool, OnReturnNumberToPool);
        Reset();
    }


    public void Reset()
    {
        //if (numberPool != null)   
        //    numberPool.Clear();

        //numberPool = new ObjectPool<DamageNumberInstance>(CreateNumber, OnTakeNumberFromPool, OnReturnNumberToPool);
    }

    public DamageNumberInstance SpawnNumber(Vector3 originalScale, Vector3 position, float lifeTime)
    {
        var r = numberPool.Get();
        r.Init(originalScale, position, lifeTime);
        return r;
    }


    // Boilerplate:
    DamageNumberInstance CreateNumber()
    {
        var instance = Instantiate(numberObject, transform);
        instance.GetComponent<DamageNumberInstance>().SetPool(numberPool);
        return instance;
    }
    void OnTakeNumberFromPool(DamageNumberInstance number)
    {
        number.gameObject.SetActive(true);
    }
    void OnReturnNumberToPool(DamageNumberInstance number)
    {
        number.gameObject.SetActive(false);
    }
}
