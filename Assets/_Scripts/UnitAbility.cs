using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewUnitAbility", order = 4)]
public class UnitAbility : ScriptableObject
{
    public UnitSearchType targetSearchType;
    public UnitSearchType validTargets;
    public bool centerOnYourself;
    public ParticleType hitParticle;
    public DamageInstanceType dmgInstanceType;
    public AOEShapes aoeShape;
    public ProjectileType projectileType;

    [Header("Full path of projectile in Resources folder, ie: units/ranger/projectile1")]
    public string projectilePath;

    public int reach = 1;

    [Space(10)]
    public float damage = 10f;
    public bool usesMagic = true;
    public UnitStatusModifier directHitStatusModifier;
    public float castDuration_firstHalf = 1;
    [Tooltip("secondHalf is after additional damage phases")]
    public float castDuration_secondHalf = 1;
    [Tooltip("additional phases are used for multi-hit abilities, add.phases happen between 1st half and 2nd half")]
    public int additionalDamagePhases = 0;
    public float additionalDamagePhaseDuration = 0;
    public float cooldown = 10f;
    [Tooltip("cooldown multiplied by this is how long the spell has cooldown at Start")]
    public float startCooldownMultiplier = 0.5f;
    public float flySpeed = 10f;
    public float minLifeTime = 0;
    public bool damagesAllies;

    [Header("Damage leaves an area of effect:")]
    public bool spawnAreaDOT = false;
    public UnitStatusModifier areaDOTStatusModifier;
    public DamageInstanceType areaDOTType;
    public AOEShapes areaDotShape;
    public UnitSearchType areaDOTValidTargets;
    public ParticleType areaDOTParticle;
    public float tickDamage = 0;
    public float tickIntervalSeconds = 0;
    public int intervalCount = 0;

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

    [Header("Leave spawnUnit empty if nothing spawns, else the name of unit")]
    public string spawnUnit = null;
    public int spawnCount = 1;
    public float spawnDuration;


    [Header("Upgrade potentials")]
    public bool damage_upgradeable = false;
    public bool cooldown_upgradeable = false;
    public bool castSpeed_upgradeable = false;
    public bool flySpeed_upgradeable = false;
    public bool reach_upgradeable = false;

    public bool bounceCount_upgradeable = false;
    public bool bounceRange_upgradeable = false;
    public bool bounceDamageAmp_upgradeable = false;
    public bool projectilesPerBounce_upgradeable = false;

    public bool spawnUnitCount_upgradeable = false;
    
}
