using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
    [SerializeField] private Vector2 particleLifetimeMinMax = Vector2.zero;
    [SerializeField] private float positionOffset = 0.2f;
    [SerializeField] private float maxSizeDamageCap = 5000f;
    [SerializeField] private float minSizePerc = 0.4f;

    List<DamageNumberPooling> particleSystems = new List<DamageNumberPooling>();
    //List<Mesh> numbers = new List<Mesh>();

    public void Reset()
    {
        print("Trying to reset particle number pools, not necessary right now tho.");
        /*
        foreach (var num in particleSystems)
        {
            num.Reset();
        }*/
    }
    
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            particleSystems.Add(transform.GetChild(i).GetComponent<DamageNumberPooling>());
            //string location = "particleSystems/numbers/number_" + i.ToString();
            //numbers.Add((Resources.Load(location) as GameObject).GetComponent<MeshFilter>().sharedMesh);
        }
    }

    public IEnumerator InitNumberParticles(float damage, Vector3 location)
    {
        bool isHeal = false;
        if (damage < 0)
            isHeal = true;

        damage = Math.Abs(damage);
        string damageAsString = Mathf.RoundToInt(damage).ToString();
        float posOffset = positionOffset * (damageAsString.Length * 0.5f);
        //print("Damagio: " + damage + ", Character count: " + damageAsString.Length + ", PosOffset: " + posOffset);
        for (int i = 0; i < damageAsString.Length; i++)
        {
            char c = damageAsString[i];
            if (char.IsDigit(c))
            {
                int digit = int.Parse(c.ToString());
                if (isHeal)
                    digit += 10;

                SpawnNumber(digit, location, posOffset, damage);
                posOffset -= positionOffset;
            }
            yield return null;
        }
    }

    void SpawnNumber(int num, Vector3 pos, float posOffset, float damage)
    {
        float perc = Mathf.Max(minSizePerc, Math.Min(maxSizeDamageCap, damage) / maxSizeDamageCap);
        //particleSystems[num].transform.position = pos + Vector3.left * (posOffset * perc);
        //particleSystems[num].transform.localScale = Vector3.one * perc;
        float lifetime = UnityEngine.Random.Range(particleLifetimeMinMax.x * perc, particleLifetimeMinMax.y * perc);
        particleSystems[num].SpawnNumber(Vector3.one * perc, pos + Vector3.left * (posOffset * perc), lifetime);
        //ParticleSystem.MainModule psmain = particleSystems[num].main;
        //psmain.startLifetime = lifetime;
        //particleSystems[num].Play();
    }
}
