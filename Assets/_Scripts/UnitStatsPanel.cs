using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public enum UnitStatSliderTypes
{
    HP, ARMOR, MRES, DMG, MAGIC, ATTSPD, MOVESPD
}
public class UnitStatsPanel : MonoBehaviour
{
    [SerializeField] private Sprite emptySlotImg;
    [SerializeField] private GameObject exitButton;
    
    private UnitData unit;
    private Image[] sliders;
    private TextMeshProUGUI[] sliderTexts;
    private List<Image> spellSlots = new List<Image>();
    private Item[] itemSlots = new Item[] { null, null, null };
    private List<Image> itemSlots_images = new List<Image>();
    private Image unitImage;

    private TextMeshProUGUI className;
    private TextMeshProUGUI[] classStatGainTexts;
    private Item itemBeingGranted;
    private Shop shop;


    private void Start()
    {
        if (transform.childCount > 2)
        {
            unitImage = transform.GetChild(3).GetComponent<Image>();
        }
        GetSliders();
        GetSpellSlots();
        GetItemImageSlots();
        GetClassObjects();
    }

    public void OpenUnitStatsPanel(UnitData unit, bool setSlidersAutomatically = true, Item itemBeingGranted = null, Shop shop = null)
    {
        this.unit = unit;
        this.itemBeingGranted = itemBeingGranted;
        this.shop = shop;
        if (shop != null && exitButton != null)
        {
            exitButton.SetActive(true);
        }
        else if (exitButton != null)
        {
            exitButton.SetActive(false);
        }
        
        Start();
        SetItems();
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
        gameObject.SetActive(true);
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
    void GetItemImageSlots()
    {
        if (transform.childCount < 4)
        {
            return;
        }
        if (itemSlots_images.Count == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                itemSlots_images.Add(transform.GetChild(4).GetChild(i).GetChild(0).GetComponent<Image>());
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
        if (unit.unitClassSuffix != null)
        {
            SetClassPrefixAndSuffix();
        }
        else if (unit.unitClassPrefix != null)
        {
            SetClassAndPrefix();
        }
        else
        {
            className.text = unit.unitClass.name;
            classStatGainTexts[0].text = unit.unitClass.hp == 0 ? "" : unit.unitClass.hp.ToString();
            classStatGainTexts[1].text = unit.unitClass.armor == 0 ? "" : unit.unitClass.armor.ToString();
            classStatGainTexts[2].text = unit.unitClass.mgArmor == 0 ? "" : unit.unitClass.mgArmor.ToString();
            classStatGainTexts[3].text = unit.unitClass.dmg == 0 ? "" : unit.unitClass.dmg.ToString();
            classStatGainTexts[4].text = unit.unitClass.mgDmg == 0 ? "" : unit.unitClass.mgDmg.ToString();
            classStatGainTexts[5].text = unit.unitClass.attSpd == 0 ? "" : unit.unitClass.attSpd.ToString();
            classStatGainTexts[6].text = unit.unitClass.moveSpd == 0 ? "" : unit.unitClass.moveSpd.ToString();
        }
    }

    void SetClassPrefixAndSuffix()
    {
        className.text = unit.unitClassPrefix.name + " " + unit.unitClass.name + " " + unit.unitClassSuffix.name;
        classStatGainTexts[0].text = (unit.unitClassPrefix.hp + unit.unitClass.hp + unit.unitClassSuffix.hp) == 0 ? "" : (unit.unitClassPrefix.hp + unit.unitClass.hp + unit.unitClassSuffix.hp).ToString();
        classStatGainTexts[1].text = (unit.unitClassPrefix.armor + unit.unitClass.armor + unit.unitClassSuffix.moveSpd) == 0 ? "" : (unit.unitClassPrefix.armor + unit.unitClass.armor + unit.unitClassSuffix.moveSpd).ToString();
        classStatGainTexts[2].text = (unit.unitClassPrefix.mgArmor + unit.unitClass.mgArmor + unit.unitClassSuffix.moveSpd) == 0 ? "" : (unit.unitClassPrefix.mgArmor + unit.unitClass.mgArmor + unit.unitClassSuffix.moveSpd).ToString();
        classStatGainTexts[3].text = (unit.unitClassPrefix.dmg + unit.unitClass.dmg + unit.unitClassSuffix.moveSpd) == 0 ? "" : (unit.unitClassPrefix.dmg + unit.unitClass.dmg + unit.unitClassSuffix.moveSpd).ToString();
        classStatGainTexts[4].text = (unit.unitClassPrefix.mgDmg + unit.unitClass.mgDmg + unit.unitClassSuffix.moveSpd) == 0 ? "" : (unit.unitClassPrefix.mgDmg + unit.unitClass.mgDmg + unit.unitClassSuffix.moveSpd).ToString();
        classStatGainTexts[5].text = (unit.unitClassPrefix.attSpd + unit.unitClass.attSpd + unit.unitClassSuffix.attSpd) == 0 ? "" : (unit.unitClassPrefix.attSpd + unit.unitClass.attSpd + unit.unitClassSuffix.attSpd).ToString();
        classStatGainTexts[6].text = (unit.unitClassPrefix.moveSpd + unit.unitClass.moveSpd + unit.unitClassSuffix.moveSpd) == 0 ? "" : (unit.unitClassPrefix.moveSpd + unit.unitClass.moveSpd + unit.unitClassSuffix.moveSpd).ToString();
    }

    void SetClassAndPrefix()
    {
        className.text = unit.unitClassPrefix.name + " " + unit.unitClass.name;
        classStatGainTexts[0].text = (unit.unitClassPrefix.hp + unit.unitClass.hp) == 0 ? "" : (unit.unitClassPrefix.hp + unit.unitClass.hp).ToString();
        classStatGainTexts[1].text = (unit.unitClassPrefix.armor + unit.unitClass.armor) == 0 ? "" : (unit.unitClassPrefix.armor + unit.unitClass.armor).ToString();
        classStatGainTexts[2].text = (unit.unitClassPrefix.mgArmor + unit.unitClass.mgArmor) == 0 ? "" : (unit.unitClassPrefix.mgArmor + unit.unitClass.mgArmor).ToString();
        classStatGainTexts[3].text = (unit.unitClassPrefix.dmg + unit.unitClass.dmg) == 0 ? "" : (unit.unitClassPrefix.dmg + unit.unitClass.dmg).ToString();
        classStatGainTexts[4].text = (unit.unitClassPrefix.mgDmg + unit.unitClass.mgDmg) == 0 ? "" : (unit.unitClassPrefix.mgDmg + unit.unitClass.mgDmg).ToString();
        classStatGainTexts[5].text = (unit.unitClassPrefix.attSpd + unit.unitClass.attSpd) == 0 ? "" : (unit.unitClassPrefix.attSpd + unit.unitClass.attSpd).ToString();
        classStatGainTexts[6].text = (unit.unitClassPrefix.moveSpd + unit.unitClass.moveSpd) == 0 ? "" : (unit.unitClassPrefix.moveSpd + unit.unitClass.moveSpd).ToString();
    }

    public void AddClassPrefix()
    {
        unit.unitClassPrefix = GameManager.Instance.ClassLibrary.GetRandomPrefix();
        SetClassAndPrefix();
    }
    public void AddClassSuffix()
    {
        unit.unitClassSuffix = GameManager.Instance.ClassLibrary.GetRandomSuffix();
        SetClassPrefixAndSuffix();
    }

    private void SetItems()
    {
        if (itemSlots_images.Count < 3)
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (unit.itemSlots_itemID[i] != -1)
            {
                itemSlots[i] = GameManager.Instance.ItemLibrary.GetItemWithID(unit.itemSlots_itemID[i]);
                itemSlots_images[i].sprite = itemSlots[i].icon;
            }
            else
            {
                itemSlots_images[i].sprite = emptySlotImg;
            }
        }
    }

    public void ItemSlotWasClicked(int slot)
    {
        // Equipping a new item
        if (itemBeingGranted != null)
        {
            if (itemSlots[slot] == null)
            {
                // Set item
                itemSlots[slot] = itemBeingGranted;
                itemSlots_images[slot].sprite = itemBeingGranted.icon;
                unit.itemSlots_itemID[slot] = itemBeingGranted.itemID;

                // Grant the stats
                print("unithp before: "+unit.maxHp);
                unit.maxHp += itemBeingGranted.itemStats.hp;
                print("unithp after: " + unit.maxHp);
                unit.damage += itemBeingGranted.itemStats.damage;
                unit.magic += itemBeingGranted.itemStats.magicDamage;
                unit.critChance += itemBeingGranted.itemStats.critChance;
                unit.critDamage += itemBeingGranted.itemStats.critDamage;
                unit.missChance += itemBeingGranted.itemStats.missChance;
                unit.moveSpeed += itemBeingGranted.itemStats.moveSpeed;
                unit.attackSpeed += itemBeingGranted.itemStats.attackSpeed;
                unit.armor += itemBeingGranted.itemStats.armor;
                unit.magicRes += itemBeingGranted.itemStats.magicRes;

                // Grant the unique effects (like flying movement)
                print("itemeistä puuttuu unique effects");

                if (shop != null)
                {
                    shop.BuyItem();
                }
                CloseUnitStatsPanel();
            }
        }
    }

    public void CloseUnitStatsPanel()
    {
        gameObject.SetActive(false);
    }
}