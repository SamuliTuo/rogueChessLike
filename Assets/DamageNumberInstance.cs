using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DamageNumberInstance : MonoBehaviour
{
    IObjectPool<DamageNumberInstance> pool;
    public void SetPool(IObjectPool<DamageNumberInstance> pool) => this.pool = pool;
    private bool released = true;

    public void Init(Vector3 scale, Vector3 position, Vector2 shootingDirectionOffset, float lifeTime)
    {
        released = false;
        transform.localScale = scale;
        transform.position = position;
        ParticleSystem.MainModule system = GetComponent<ParticleSystem>().main;
        system.startLifetime = lifeTime;
        ParticleSystem.ShapeModule shape = GetComponent<ParticleSystem>().shape;
        shape.rotation.Set(shootingDirectionOffset.x, shootingDirectionOffset.y, 0);
        GetComponent<ParticleSystem>().Play();
        StartCoroutine(InstanceRecycler(lifeTime));
    }
    IEnumerator InstanceRecycler(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime + 0.3f);
        Deactivate();
    }

    public void Deactivate()
    {
        if (!released)
        {
            released = true;
            if (pool != null)
                pool.Release(this);
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
                pool.Release(this);
        }

    }
}
