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

    [Header("Bouncing and multiplying:")]
    public Unit_NormalAttack bounceAttack = null;
    public UnitSearchType bounceAttack_targeting = UnitSearchType.ENEMIES_ONLY;
    public int bounceRange_atk = 1;
    public int bounceCount_atk = 0;
    public int bounceSpawnCount_atk = 0;
    [Space(4)]
    public UnitAbility bounceAbility = null;
    public UnitSearchType bounceAbility_targeting = UnitSearchType.ENEMIES_ONLY;
    public int bounceRange_ability = 1;
    public int bounceCount_ability = 0;
    public int bounceSpawnCount_ability = 0;
    [Header("This is not used for anything, thinking...")]
    public float bounceDamagePercChangePerJump = 1;
    public bool onlyOneBouncePerUnit = true;
}
