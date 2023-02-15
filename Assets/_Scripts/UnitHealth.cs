using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float hpBarOffset = 1f;
    [SerializeField] private float maxHp = 100;

    [HideInInspector] public float hp;

    private Material mat;
    private Color originalColor;
    private GameObject hpBar = null;
    private HpBarInstance hpScript;
    private bool dying;

    private void Awake()
    {
        dying = false;
        hp = maxHp;
        mat = GetComponentInChildren<MeshRenderer>().materials[0];
        originalColor = mat.color;

        if (hpBar == null)
        {
            hpBar = GameManager.Instance.HPBars.SpawnBar();
            hpScript = hpBar.GetComponent<HpBarInstance>();
            hpScript.Init(this.transform, hpBarOffset);
        }
    }

    public void GetDamaged(float damage)
    {
        hp -= damage;
        if (hp >= maxHp)
            hp = maxHp;

        hpScript.SetBarValue(hp / maxHp);
        if (hp <= 0 && dying == false)
        {
            dying = true;
            hpScript.Deactivate();
            Destroy(this.gameObject);
        }
        else
        {
            StopCoroutine("DamageFlash");
            StartCoroutine("DamageFlash");
        }
    }

    IEnumerator DamageFlash()
    {
        mat.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        mat.color = originalColor;
    }
}
