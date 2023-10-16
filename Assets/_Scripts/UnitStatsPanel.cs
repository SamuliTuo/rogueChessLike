using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public enum UnitStatSliderTypes
{
    HP,
    DMG,
    MAGIC,
    ATTSPD,
    MOVESPD
}
public class UnitStatsPanel : MonoBehaviour
{
    [SerializeField] private Sprite emptySlotImg;
    [SerializeField] private float maxHp = 300;
    [SerializeField] private float maxDamage = 50;
    [SerializeField] private float maxMagic = 50;
    [SerializeField] private float maxAttackSpeed = 100;
    [SerializeField] private float maxMoveSpeed = 100;

    private UnitData unit;
    // 0. hp
    // 1. dmg
    // 2. magic
    // 3. attspd
    // 4. movespd
    private List<Image> sliders = new List<Image>();
    private List<Image> spellSlots = new List<Image>();
    //[SerializeField] private List<<> maximumSpeeds = new List<float>();



    private void Start()
    {
        GetSliders();
        GetSpellSlots();
    }

    public void OpenUnitStatsPanel(UnitData unit)
    {
        this.unit = unit;
        GetSliders();
        GetSpellSlots();

        SetSlider(UnitStatSliderTypes.HP, unit.maxHp);
        SetSlider(UnitStatSliderTypes.DMG, unit.damage);
        SetSlider(UnitStatSliderTypes.MAGIC, unit.magic);
        SetSlider(UnitStatSliderTypes.ATTSPD, unit.attackSpeed);
        SetSlider(UnitStatSliderTypes.MOVESPD, unit.moveSpeed);
        SetSpellslots();
    }

    public void SetSlider(UnitStatSliderTypes slider, float value)
    {
        switch (slider)
        {
            case UnitStatSliderTypes.HP:
                SetSlider((int)slider, value, maxHp); break;
            case UnitStatSliderTypes.DMG:
                SetSlider((int)slider, value, maxDamage); break;
            case UnitStatSliderTypes.MAGIC:
                SetSlider((int)slider, value, maxMagic); break;
            case UnitStatSliderTypes.ATTSPD:
                SetSlider((int)slider, value, maxAttackSpeed); break;
            case UnitStatSliderTypes.MOVESPD:
                SetSlider((int)slider, value, maxMoveSpeed); break;
        }
    }
    void SetSlider(int slider, float value, float maxValue)
    {
        float perc = value / maxValue;
        sliders[slider].fillAmount = perc;
    }

    public void SetSpellslots()
    {
        var spells = unit.LearnedAbilities();
        for (int i = 0; i < spellSlots.Count; i++)
        {
            if (i < unit.LearnedAbilities().Count)
            {
                spellSlots[i].sprite = GameManager.Instance.AbilityLibrary.GetImg(spells[i]);
            }
            else
            {
                spellSlots[i].sprite = emptySlotImg;
            }
        }
    }
    void GetSliders()
    {
        if (sliders.Count == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                sliders.Add(transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>());
            }
        }
    }
    void GetSpellSlots()
    {
        if (spellSlots.Count == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                spellSlots.Add(transform.GetChild(1).GetChild(i).GetComponent<Image>());
            }
        }
    }
}
