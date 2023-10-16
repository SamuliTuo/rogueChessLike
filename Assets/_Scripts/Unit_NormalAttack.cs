using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Unit_NormalAttack", order = 2)]
public class Unit_NormalAttack : ScriptableObject
{
    public UnitSearchType targeting;
    public ParticleType hitParticle;
    public DamageInstanceType dmgInstanceType;
    public ProjectileType projectileType;
    public UnitStatusModifier statusModifiers;
    public int attackRange = 1;
    public float damage = 10f;
    public float attackDuration_firstHalf = 1f;
    public float attackDuration_secondHalf = 1f;
    public float attackFlySpeed = 10f;
    public float minLifeTime = 0;

    [Header("Full path of projectile in Resources folder, ie: units/ranger/projectile1")]
    public string projectilePath;

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
