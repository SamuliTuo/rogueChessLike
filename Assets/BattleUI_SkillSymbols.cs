using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI_SkillSymbols : MonoBehaviour
{ 
    public List<Image> skillSymbols = new List<Image>();
    public List<Image> backgrounds = new List<Image>();
    public List<Image> borders = new List<Image>();

    [SerializeField] private float hiddenAlpha = 1f;


    private void Start()
    {
        // This was a test to assign to Alpha channel. It works.

        //foreach (var item in skillSymbols)
        //{
        //    var tempColor = item.color;
        //    tempColor.a = hiddenAlpha;
        //    item.color = tempColor;
        //}
        //foreach (var item in backgrounds)
        //{
        //    var tempColor = item.color;
        //    tempColor.a = hiddenAlpha;
        //    item.color = tempColor;
        //}
    }

    public void InitSkillSymbols(List<UnitAbility> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            print("täällä koitetaan laittaa skilli symbolit ui elementteihin");
            //skillSymbols[i].sprite = skills[i].;
        }
    }
    
    public void UpdateAbilityCooldown(float perc, int slot)
    {
        if (slot < 0 && slot >= skillSymbols.Count)
            return;

        skillSymbols[slot].fillAmount = perc;
        backgrounds[slot].fillAmount = perc;
        borders[slot].fillAmount = perc;
    }

    public void SetSkillSlot(UnitAbility abi, int slot)
    {
        if (abi == null)
        {
            skillSymbols[slot].gameObject.SetActive(false);
            backgrounds[slot].gameObject.SetActive(false);
            borders[slot].gameObject.SetActive(false);
        }
        skillSymbols[slot].sprite = GameManager.Instance.UnitLibrary.GetSpellSymbol(abi);
    }
}
