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
    SPARKS_JUMPY
}

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] DamageNumbers damageNumbers;
    [SerializeField] StunnedParticles stunnedParticles;

    [SerializeField] ParticleSystem basicCastPuff;
    [SerializeField] ParticleSystem clericBomba;
    [SerializeField] ParticleSystem rangerStabb;
    [SerializeField] ParticleSystem warriorHit;
    [SerializeField] ParticleSystem zzzap;
    [SerializeField] ParticleSystem healParticles;
    [SerializeField] ParticleSystem fireExplosion;
    [SerializeField] ParticleSystem noteRing;
    [SerializeField] ParticleSystem sparks_jumpy;

    public void SpawnParticles(ParticleType type, Vector3 pos)
    {
        switch (type)
        {
            case ParticleType.BASIC_CAST_PUFF:
                PlayParticle(basicCastPuff, pos); break;

            case ParticleType.CLERIC_BOOMBA:
                PlayParticle(clericBomba, pos); break;

            case ParticleType.RANGER_STABB:
                PlayParticle(rangerStabb, pos); break;

            case ParticleType.WARRIOR_HIT:
                PlayParticle(warriorHit, pos); break;

            case ParticleType.ZZZAP:
                PlayParticle(zzzap, pos); break;

            case ParticleType.HEAL_PARTICLES:
                PlayParticle(healParticles, pos); break;

            case ParticleType.FIRE_EXPLOSION:
                PlayParticle(fireExplosion, pos); break;

            case ParticleType.NOTE_RING:
                PlayParticle(noteRing, pos); break;

            case ParticleType.SPARKS_JUMPY:
                PlayParticle(sparks_jumpy, pos); break;

            default:
                break;
        }
    }





    public void Reset()
    {
        damageNumbers.Reset();
    }

    void PlayParticle(ParticleSystem s, Vector3 pos)
    {
        s.transform.position = pos;
        s.Play();
        var source = s.GetComponent<AudioSource>();
        source.pitch = Random.Range(0.80f, 1.20f);
        source.Play();
    }

    // Damage numbers
    public void InitDamageNumbers(float dmg, Vector3 pos)
    {
        damageNumbers.StartCoroutine(damageNumbers.InitNumberParticles(dmg, pos));
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
}
