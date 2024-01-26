using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class StunnedParticles : MonoBehaviour
{
    private GameObject stunObj;
    private IObjectPool<GameObject> stunPool;
    private Dictionary<Unit, GameObject> stunnedUnits = new Dictionary<Unit, GameObject>();

    private void Start()
    {
        stunObj = transform.GetChild(0).gameObject;
        stunPool = new ObjectPool<GameObject>(CreateNumber, OnTakeNumberFromPool, OnReturnNumberToPool);
        stunnedUnits.Clear();
    }

    public void SpawnStun(Unit unit)
    {
        if (stunnedUnits.ContainsKey(unit))
            return;

        var r = stunPool.Get();
        r.GetComponent<StunnedParticle>().InitStun(unit);
        stunnedUnits.Add(unit, r);
    }
    public void StopStun(Unit unit)
    {
        Unit remove = null;
        foreach (KeyValuePair<Unit, GameObject> pair in stunnedUnits)
        {
            if (pair.Key == unit)
            {
                pair.Value.GetComponent<StunnedParticle>().Deactivate();
                remove = pair.Key;
                break;
            }
        }
        if (remove != null) 
            stunnedUnits.Remove(remove);
    }


    // Boilerplate:
    GameObject CreateNumber()
    {
        var instance = Instantiate(stunObj, transform);
        instance.GetComponent<StunnedParticle>().SetPool(stunPool);
        return instance;
    }
    void OnTakeNumberFromPool(GameObject number)
    {
        number.gameObject.SetActive(true);
    }
    void OnReturnNumberToPool(GameObject number)
    {
        number.gameObject.SetActive(false);
    }
}
