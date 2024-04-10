using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePools : MonoBehaviour
{
    [System.Serializable]
    public class Projectiles
    {
        public string name;
        public GameObject projectile;
    }

    [Header("name is used when spawning projectiles")]
    //public List<Projectiles> projectiles = new List<Projectiles>();

    //private Dictionary<string, ProjectilePool> projectilePools = new Dictionary<string, ProjectilePool>();
    //private GameObject clone;

    public List<ProjectilePool> pools = new List<ProjectilePool>();

    //void Awake()
    //{
    //    FillPools();
    //}

    //function to create pools for each Projectiles in projectiles
    //public void FillPools()
    //{
    //    foreach (var item in projectiles)
    //    {
    //        if (!projectilePools.ContainsKey(item.name))
    //        {
    //            projectilePools.Add(item.name, new ProjectilePool(item.projectile, this));
    //        }
    //    }
    //}

    public void CreatePool(GameObject projectile)
    {
        if (projectile == null)
        {
            print("yritettiin tehdä pool null -projektiilille!");
            return;
        }
        foreach (var item in pools)
        {
            if (item.projectile == projectile)
            {
                return;
            }
        }
        pools.Add(new ProjectilePool(projectile, this));
    }

    public GameObject SpawnProjectile(GameObject projectile, Vector3 position, Quaternion rotation)
    {
        var pool = GetPool(projectile);
        if (pool == null)
        {
            Debug.Log("ERROR! Didnt find projectile pool. " + projectile+". Returning null projectile");
            return null;
        }
        var clone = pool.projectilePool.Get();
        clone.transform.position = position;
        clone.transform.rotation = rotation;
        //clone.GetComponent<DamageInstance>().Init(lifetime, dmgInterval, dmgPerInterval, poiseDmgPerInterval, radius);
        return clone;
    }

    public ProjectilePool GetPool(GameObject projectile)
    {
        foreach (var pool in pools)
        {
            if (pool.projectile == projectile)
            {
                return pool;
            }
        }
        return null;
    }

    //public void FillPools()
    //{
    //    foreach (var item in projectiles)
    //    {
    //        if (!projectilePools.ContainsKey(item.name))
    //        {
    //            projectilePools.Add(item.name, new ProjectilePool(item.projectile));
    //        }
    //    }
    //    foreach (var item in projectilePools)
    //    {
    //        print(item.Key);
    //    }
    //}

    public GameObject InstantiateProjectile(GameObject obj)
    {
        var clone = Instantiate(obj) as GameObject;
        return clone;
    }


    public void RefreshPools() 
    { 
        foreach (var item in pools)
        {
            item.projectilePool.Clear();
        }
    }
}


[Serializable]
public class ProjectilePool
{
    public ProjectilePools pools;
    public ObjectPool<GameObject> projectilePool;
    public GameObject projectile;

    public ProjectilePool(GameObject projectile, ProjectilePools pools)
    {
        this.projectile = projectile;
        this.pools = pools;
        if (projectilePool == null)
            projectilePool = new ObjectPool<GameObject>(CreateProjectile, OnTakeProjectileFromPool, OnReturnProjectileToPool);
    }

    GameObject CreateProjectile()
    {
        var instance = pools.InstantiateProjectile(projectile);

        // Projectile is auto attack:
        var proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.SetPool(projectilePool);
            return instance;
        }
        // Projectile is ability:
        var abil = projectile.GetComponent<AbilityInstance>();
        if (abil)
        {
            abil.SetPool(projectilePool);
            return instance;
        }

        return null;
    }

    void OnTakeProjectileFromPool(GameObject item)
    {
        item.SetActive(true);
    }
    void OnReturnProjectileToPool(GameObject item)
    {
        item.SetActive(false);
    }
    public void ClearPool()
    {
        projectilePool.Clear();
    }
}
