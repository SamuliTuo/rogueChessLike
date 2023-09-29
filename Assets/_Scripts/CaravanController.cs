using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CaravanController : MonoBehaviour
{
    [SerializeField] GameObject caravanPanel;

    private List<UnitData> caravanSlots = new List<UnitData>();
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
        List<UnitData> choices = GetUnitChoices();
        foreach (var choice in choices)
        {
            caravanSlots.Add(choice);
        }
        caravanSlot1Image.sprite = GameManager.Instance.UnitSavePaths.GetImg(caravanSlots[0].unitName);
        caravanSlot2Image.sprite = GameManager.Instance.UnitSavePaths.GetImg(caravanSlots[1].unitName);
        caravanSlot3Image.sprite = GameManager.Instance.UnitSavePaths.GetImg(caravanSlots[2].unitName);
        SetSpellImages(caravanSlots[0], slot1_spells);
        SetSpellImages(caravanSlots[1], slot2_spells);
        SetSpellImages(caravanSlots[2], slot3_spells);
        caravanPanel.SetActive(true);
    }

    void SetSpellImages(UnitData slot, Image[] accordingImageSlots)
    {
        accordingImageSlots[0].sprite = slot.ability1 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(slot.ability1);
        accordingImageSlots[1].sprite = slot.ability2 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(slot.ability2);
        accordingImageSlots[2].sprite = slot.ability3 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(slot.ability3);
        accordingImageSlots[3].sprite = slot.ability4 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(slot.ability4);
    }

    public void ChooseUnit(int chosenSlot)
    {
        GameManager.Instance.PlayerParty.AddUnit(caravanSlots[chosenSlot]);
        CloseCaravan();
    }


    List<UnitData> GetUnitChoices()
    {
        var r = new List<UnitData>();
        for (int i = 0; i < 3; i++)
        {
            r.Add(GetARandomCaravanChoice());
        }
        return r;
    }

    void CloseCaravan()
    {
        caravanPanel.SetActive(false);
        GameManager.Instance.MapController.SetCanMove(true);
    }

    UnitData GetARandomCaravanChoice()
    {
        UnitAndSavePath randomUnit = null;
        foreach (var unit in GameManager.Instance.UnitSavePaths.unitsDatas)
        {
            if (unit.unitPrefab.name == "Unit_penguin")
                randomUnit = unit;
        }
        
        //GameManager.Instance.UnitSavePaths.unitsDatas[Random.Range(0, GameManager.Instance.UnitSavePaths.unitsDatas.Count)];
        //var randomUnit = GameManager.Instance.UnitSavePaths.unitsDatas[Random.Range(0, GameManager.Instance.UnitSavePaths.unitsDatas.Count)];
        Vector2Int spawnPos = GameManager.Instance.PlayerParty.GetFirstFreePartyPos();
        UnitData data = new UnitData(randomUnit.unitPrefab.GetComponent<Unit>(), spawnPos.x, spawnPos.y);

        // Clone and set the attacks for the unit:
        Unit unitScript = randomUnit.unitPrefab.GetComponent<Unit>();
        List<Unit_NormalAttack> clonedAttacks = new List<Unit_NormalAttack>();
        for (int i = 0; i < unitScript.normalAttacks.Count; i++)
        {
            string attackName = unitScript.normalAttacks[i].name;
            var clone = Instantiate(unitScript.normalAttacks[i]);
            clone.name = attackName;
            clonedAttacks.Add(clone);
        }
        foreach (var attack in clonedAttacks)
        {
            data.attacks.Add(attack);
        }
        
        // Clone and set the abilities for the unit:
        var unitAbilityManager = randomUnit.unitPrefab.GetComponent<UnitAbilityManager>();
        List<UnitAbility> abilities = new List<UnitAbility>();
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
    }
}