using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewUnitAbility", order = 4)]
public class UnitAbility : ScriptableObject
{
    //PartyMemberFindingOrder ???
    public UnitSearchType targetSearchType;
    public UnitSearchType validTargets;
    public bool centerOnYourself;
    public ParticleType hitParticle;
    public DamageInstanceType dmgInstanceType;
    public ProjectileType projectileType;
    public GameObject projectile;
    public int reach = 1;

    [Space(10)]
    public float damage = 10f;
    public float castSpeed = 1;
    public float cooldown = 10f;
    public float flySpeed = 10f;
    public float minLifeTime = 0;
    public bool damagesAllies;

    [Header("Bouncing and multiplying:")]
    public Unit_NormalAttack bounceAttack = null;
    public UnitSearchType bounceAttack_targeting = UnitSearchType.ENEMIES_ONLY;
    public int bounceRange_attack = 1;
    public int bounceCount_atk = 0;
    public int bounceSpawnCount_atk = 0;
    public UnitAbility bounceAbility = null;
    public UnitSearchType bounceAbility_targeting = UnitSearchType.ENEMIES_ONLY;
    public int bounceRange_ability = 1;
    public int bounceCount_ability = 0;
    public int bounceSpawnCount_ability = 0;
    
    [Header("These are not used for anything, thinking...")]
    public float bounceDamagePercChangePerJump = 1;
    public bool onlyOneBouncePerUnit = true;

    [Header("Spawn stuff!")]
    public GameObject spawnUnit = null;
    public int spawnCount = 1;
    public float spawnDuration;

}
