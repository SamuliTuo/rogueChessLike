using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageNumbersTypes
{
    NORMAL, NORMAL_CRIT, HEAL, HEAL_CRIT
}

public class DamageNumbers : MonoBehaviour
{
    [SerializeField] private Vector2 particleLifetimeMinMax = Vector2.zero;
    [SerializeField] private float letterSpacing = 0.2f;
    [SerializeField] private float randomPositionOffset_downwards = 2;
    [SerializeField] private float randomPositionOffset_sides = 0.6f;
    [SerializeField] private float cameraUpTweakPerc = 0.3f;
    [SerializeField] private float cameraRightTweakPerc = 0.3f;
    [SerializeField] private float maxSize = 1;
    [SerializeField] private float maxSizeDamageCap = 5000f;
    [SerializeField] private float minSizePerc = 0.4f;
    [Header("Crit")]
    [SerializeField] private float critSizeMultiplier = 1.5f;
    [SerializeField] private float critTimeMultiplier = 1.5f;
    [SerializeField] private float critLetteringPosOffsetMultiplier = 1.5f;

    [Header("Different types of letters:")]
    public List<DamageNumbersType> particleSystems = new List<DamageNumbersType>();

    [Serializable]
    public class DamageNumbersType 
    {
        public List<DamageNumberPooling> pools;
        public DamageNumbersTypes type;
        public DamageNumbersType(List<DamageNumberPooling> pools, DamageNumbersTypes type)
        {
            this.pools = pools;
            this.type = type;
        }
    }
    //List<Mesh> numbers = new List<Mesh>();

    public void Reset()
    {
        //print("Trying to reset particle number pools, not necessary right now tho.");
        /*
        foreach (var num in particleSystems)
        {
            num.Reset();
        }*/
    }

    public IEnumerator InitNumberParticles(float damage, bool crit, Vector3 location, bool missed)
    {
        if (cam == null)
            cam = Camera.main;

        bool isHeal = false;
        if (damage < 0)
            isHeal = true;

        damage = Math.Abs(damage);
        string damageAsString = Mathf.RoundToInt(damage).ToString();
        float posOffset = (damageAsString.Length * 0.5f) * letterSpacing;
        Vector3 spawnPos = 
            location + 
            cam.transform.right * cameraRightTweakPerc 
            + cam.transform.up  * cameraUpTweakPerc 
            - Vector3.up * randomPositionOffset_downwards
            + cam.transform.right * UnityEngine.Random.Range(-randomPositionOffset_sides, randomPositionOffset_sides)
            + cam.transform.up * UnityEngine.Random.Range(-randomPositionOffset_sides * 0.34f, randomPositionOffset_sides * 0.34f);

        if (missed)
        {
            SpawnMiss(spawnPos, posOffset);
            yield break;
        }
            

        for (int i = 0; i < damageAsString.Length; i++)
        {
            char c = damageAsString[i];
            if (char.IsDigit(c))
            {
                int digit = int.Parse(c.ToString());
                SpawnNumber(digit, crit, isHeal, spawnPos, posOffset, damage);
                posOffset -= crit ? letterSpacing * critLetteringPosOffsetMultiplier : letterSpacing;
            }
            yield return null;
        }
    }

    void SpawnMiss(Vector3 _spawnPos, float _posOffset)
    {
        print("attack missed, missing miss sign");
    }

    Vector2 GetRandomOffsetForShootingDirection()
    {
        return new Vector2(UnityEngine.Random.Range(-45, -135), UnityEngine.Random.Range(0, 360));
    }

    Camera cam;
    void SpawnNumber(int num, bool crit, bool isHeal, Vector3 pos, float posOffset, float damage)
    {
        float perc = Mathf.Max(minSizePerc, Math.Min(maxSizeDamageCap, damage) / maxSizeDamageCap);
        //particleSystems[num].transform.position = pos + Vector3.left * (posOffset * perc);
        //particleSystems[num].transform.localScale = Vector3.one * perc;
        float lifetime = UnityEngine.Random.Range(particleLifetimeMinMax.x * perc, particleLifetimeMinMax.y * perc);

        DamageNumbersTypes type;
        if (isHeal)
        {
            if (crit)
                type = DamageNumbersTypes.HEAL_CRIT;
            else
                type = DamageNumbersTypes.HEAL;
        }
        else
        {
            if (crit)
                type = DamageNumbersTypes.NORMAL_CRIT;
            else
                type = DamageNumbersTypes.NORMAL;
        }
        //choose from systems
        float sizeMultiplier = crit ? critSizeMultiplier : 1f;
        float timeMultiplier = crit ? critTimeMultiplier : 1f;

        Vector2 randomDir = GetRandomOffsetForShootingDirection();
        foreach (var item in particleSystems)
            if (item.type == type)
                item.pools[num].SpawnNumber(
                    Vector3.one * maxSize * perc * sizeMultiplier, 
                    pos - cam.transform.right * (posOffset * perc),
                    randomDir,
                    lifetime * timeMultiplier);

        //ParticleSystem.MainModule psmain = particleSystems[num].main;
        //psmain.startLifetime = lifetime;
        //particleSystems[num].Play();
    }
}
