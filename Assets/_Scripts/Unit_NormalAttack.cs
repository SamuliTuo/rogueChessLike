using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Unit_NormalAttack", order = 2)]
public class Unit_NormalAttack : ScriptableObject
{
    public UnitSearchType targeting;
    public ParticleType hitParticle;
    public DamageInstanceType dmgInstanceType;
    public ProjectileType projectileType;
    public int attackRange = 1;
    public float damage = 10f;
    public float attackInterval = 1f;
    public float attackFlySpeed = 10f;
    public float minLifeTime = 0;
    public GameObject projectile;
    public bool damagesAllies;
}
