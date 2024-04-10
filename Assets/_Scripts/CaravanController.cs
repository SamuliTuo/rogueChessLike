using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class CaravanController : MonoBehaviour
{
    [SerializeField] GameObject caravanPanel;

    private List<Tuple<UnitData, UnitInLibrary>> caravanSlots = new List<Tuple<UnitData, UnitInLibrary>>();
    private Image[] caravanSlotUnitImages;
    private List<Image[]> caravanSlotSpellImages;
    private UnitStatsPanel[] caravanSlotUnitStats;
    private GameObject[] rerollButtons;
    private bool canReroll;
    

    private void Start()
    {
        GetReferences();
    }

    public Sprite emptyImage;

    public void OpenCaravan()
    {
        caravanSlots.Clear();
        SetupUnitChoices();
        caravanPanel.SetActive(true);

        print("check if this is first caravan and only then allow reroll (or if player has been given ability to reroll units later in game)");
        canReroll = true;
        ToggleRerollButtons(true);
    }



    public void FullReroll(int slot)
    {
        if (!canReroll)
            return;

        canReroll = false;
        ToggleRerollButtons(false);

        caravanSlots[slot] = GetARandomCaravanChoice(slot);
        caravanSlotUnitImages[slot].sprite = caravanSlots[slot].Item2.image;
        caravanSlotSpellImages[slot][0].sprite = caravanSlots[slot].Item1.ability1 == null ? emptyImage : caravanSlots[slot].Item2.GetSpellImage(caravanSlots[slot].Item1.ability1);
        caravanSlotSpellImages[slot][1].sprite = caravanSlots[slot].Item1.ability2 == null ? emptyImage : caravanSlots[slot].Item2.GetSpellImage(caravanSlots[slot].Item1.ability2);
        caravanSlotSpellImages[slot][2].sprite = caravanSlots[slot].Item1.ability3 == null ? emptyImage : caravanSlots[slot].Item2.GetSpellImage(caravanSlots[slot].Item1.ability3);
    }
    public void RerollStats(int slot)
    {
        if (!canReroll)
            return;

        canReroll = false;
        ToggleRerollButtons(false);

        SetStats(caravanSlots[slot].Item1, caravanSlots[slot].Item2, slot);
    }
    public void RerollClass(int slot)
    {
        if (!canReroll)
            return;

        canReroll = false;
        ToggleRerollButtons(false);

        caravanSlots[slot].Item1.unitClass = GameManager.Instance.ClassLibrary.GetRandomClass(caravanSlots[slot].Item1.unitClass);
        caravanSlotUnitStats[slot].SetClass();
    }


    public void ChooseUnit(int chosenSlot)
    {
        GameManager.Instance.PlayerParty.AddUnit(caravanSlots[chosenSlot].Item1, caravanSlots[chosenSlot].Item2);
        CloseCaravan();
    }
    void CloseCaravan()
    {
        caravanPanel.SetActive(false);
        GameManager.Instance.MapController.SetCanMove(true);
    }

    void SetupUnitChoices()
    {
        caravanSlots.Clear();
        for (int i = 0; i < 3; i++)
        {
            caravanSlots.Add(GetARandomCaravanChoice(i));
        }
    }

    int randomFactorHP, randomFactorArmor, randomFactorMres, randomFactorDmg, randomFactorMagic, randomFactorAS, randomFactorMS;
    Tuple<UnitData, UnitInLibrary> GetARandomCaravanChoice(int index)
    {
        UnitInLibrary randomUnit;
        List<UnitInLibrary> units = new List<UnitInLibrary>();
        foreach (var unit in GameManager.Instance.UnitLibrary.playerUnits)
        {
            if ((DebugTools.Instance.Penguin && unit.nameInList == "Penguin")
                || (DebugTools.Instance.BearCub && unit.nameInList == "Bearbub")
                || (DebugTools.Instance.Squirrel && unit.nameInList == "Squirrel")
                || (DebugTools.Instance.BlackLion && unit.nameInList == "Black Lion"))
            {
                units.Add(unit);
            }
        }
        randomUnit = units[UnityEngine.Random.Range(0, units.Count)];

        //GameManager.Instance.UnitSavePaths.unitsDatas[Random.Range(0, GameManager.Instance.UnitSavePaths.unitsDatas.Count)];
        //var randomUnit = GameManager.Instance.UnitSavePaths.unitsDatas[Random.Range(0, GameManager.Instance.UnitSavePaths.unitsDatas.Count)];
        Vector2Int spawnPos = GameManager.Instance.PlayerParty.GetFirstFreePartyPos();
        UnitData data = new UnitData(randomUnit.prefab.GetComponent<Unit>(), randomUnit.nameInList, spawnPos.x, spawnPos.y, 1);

        // Clone and set the attacks for the unit:
        List<Unit_NormalAttack> clonedAttacks = new List<Unit_NormalAttack>();
        for (int i = 0; i < randomUnit.attacks.Count; i++)
        {
            string attackName = randomUnit.attacks[i].name;
            var clone = Instantiate(randomUnit.attacks[i].attack);
            clone.name = attackName;
            clonedAttacks.Add(clone);
        }
        foreach (var attack in clonedAttacks)
        {
            data.attacks.Add(attack);
        }
        data.randomizeAttacks = randomUnit.randomizeAttackingOrder;
        
        // Clone and set the abilities for the unit:
        var unitAbilityManager = randomUnit.prefab.GetComponent<UnitAbilityManager>();

        // Get a random "signature spell"
        int rand = UnityEngine.Random.Range(0, 3);
        string spellName = unitAbilityManager.possibleAbilities[rand].name;
        UnitAbility c = Instantiate(unitAbilityManager.possibleAbilities[rand]);
        c.name = spellName;
        data.ability1 = c;

        // Set class
        data.unitClass = GameManager.Instance.ClassLibrary.GetRandomClass();

        caravanSlotUnitImages[index].sprite = randomUnit.image;
        caravanSlotSpellImages[index][0].sprite = data.ability1 == null ? emptyImage : randomUnit.GetSpellImage(data.ability1);
        caravanSlotSpellImages[index][1].sprite = data.ability2 == null ? emptyImage : randomUnit.GetSpellImage(data.ability2);
        caravanSlotSpellImages[index][2].sprite = data.ability3 == null ? emptyImage : randomUnit.GetSpellImage(data.ability3);

        SetStats(data, randomUnit, index);

        return new Tuple<UnitData, UnitInLibrary>(data, randomUnit);
    }


    void SetStats(UnitData data, UnitInLibrary randomUnit, int index)
    {
        randomFactorHP = UnityEngine.Random.Range(-1, 2);
        randomFactorArmor = UnityEngine.Random.Range(-1, 2);
        randomFactorMres = UnityEngine.Random.Range(-1, 2);
        randomFactorDmg = UnityEngine.Random.Range(-1, 2);
        randomFactorMagic = UnityEngine.Random.Range(-1, 2);
        randomFactorAS = UnityEngine.Random.Range(-1, 2);
        randomFactorMS = UnityEngine.Random.Range(-1, 2);

        data.maxHp = Mathf.Max(0, randomUnit.stats.hp + (randomFactorHP * GameManager.Instance.ClassLibrary.hpPerPoint));
        data.armor = Mathf.Max(0, randomUnit.stats.armor + (randomFactorArmor * GameManager.Instance.ClassLibrary.armorPerPoint));
        data.magicRes = Mathf.Max(0, randomUnit.stats.magicRes + (randomFactorMres * GameManager.Instance.ClassLibrary.mgArmorPerPoint));
        data.damage = Mathf.Max(0, randomUnit.stats.damage + (randomFactorDmg * GameManager.Instance.ClassLibrary.dmgPerPoint));
        data.magic = Mathf.Max(0, randomUnit.stats.magicDamage + (randomFactorMagic * GameManager.Instance.ClassLibrary.magicPerPoint));
        data.attackSpeed = Mathf.Max(0, randomUnit.stats.attackSpeed + (randomFactorAS * GameManager.Instance.ClassLibrary.attSpdPerPoint));
        data.moveSpeed = Mathf.Max(0, randomUnit.stats.moveSpeed + (randomFactorMS * GameManager.Instance.ClassLibrary.moveSpdPerPoint));
        data.moveInterval = Mathf.Max(0, randomUnit.stats.visibleMoveSpeed);

        caravanSlotUnitStats[index].OpenUnitStatsPanel(data, false);
        caravanSlotUnitStats[index].SetSlider(UnitStatSliderTypes.HP, data.maxHp, (randomFactorHP == 0 ? "" : (randomFactorHP == 1 ? "+" : "-") + "\n") + data.maxHp.ToString());
        caravanSlotUnitStats[index].SetSlider(UnitStatSliderTypes.ARMOR, data.armor, (randomFactorArmor == 0 ? "" : (randomFactorArmor == 1 ? "+" : "-") + "\n") + data.armor.ToString());
        caravanSlotUnitStats[index].SetSlider(UnitStatSliderTypes.MRES, data.magicRes, (randomFactorMres == 0 ? "" : (randomFactorMres == 1 ? "+" : "-") + "\n") + data.magicRes.ToString());
        caravanSlotUnitStats[index].SetSlider(UnitStatSliderTypes.DMG, data.damage, (randomFactorDmg == 0 ? "" : (randomFactorDmg == 1 ? "+" : "-") + "\n") + data.damage.ToString());
        caravanSlotUnitStats[index].SetSlider(UnitStatSliderTypes.MAGIC, data.magic, (randomFactorMagic == 0 ? "" : (randomFactorMagic == 1 ? "+" : "-") + "\n") + data.magic.ToString());
        caravanSlotUnitStats[index].SetSlider(UnitStatSliderTypes.ATTSPD, data.attackSpeed, (randomFactorAS == 0 ? "" : (randomFactorAS == 1 ? "+" : "-") + "\n") + data.attackSpeed.ToString());
        caravanSlotUnitStats[index].SetSlider(UnitStatSliderTypes.MOVESPD, data.moveSpeed, (randomFactorMS == 0 ? "" : (randomFactorMS == 1 ? "+" : "-") + "\n") + data.moveSpeed.ToString());
    }


    void GetReferences()
    {
        caravanSlotUnitImages = new Image[] {
            caravanPanel.transform.Find("SLOT1").GetComponent<Image>(),
            caravanPanel.transform.Find("SLOT2").GetComponent<Image>(),
            caravanPanel.transform.Find("SLOT3").GetComponent<Image>()
        };

        caravanSlotSpellImages = new List<Image[]>()
        {
            caravanSlotUnitImages[0].transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>(),
            caravanSlotUnitImages[1].transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>(),
            caravanSlotUnitImages[2].transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>()
        };

        caravanSlotUnitStats = new UnitStatsPanel[]
        {
            caravanSlotUnitImages[0].transform.GetChild(1).GetComponent<UnitStatsPanel>(),
            caravanSlotUnitImages[1].transform.GetChild(1).GetComponent<UnitStatsPanel>(),
            caravanSlotUnitImages[2].transform.GetChild(1).GetComponent<UnitStatsPanel>()
        };

        rerollButtons = new GameObject[]
        {
            caravanSlotUnitImages[0].transform.GetChild(2).gameObject,
            caravanSlotUnitImages[1].transform.GetChild(2).gameObject,
            caravanSlotUnitImages[2].transform.GetChild(2).gameObject
        };
        // Set active only if this is the 1st caravan:
        for (int i = 0; i < rerollButtons.Length; i++)
        {
            rerollButtons[i].SetActive(true);
        }
    }

    void ToggleRerollButtons(bool state)
    {
        for (int i = 0; i < rerollButtons.Length; i++)
        {
            rerollButtons[i].SetActive(state);
        }
    }
}