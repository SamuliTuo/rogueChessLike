using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public bool dying;
    public float hpBarOffset = 1f;
    public int bloodMoneyAmount = 0;

    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float dyingTimeBeforeSinking = 1.5f;
    [SerializeField] private float sinkingSpeed = 1f;
    public float GetMaxHp() { return maxHp; }
    public void SetMaxHp(float value) { maxHp = value; }
    private float hpBarBiggenTime = 1;
    private float hpBarMaxBiggenPerc = 1.6f;

    [HideInInspector] public float hp;
    [HideInInspector] public float shield;
    [HideInInspector] public float armor;
    [HideInInspector] public float magicRes;

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
            hpScript.Init(this.transform, hpBarOffset, unit.team, hp);
            hpBarOriginalScale = hpBar.transform.localScale;
        }
    }

    public bool RemoveHPAndCheckIfUnitDied(float damage, bool dieFast = false, float critChance = 0, float critDamage = 1, float missChance = 0, bool isMagic = false)
    {
        if (DebugTools.Instance?.godMode == true && unit.team == 0)
        {
            return false;
        }
        
        // hit or miss?
        bool hit = Random.Range(0.000f, 0.999f) < 1 - missChance;
        if (!hit)
        {
            if (!dieFast) GameManager.Instance.ParticleSpawner.InitDamageNumbers(0, false, hpBar.GetComponent<HpBarInstance>().GetBarHpPos(), false); //transform.position + Vector3.up * hpBarOffset, true);// (ParticleType.ATTACK_MISS, transform.position, Camera.main.transform.forward);
        }

        // crits?
        bool isCrit = Random.Range(0.000f, 0.999f) < critChance;
        if (isCrit)
        {
            damage *= critDamage;
        }
        
        // Modify damage with armor / m.res value:
        float damageTaken;
        if (isMagic)
        {
            damageTaken = damage - (damage * (magicRes / (100 + magicRes)));
        }
        else
        {
            damageTaken = damage - (damage * (armor / (100 + armor)));
        }
        
        // Damage numbers:
        if (!dieFast)
        {
            GameManager.Instance.ParticleSpawner.InitDamageNumbers(damageTaken, isCrit, hpBar.GetComponent<HpBarInstance>().GetBarHpPos(), false); // transform.position + Vector3.up * hpBarOffset, false) ;
        }

        // Has an active shield:
        if (damageTaken > 0 && shield > 0)
        {
            shield -= damageTaken;
            if (shield >= 0)
            {
                return false;
            }
            else
            {
                damageTaken = Mathf.Abs(shield);
                shield = 0;
            }
        }

        // Do the honors:
        hp -= damageTaken;
        if (hp >= maxHp)
        {
            hp = maxHp;
        }
        if (hpScript != null)
        {
            hpScript.SetBarValue(hp / maxHp);
        }

        // Are you wanna die?
        if (hp <= 0 && dying == false)
        {
            dying = true;
            if (bloodMoneyAmount > 0)
            {
                GameManager.Instance.PlayerParty.AddMoney(bloodMoneyAmount);
            }
            GameManager.Instance.UnitHasDied(unit);
            StartCoroutine(Die(dieFast));
            return true;
        }
        else
        {
            //StopCoroutine("DamageFlash");
            //StartCoroutine("DamageFlash");
            if (timer > 0.6f)
            {
                //StopCoroutine("DamageFlash");
                StartCoroutine("BiggenHPBar", Mathf.Min(Mathf.Abs(damageTaken / maxHp), 1));
            }
            return false;
        }
    }

    public float lootSpawnWhenDyingTimePerc = 0.2f;
    public IEnumerator Die(bool dieFast = false)
    {
        GetComponent<UnitStatusModifiersHandler>()?.StopAllStatusEffects();
        var moneySpawnPos = transform.position;

        if (dieFast)
        {
            print("dying fast");
            Destroy(this.gameObject);
            StopAllCoroutines();
        }

        if (hpScript != null)
        {
            hpScript.Deactivate();
            hpScript = null;
        }

        // Dying animation
        anim?.Play("die", 0, 0);
        float t = 0;
        var moneySpawned = false;
        while (t < dyingTimeBeforeSinking)
        {
            if (!moneySpawned && t / dyingTimeBeforeSinking > lootSpawnWhenDyingTimePerc * dyingTimeBeforeSinking)
            {
                moneySpawned = true;
                GameManager.Instance.LootSpawner.SpawnMoney(Random.Range(1, 11), moneySpawnPos);
            }
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
        hpBarOriginalScale = Vector3.one;
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

            hpScript?.ScaleHpBar(Vector3.Lerp(hpBarOriginalScale + hpBarOriginalScale * (damagePerc * hpBarMaxBiggenPerc), hpBarOriginalScale, perc));
            yield return null;
        }
        hpScript?.ScaleHpBar(hpBarOriginalScale);
        //hpBar.transform.localScale = hpBarOriginalScale;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (hpScript != null)
        {
            hpScript.Deactivate();
            hpScript = null;
        }
    }

    public void RefreshSkillCooldownUISlot(int slot, float perc)
    {
        hpScript?.skillSymbols.UpdateAbilityCooldown(perc, slot);
    }

    public IEnumerator SetSkillSymbol(UnitAbility ability, int slot)
    {
        float t = 0;
        while (hpScript == null && t < 5)
        {
            t += Time.deltaTime;
            yield return null;
        }
        hpScript?.skillSymbols.SetSkillSlot(ability, slot);
    }
}
