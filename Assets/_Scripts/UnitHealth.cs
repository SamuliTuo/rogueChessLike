using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public bool dying;

    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float hpBarOffset = 1f;
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float dyingTimeBeforeSinking = 1.5f;
    [SerializeField] private float sinkingSpeed = 1f;
    public float GetMaxHp() { return maxHp; }
    public void SetMaxHp(float value) { maxHp = value; }
    private float hpBarBiggenTime = 0.6f;
    private float hpBarMaxBiggenPerc = 2.3f;    

    [HideInInspector] public float hp;
    [HideInInspector] public float shield;

    //private Material mat;
    private Color originalColor;
    private GameObject hpBar = null;
    private HpBarInstance hpScript;
    private Animator anim;
    
    private Unit unit;
    private Vector3 hpBarOriginalScale;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        dying = false;
        hp = maxHp;

        if (hpScript == null)
        {
            hpBar = GameManager.Instance.HPBars.SpawnBar();
            hpScript = hpBar.GetComponent<HpBarInstance>();
            hpScript.Init(this.transform, hpBarOffset, unit.team);
            hpBarOriginalScale = hpBar.transform.localScale;
        }
    }

    public void RemoveHP(float damage)
    {
        GameManager.Instance.ParticleSpawner.InitDamageNumbers(damage, transform.position + Vector3.up * hpBarOffset);

        // If taking damage and we have an active shield:
        if (damage > 0 && shield > 0)
        {
            shield -= damage;
            if (shield >= 0)
            {
                return;
            }
            else
            {
                damage = Mathf.Abs(shield);
                shield = 0;
            }
        }

        hp -= damage;
        if (hp >= maxHp)
            hp = maxHp;

        if (hpScript != null)
            hpScript.SetBarValue(hp / maxHp);
        
        if (hp <= 0 && dying == false)
        {
            dying = true;
            GameManager.Instance.UnitHasDied(unit);
            StartCoroutine(Die());
        }
        else
        {
            //StopCoroutine("DamageFlash");
            //StartCoroutine("DamageFlash");
            if (timer > 0.6f)
            {
                //StopCoroutine("DamageFlash");
                StartCoroutine("BiggenHPBar", Mathf.Min(Mathf.Abs(damage / maxHp), 1));
            }
        }
    }

    public IEnumerator Die()
    {
        GetComponent<UnitStatusModifiersHandler>()?.StopAllCoroutines();
        if (hpScript != null)
        {
            hpScript.Deactivate();
            hpScript = null;
        }

        // Dying animation
        anim?.Play("die", 0, 0);
        float t = 0;
        while (t < dyingTimeBeforeSinking)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // Sink the body
        t = 0;
        Vector3 starPos = transform.position;
        Vector3 endPos = transform.position + Vector3.down * 4;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(starPos, endPos, t);
            t += Time.deltaTime * sinkingSpeed;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    public float GetHealthPercentage()
    {
        return hp / maxHp;
    }

    public void AddMaxHealth(float add)
    {
        var perc = GetHealthPercentage();
        maxHp += add;
        hp = maxHp * perc;
    }

    public IEnumerator AddShield(float amount, float duration)
    {
        shield += amount;
        yield return new WaitForSeconds(duration);
        shield -= amount;
        if (shield < 0)
        {
            shield = 0;
        }
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
                perc = n1 * perc * perc;

            else if (perc < 2 / d1)
                perc = n1 * (perc -= 1.5f / d1) * perc + 0.75f;

            else if (perc < 2.5 / d1)
                perc = n1 * (perc -= 2.25f / d1) * perc + 0.9375f;

            else
                perc = n1 * (perc -= 2.625f / d1) * perc + 0.984375f;

            //perc = 1 - (1 - perc) * (1 - perc);
            timer += Time.deltaTime;

            hpBar.transform.localScale = Vector3.Lerp(hpBarOriginalScale + hpBarOriginalScale * (damagePerc * hpBarMaxBiggenPerc), hpBarOriginalScale, perc);
            yield return null;
        }
        hpBar.transform.localScale = hpBarOriginalScale;
    }

    private void OnDisable()
    {
        if (hpScript != null)
        {
            hpScript.Deactivate();
            hpScript = null;
        }
    }
}
