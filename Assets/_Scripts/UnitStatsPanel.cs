using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;


public enum UnitStatSliderTypes
{
    HP, ARMOR, MRES, DMG, MAGIC, ATTSPD, MOVESPD
}
public class UnitStatsPanel : MonoBehaviour
{
    [SerializeField] private Sprite emptySlotImg;
    
    private UnitData unit;
    private Image[] sliders;
    private TextMeshProUGUI[] sliderTexts;
    private List<Image> spellSlots = new List<Image>();
    private Image unitImage;

    private TextMeshProUGUI className;
    private TextMeshProUGUI[] classStatGainTexts;



    private void Start()
    {
        if (transform.childCount > 2)
        {
            unitImage = transform.GetChild(3).GetComponent<Image>();
        }
        GetSliders();
        GetSpellSlots();
        GetClassObjects();
    }

    public void OpenUnitStatsPanel(UnitData unit, bool setSlidersAutomatically = true)
    {
        this.unit = unit;
        Start();

        if (setSlidersAutomatically)
        {
            SetSlider(UnitStatSliderTypes.HP, unit.maxHp);
            SetSlider(UnitStatSliderTypes.ARMOR, unit.armor);
            SetSlider(UnitStatSliderTypes.MRES, unit.magicRes);
            SetSlider(UnitStatSliderTypes.DMG, unit.damage);
            SetSlider(UnitStatSliderTypes.MAGIC, unit.magic);
            SetSlider(UnitStatSliderTypes.ATTSPD, unit.attackSpeed);
            SetSlider(UnitStatSliderTypes.MOVESPD, unit.moveSpeed);
        }
        
        SetSpellslots();
        SetImage();
        SetClass();
    }

    public void SetSlider(UnitStatSliderTypes slider, float value, string text = "")
    {
        string sliderValueText = text == "" ? value.ToString() : text;
        switch (slider)
        {
            case UnitStatSliderTypes.HP: SetSlider((int)slider, value, GameManager.Instance.maxHpStat, sliderValueText); break;
            case UnitStatSliderTypes.ARMOR: SetSlider((int)slider, value, GameManager.Instance.maxArmorStat, sliderValueText); break;
            case UnitStatSliderTypes.MRES: SetSlider((int)slider, value, GameManager.Instance.maxMresStat, sliderValueText); break;
            case UnitStatSliderTypes.DMG: SetSlider((int)slider, value, GameManager.Instance.maxDamageStat, sliderValueText); break;
            case UnitStatSliderTypes.MAGIC: SetSlider((int)slider, value, GameManager.Instance.maxMagicStat, sliderValueText); break;
            case UnitStatSliderTypes.ATTSPD: SetSlider((int)slider, value, GameManager.Instance.maxAttackSpeedStat, sliderValueText); break;
            case UnitStatSliderTypes.MOVESPD: SetSlider((int)slider, value, GameManager.Instance.maxMoveSpeedStat, sliderValueText); break;
        }
    }

    void SetSlider(int slider, float value, float maxValue, string text)
    {
        float perc = value / maxValue;
        sliders[slider].fillAmount = perc;
        sliderTexts[slider].text = text;
    }

    void SetImage()
    {
        if (unitImage != null)
        {
            unitImage.sprite = GameManager.Instance.UnitLibrary.GetUnit(unit).image;
        }
    }

    public void SetSpellslots()
    {
        var spells = unit.LearnedAbilities();
        for (int i = 0; i < spellSlots.Count; i++)
        {
            if (i < unit.LearnedAbilities().Count)
            {
                spellSlots[i].sprite = GameManager.Instance.UnitLibrary.GetSpellSymbol(spells[i]);
            }
            else
            {
                spellSlots[i].sprite = emptySlotImg;
            }
        }
    }

    void GetSliders()
    {
        Transform slidersTransform = transform.GetChild(0);
        if (sliders == null)
        {
            sliders = new Image[]
            {
                slidersTransform.GetChild(1).GetChild(0).GetComponent<Image>(),
                slidersTransform.GetChild(2).GetChild(0).GetComponent<Image>(),
                slidersTransform.GetChild(3).GetChild(0).GetComponent<Image>(),
                slidersTransform.GetChild(4).GetChild(0).GetComponent<Image>(),
                slidersTransform.GetChild(5).GetChild(0).GetComponent<Image>(),
                slidersTransform.GetChild(6).GetChild(0).GetComponent<Image>(),
                slidersTransform.GetChild(7).GetChild(0).GetComponent<Image>(),
            };
        }
        if (sliderTexts == null)
        {
            sliderTexts = new TextMeshProUGUI[]
            {
                slidersTransform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>(),
                slidersTransform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>(),
                slidersTransform.GetChild(3).GetChild(2).GetComponent<TextMeshProUGUI>(),
                slidersTransform.GetChild(4).GetChild(2).GetComponent<TextMeshProUGUI>(),
                slidersTransform.GetChild(5).GetChild(2).GetComponent<TextMeshProUGUI>(),
                slidersTransform.GetChild(6).GetChild(2).GetComponent<TextMeshProUGUI>(),
                slidersTransform.GetChild(7).GetChild(2).GetComponent<TextMeshProUGUI>()
            };
        }
    }

    void GetSpellSlots()
    {
        if (transform.childCount <= 2)
        {
            return;
        }
        if (spellSlots.Count == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                spellSlots.Add(transform.GetChild(2).GetChild(0).GetChild(i).GetComponent<Image>());
            }
        }
    }

    void GetClassObjects()
    {
        className = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        classStatGainTexts = new TextMeshProUGUI[]
        {
            className.transform.GetChild(0).GetComponent<TextMeshProUGUI>(), //hp
            className.transform.GetChild(1).GetComponent<TextMeshProUGUI>(), //armor
            className.transform.GetChild(2).GetComponent<TextMeshProUGUI>(), //mres
            className.transform.GetChild(3).GetComponent<TextMeshProUGUI>(), //dmg
            className.transform.GetChild(4).GetComponent<TextMeshProUGUI>(), //magic
            className.transform.GetChild(5).GetComponent<TextMeshProUGUI>(), //AS
            className.transform.GetChild(6).GetComponent<TextMeshProUGUI>()  //MS
        };
    }
    

    public void SetClass()
    {
        className.text = unit.unitClass.name;
        classStatGainTexts[0].text = unit.unitClass.hp.ToString();
        classStatGainTexts[1].text = unit.unitClass.armor.ToString();
        classStatGainTexts[2].text = unit.unitClass.mgArmor.ToString();
        classStatGainTexts[3].text = unit.unitClass.dmg.ToString();
        classStatGainTexts[4].text = unit.unitClass.mgDmg.ToString();
        classStatGainTexts[5].text = unit.unitClass.attSpd.ToString();
        classStatGainTexts[6].text = unit.unitClass.moveSpd.ToString();
    }
}
