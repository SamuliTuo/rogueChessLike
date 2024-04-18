using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum UnitAugment
{
    BEST_FRIENDS,
    BURNING_ATTACKS,
    ICY_ATTACKS,
    ACCELERATING_ATTACKS,
    BLOOD_MONEY,
    MONEY_GRUBBER,
    RANDOM_ITEM,
    RANDOM_ARTIFACT,
    FAST_LEARNER,
    ITEM_HEALTH,
    COOLDOWNER,
    EMPTY_HANDS,
    LIFE_STEALER,
    KILL_HEALS,
}

public class UnitAugments : MonoBehaviour
{
    public List<Augment> augments = new List<Augment>();

    public int bloodMoneyAmount = 10;
    public int moneyGrubberAmount = 10;
    public float fastLearnerMult = 1.15f;
    public float itemHealthAmount = 50f;
    public float cooldownerReduction = 0.20f;
    public float lifeStealerAmount = 0.1f;
    public float killHealsAmount = 100f;
    public float bestFriendsAttackSPD = 20;
    public float bestFriendsArmor = 3;
    [Space(15)]
    public float burningAttacksTickDamage = 15f;
    public int burningAttacksTickCount = 2;
    public float burningAttacksTickInterval = 1f;

    //private void Start()
    //{
    //    GetRandomAugment();
    //}


    public List<Augment> GetRandomAugments(int amount, List<UnitAugment> existingAugments)
    {
        List<UnitAugment> l = new List<UnitAugment>();
        List<Augment> r = new List<Augment>();
        l.AddRange(existingAugments);
        for (int i = 0; i < amount; i++)
        {
            var aug = GetRandomAugment(l);
            l.Add(aug);
            r.Add(GetAugmentItem(aug));
        }
        return r;
    }
    
    public UnitAugment GetRandomAugment()
    {
        var f = (UnitAugment)UnityEngine.Random.Range(0, Enum.GetValues(typeof(UnitAugment)).Length);
        return f;
    }

    public UnitAugment GetRandomAugment(List<UnitAugment> existingAugments)
    {
        var l = new List<UnitAugment>();
        foreach (var a in Enum.GetValues(typeof(UnitAugment))) 
        {
            if (existingAugments.Contains((UnitAugment)a))
            {
                continue;
            }
            else
            {
                l.Add((UnitAugment)a);
            }
        }
        return l[UnityEngine.Random.Range(0, l.Count)];
    }

    public Augment GetAugmentItem(UnitAugment augment)
    {
        foreach (var aug in augments)
        {
            if (aug.augmentType == augment)
            {
                return aug;
            }
        }
        return null;
    }

    [Serializable]
    public class Augment
    {
        public string name;
        public UnitAugment augmentType;
        public Sprite image;
    }

}
