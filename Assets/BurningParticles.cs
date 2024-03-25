using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BurningParticles : MonoBehaviour
{
    private GameObject burnObj;
    private IObjectPool<GameObject> burnPool;
    private Dictionary<Unit, BurnCounterForUnit> burningUnits = new Dictionary<Unit, BurnCounterForUnit>();

    private void Start()
    {
        burnObj = transform.GetChild(0).gameObject;
        burnPool = new ObjectPool<GameObject>(CreateBurn, OnTakeBurnFromPool, OnReturnBurnToPool);
        burningUnits.Clear();
    }

    public void SetUnitsBurnCount(Unit unit, int burnCount)
    {

        if (burningUnits.ContainsKey(unit))
        {
            burningUnits[unit].SetBurnCount(burnCount);
            if (burnCount == 0)
            {
                burningUnits.Remove(unit);
            }
        }
        else
        {
            burningUnits.Add(unit, new BurnCounterForUnit(burnCount, burnPool, unit));
        }
    }

    public void StopBurn(Unit unit)
    {
        Unit removeUnit = null;
        foreach (KeyValuePair<Unit, BurnCounterForUnit> pair in burningUnits)
        {
            if (pair.Key == unit)
            {
                
                break;
            }
        }
        if (removeUnit != null) 
            burningUnits.Remove(removeUnit);
    }

    class BurnCounterForUnit
    {
        IObjectPool<GameObject> pool;
        public List<GameObject> burns;
        public int burnCount;
        public Unit unit;

        public BurnCounterForUnit(int burnCount, IObjectPool<GameObject> pool, Unit unit)
        {
            burns = new List<GameObject>();
            this.pool = pool;
            this.burnCount = burnCount;
            this.unit = unit;
            SetBurnCount(burnCount);
        }

        public void SetBurnCount(int burnCount)
        {
            this.burnCount = burnCount;

            while (burns.Count != burnCount)
            {
                if (burns.Count < burnCount)
                {
                    var r = pool.Get();
                    r.GetComponent<ParticleSystem>().Play();
                    r.GetComponent<StatusEffectParticle>().InitStatusParticle(unit);
                    burns.Add(r);
                }
                else if (burns.Count > burnCount)
                {
                    burns[burns.Count - 1].GetComponent<StatusEffectParticle>().Deactivate();
                    burns.RemoveAt(burns.Count - 1);
                }
            }
        }
    }



    // Boilerplate:
    GameObject CreateBurn()
    {
        var instance = Instantiate(burnObj, transform);
        instance.GetComponent<StatusEffectParticle>().SetPool(burnPool);
        return instance;
    }
    void OnTakeBurnFromPool(GameObject number)
    {
        number.gameObject.SetActive(true);
    }
    void OnReturnBurnToPool(GameObject number)
    {
        number.gameObject.SetActive(false);
    }
}
