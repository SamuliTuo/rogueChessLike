using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CaravanController : MonoBehaviour
{
    [SerializeField] GameObject caravanPanel;

    private List<Tuple<UnitData, UnitInLibrary>> caravanSlots = new List<Tuple<UnitData, UnitInLibrary>>();
    private Image caravanSlot1Image;
    private Image caravanSlot2Image;
    private Image caravanSlot3Image;
    private Image[] slot1_spells;
    private Image[] slot2_spells;
    private Image[] slot3_spells;
    

    private void Start()
    {
        caravanSlot1Image = caravanPanel.transform.Find("Image1").GetComponent<Image>();
        caravanSlot2Image = caravanPanel.transform.Find("Image2").GetComponent<Image>();
        caravanSlot3Image = caravanPanel.transform.Find("Image3").GetComponent<Image>();
        
        slot1_spells = caravanSlot1Image.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>();
        slot2_spells = caravanSlot2Image.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>();
        slot3_spells = caravanSlot3Image.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>();
    }

    public Sprite emptyImage;

    public void OpenCaravan()
    {
        caravanSlots.Clear();
        List<Tuple<UnitData, UnitInLibrary>> choices = GetUnitChoices();
        foreach (var choice in choices)
        {
            caravanSlots.Add(choice);
        }
        caravanSlot1Image.sprite = caravanSlots[0].Item2.image;
        caravanSlot2Image.sprite = caravanSlots[1].Item2.image;
        caravanSlot3Image.sprite = caravanSlots[2].Item2.image;
        SetSpellImages(caravanSlots[0], slot1_spells);
        SetSpellImages(caravanSlots[1], slot2_spells);
        SetSpellImages(caravanSlots[2], slot3_spells);
        caravanPanel.SetActive(true);
    }

    void SetSpellImages(Tuple<UnitData, UnitInLibrary> slot, Image[] skillSlots)
    {
        skillSlots[0].sprite = slot.Item1.ability1 == null ? emptyImage : slot.Item2.GetSpellImage(slot.Item1.ability1);
        skillSlots[1].sprite = slot.Item1.ability2 == null ? emptyImage : slot.Item2.GetSpellImage(slot.Item1.ability2);
        skillSlots[2].sprite = slot.Item1.ability3 == null ? emptyImage : slot.Item2.GetSpellImage(slot.Item1.ability3);

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

    List<Tuple<UnitData, UnitInLibrary>> GetUnitChoices()
    {
        var r = new List<Tuple<UnitData, UnitInLibrary>>();
        for (int i = 0; i < 3; i++)
        {
            r.Add(GetARandomCaravanChoice());
        }
        return r;
    }

    Tuple<UnitData, UnitInLibrary> GetARandomCaravanChoice()
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
        Unit unitScript = randomUnit.prefab.GetComponent<Unit>();
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

        // Set the stats of the unit
        data.maxHp =        randomUnit.stats.hp != -1 ?                 randomUnit.stats.hp : GameManager.Instance.UnitLibrary.hp;
        data.damage =       randomUnit.stats.damage != -1 ?             randomUnit.stats.damage : GameManager.Instance.UnitLibrary.damage;
        data.magic =        randomUnit.stats.magicDamage != -1 ?        randomUnit.stats.magicDamage : GameManager.Instance.UnitLibrary.magicDamage;
        data.attackSpeed =  randomUnit.stats.attackSpeed != -1 ?        randomUnit.stats.attackSpeed : GameManager.Instance.UnitLibrary.attackSpeed;
        data.moveSpeed =    randomUnit.stats.moveSpeed != -1 ?          randomUnit.stats.moveSpeed : GameManager.Instance.UnitLibrary.moveSpeed;
        data.moveInterval = randomUnit.stats.visibleMoveSpeed != -1 ?   randomUnit.stats.visibleMoveSpeed : GameManager.Instance.UnitLibrary.visibleMoveSpeed;
        data.armor =        randomUnit.stats.armor != -1 ?              randomUnit.stats.armor : GameManager.Instance.UnitLibrary.armor;
        data.magicRes =     randomUnit.stats.magicRes != -1 ?           randomUnit.stats.magicRes : GameManager.Instance.UnitLibrary.magicRes;

        return new Tuple<UnitData, UnitInLibrary>(data, randomUnit);
        /*

        int[] abilChoices = GameManager.Instance.GenerateRandomUniqueIntegers(new Vector2Int(1, 2), new Vector2Int(0, unitAbilityManager.possibleAbilities.Count));
        if (abilChoices != null)
        {
            foreach (int randomInt in abilChoices)
            {
                string spellName = unitAbilityManager.possibleAbilities[randomInt].name;
                var clone = Instantiate(unitAbilityManager.possibleAbilities[randomInt]);
                clone.name = spellName;
                abilities.Add(clone);
            }
            data.ability1 = abilities.Count >= 1 ? abilities[0] : null;
            data.ability2 = abilities.Count >= 2 ? abilities[1] : null;
            data.ability3 = abilities.Count >= 3 ? abilities[2] : null;
            data.ability4 = abilities.Count >= 4 ? abilities[3] : null;
        }
        return data;
        */
    }
}