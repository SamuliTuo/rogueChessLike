using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public enum ParticleType
{
    NONE,
    BASIC_CAST_PUFF,
    CLERIC_BOOMBA,
    RANGER_STABB,
    WARRIOR_HIT,
    ZZZAP,
    HEAL_PARTICLES,
    FIRE_EXPLOSION,
    NOTE_RING,
    SPARKS_JUMPY,
    JUMP_CLOUDS,

    ATTACK_MISS,
    //penguin
    FIRE_BREATH,
}

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] ParticleSystem basicCastPuff;
    [SerializeField] ParticleSystem clericBomba;
    [SerializeField] ParticleSystem rangerStabb;
    [SerializeField] ParticleSystem warriorHit;
    [SerializeField] ParticleSystem zzzap;
    [SerializeField] ParticleSystem healParticles;
    [SerializeField] ParticleSystem fireExplosion;
    [SerializeField] ParticleSystem noteRing;
    [SerializeField] ParticleSystem sparks_jumpy;
    [SerializeField] ParticleSystem jump_clouds;

    [Header("General:")]
    [SerializeField] DamageNumbers damageNumbers;
    [SerializeField] ParticleSystem attackMissed;
    [SerializeField] StunnedParticles stunnedParticles;
    [SerializeField] BurningParticles burnParticles;

    [Header("Penguin:")]
    [SerializeField] private ParticleSystem fireBreath;

    public void SpawnParticles(ParticleType type, Vector3 pos, Vector3 forw)
    {
        switch (type)
        {
            case ParticleType.BASIC_CAST_PUFF: PlayParticle(basicCastPuff, pos, forw); break;
            case ParticleType.CLERIC_BOOMBA: PlayParticle(clericBomba, pos, forw); break;
            case ParticleType.RANGER_STABB: PlayParticle(rangerStabb, pos, forw); break;
            case ParticleType.WARRIOR_HIT: PlayParticle(warriorHit, pos, forw); break;
            case ParticleType.ZZZAP: PlayParticle(zzzap, pos, forw); break;
            case ParticleType.HEAL_PARTICLES: PlayParticle(healParticles, pos, forw); break;
            case ParticleType.FIRE_EXPLOSION: PlayParticle(fireExplosion, pos, forw); break;
            case ParticleType.NOTE_RING: PlayParticle(noteRing, pos, forw); break;
            case ParticleType.SPARKS_JUMPY: PlayParticle(sparks_jumpy, pos, forw); break;
            case ParticleType.FIRE_BREATH: PlayParticle(fireBreath, pos, forw); break;
            case ParticleType.JUMP_CLOUDS: PlayParticle(jump_clouds, pos, forw); break;
            case ParticleType.ATTACK_MISS: PlayParticle(attackMissed, pos, forw); break;
            default: break;
        }
    }




    public void Reset()
    {
        damageNumbers.Reset();
    }


    void PlayParticle(ParticleSystem s, Vector3 pos, Vector3 forw)
    {
        s.transform.position = pos;
        s.transform.LookAt(pos + forw, Vector3.up);
        s.Play();
        var source = s.GetComponent<AudioSource>();

        if (source == null)
            return;

        source.pitch = Random.Range(0.80f, 1.20f);
        source.Play();
    }

    // Damage numbers
    public void InitDamageNumbers(float dmg, bool crit, Vector3 pos, bool missed)
    {
        damageNumbers.StartCoroutine(damageNumbers.InitNumberParticles(dmg, crit, pos, missed));
    }

    // Stun
    public void SpawnStun(Unit unit)
    {
        stunnedParticles.SpawnStun(unit);
    }
    public void StopStun(Unit unit)
    {
        stunnedParticles.StopStun(unit);
    }

    // Burn
    public void SetUnitsBurnCount(Unit unit, int count)
    {
        burnParticles.SetUnitsBurnCount(unit, count);
    }
}
