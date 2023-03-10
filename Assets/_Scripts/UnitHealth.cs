using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{

    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float hpBarOffset = 1f;
    [SerializeField] private float maxHp = 100;
    private float hpBarBiggenTime = 0.6f;
    private float hpBarMaxBiggenPerc = 2.3f;

    [HideInInspector] public float hp;

    //private Material mat;
    private Color originalColor;
    private GameObject hpBar = null;
    private HpBarInstance hpScript;
    private bool dying;
    private Unit unit;
    private Vector3 hpBarOriginalScale;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    private void Start()
    {
        dying = false;
        hp = maxHp;
        //mat = GetComponentInChildren<MeshRenderer>().materials[0];
        //originalColor = mat.color;

        if (hpBar == null)
        {
            hpBar = GameManager.Instance.HPBars.SpawnBar();
            hpScript = hpBar.GetComponent<HpBarInstance>();
            hpScript.Init(this.transform, hpBarOffset, unit.team);
            hpBarOriginalScale = hpBar.transform.localScale;
        }
    }

    public void GetDamaged(float damage)
    {
        hp -= damage;
        GameManager.Instance.ParticleSpawner.InitDamageNumbers(damage, transform.position + Vector3.up * hpBarOffset);

        if (hp >= maxHp)
            hp = maxHp;

        hpScript.SetBarValue(hp / maxHp);
        if (hp <= 0 && dying == false)
        {
            dying = true;
            Die();
        }
        else
        {
            StopCoroutine("DamageFlash");
            StartCoroutine("DamageFlash");
            if (timer > 0.6f)
            {
                StopCoroutine("DamageFlash");
                StartCoroutine("BiggenHPBar", Mathf.Min(Mathf.Abs(damage / maxHp), 1));
            }
            
        }
    }

    public void Die()
    {
        hpScript.Deactivate();
        Destroy(this.gameObject);
    }

    public float GetHealthPercentage()
    {
        return hp / maxHp;
    }

    IEnumerator DamageFlash()
    {
        //mat.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        //mat.color = originalColor;
    }

    float timer = 1;
    IEnumerator BiggenHPBar(float damagePerc)
    {
        timer = 0;
        while (timer <= hpBarBiggenTime)
        {
            float perc = timer / hpBarBiggenTime;

            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (perc < 1 / d1)
            {
                perc = n1 * perc * perc;
            }
            else if (perc < 2 / d1)
            {
                perc = n1 * (perc -= 1.5f / d1) * perc + 0.75f;
            }
            else if (perc < 2.5 / d1)
            {
                perc = n1 * (perc -= 2.25f / d1) * perc + 0.9375f;
            }
            else
            {
                perc = n1 * (perc -= 2.625f / d1) * perc + 0.984375f;
            }

            //perc = 1 - (1 - perc) * (1 - perc);
            timer += Time.deltaTime;

            hpBar.transform.localScale = Vector3.Lerp(hpBarOriginalScale + hpBarOriginalScale * (damagePerc * hpBarMaxBiggenPerc), hpBarOriginalScale, perc);
            yield return null;
        }
        hpBar.transform.localScale = hpBarOriginalScale;
    }
}
