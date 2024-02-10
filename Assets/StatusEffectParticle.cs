using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class StatusEffectParticle : MonoBehaviour
{
    private IObjectPool<GameObject> pool;
    private Transform unit;
    private float positionOffset_up = 1;

    public void SetPool(IObjectPool<GameObject> pool) => this.pool = pool;
    bool released = true;

    public void InitStatusParticle(Unit unit)
    {
        this.unit = unit.transform;
        released = false;
        positionOffset_up = unit.GetComponent<UnitHealth>().hpBarOffset - 0.3f;
        StartCoroutine(EffectUpdate());
    }

    IEnumerator EffectUpdate()
    {
        while (!released)
        {
            transform.position = unit.transform.position + Vector3.up * positionOffset_up;
            yield return null;
        }
    }

    public void Deactivate()
    {
        if (!released)
        {
            StopAllCoroutines();
            released = true;
            if (pool != null)
                pool.Release(gameObject);
            else
                Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (!released)
        {
            released = true;
            if (pool != null)
                pool.Release(gameObject);
        }

    }
}
