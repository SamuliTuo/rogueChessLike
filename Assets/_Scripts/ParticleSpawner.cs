using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum ParticleType
{
    NONE,
    BASIC_CAST_PUFF,
    CLERIC_BOOMBA,
    RANGER_STABB,
}

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] ParticleSystem basicCastPuff;
    [SerializeField] ParticleSystem clericBomba;
    [SerializeField] ParticleSystem rangerStabb;

    public void SpawnParticles(ParticleType type, Vector3 pos)
    {
        switch (type)
        {
            case ParticleType.BASIC_CAST_PUFF:
                PlayParticle(basicCastPuff, pos);
                break;
            case ParticleType.CLERIC_BOOMBA:
                PlayParticle(clericBomba, pos);
                break;
            case ParticleType.RANGER_STABB:
                PlayParticle(rangerStabb, pos);
                break;
            default:
                break;
        }
    }

    void PlayParticle(ParticleSystem s, Vector3 pos)
    {
        s.transform.position = pos;
        s.Play();
        var source = s.GetComponent<AudioSource>();
        source.pitch = Random.Range(0.80f, 1.20f);
        source.Play();
    }
}
