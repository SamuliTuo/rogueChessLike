using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewUnitAbility", order = 4)]
public class UnitAbility : ScriptableObject
{
    public UnitSearchType targetingMode;
    public ParticleType hitParticle;
    public DamageInstanceType dmgInstanceType;
    public ProjectileType projectileType;
    public GameObject projectile;
    public int reach = 1;
    public float damage = 10f;
    public float castSpeed = 1;
    public float cooldown = 10f;
    public float flySpeed = 10f;
    public float minLifeTime = 0;
    public bool damagesAllies;
}
