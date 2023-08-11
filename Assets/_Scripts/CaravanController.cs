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
        var choices = GetUnitChoices();
        foreach (var choice in choices)
        {
            caravanSlots.Add(choice);
        }
        caravanSlot1Image.sprite = GameManager.Instance.UnitSavePaths.GetImg(caravanSlots[0].unitName);
        slot1_spells[0].sprite = caravanSlots[0].ability1 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[0].ability1);
        slot1_spells[1].sprite = caravanSlots[0].ability2 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[0].ability2);
        slot1_spells[2].sprite = caravanSlots[0].ability3 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[0].ability3);
        slot1_spells[3].sprite = caravanSlots[0].ability4 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[0].ability4);
        caravanSlot2Image.sprite = GameManager.Instance.UnitSavePaths.GetImg(caravanSlots[1].unitName);
        slot2_spells[0].sprite = caravanSlots[1].ability1 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[1].ability1);
        slot2_spells[1].sprite = caravanSlots[1].ability2 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[1].ability2);
        slot2_spells[2].sprite = caravanSlots[1].ability3 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[1].ability3);
        slot2_spells[3].sprite = caravanSlots[1].ability4 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[1].ability4);
        caravanSlot3Image.sprite = GameManager.Instance.UnitSavePaths.GetImg(caravanSlots[2].unitName);
        slot3_spells[0].sprite = caravanSlots[2].ability1 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[2].ability1);
        slot3_spells[1].sprite = caravanSlots[2].ability2 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[2].ability2);
        slot3_spells[2].sprite = caravanSlots[2].ability3 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[2].ability3);
        slot3_spells[3].sprite = caravanSlots[2].ability4 == null ? emptyImage : GameManager.Instance.AbilityLibrary.GetImg(caravanSlots[2].ability4);

        caravanPanel.SetActive(true);
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
        var randomUnit = GameManager.Instance.UnitSavePaths.unitsDatas[Random.Range(0, GameManager.Instance.UnitSavePaths.unitsDatas.Count)];
        Vector2Int spawnPos = GameManager.Instance.PlayerParty.GetFirstFreePartyPos();
        UnitData data = new UnitData(randomUnit.unitPrefab.GetComponent<Unit>(), spawnPos.x, spawnPos.y);

        var unitAbilityManager = randomUnit.unitPrefab.GetComponent<UnitAbilityManager>();
        List<UnitAbility> abilities = new List<UnitAbility>();
        int[] abilChoices = GenerateRandomUniqueIntegers(new Vector2Int(1, 3), new Vector2Int(0, unitAbilityManager.possibleAbilities.Count));
        //print("abil choices: " + abilChoices + ", name: " + data.unitName);
        if (abilChoices != null)
        {
            foreach (int randomInt in abilChoices)
            {
                abilities.Add(unitAbilityManager.possibleAbilities[randomInt]);
            }
            data.ability1 = abilities.Count >= 1 ? abilities[0] : null;
            data.ability2 = abilities.Count >= 2 ? abilities[1] : null;
            data.ability3 = abilities.Count >= 3 ? abilities[2] : null;
            data.ability4 = abilities.Count >= 4 ? abilities[3] : null;
        }
        //
        return data;
    }
    
    private int[] GenerateRandomUniqueIntegers(Vector2Int countRange, Vector2Int valueRange)
    {
        if (valueRange == Vector2Int.zero)
            return null;

        var values = new List<int>();
        for (int i = Mathf.Min(valueRange.x, valueRange.y); i < Mathf.Max(valueRange.x, valueRange.y); i++)
            values.Add(i);

        var randomNumbers = new int[Random.Range(Mathf.Min(countRange.x, countRange.y), Mathf.Max(countRange.x, countRange.y))];
        for (int i = 0; i < randomNumbers.Length; i++)
        {
            if (values.Count == 0)
                continue;

            var thisNumber = Random.Range(0, values.Count);
            randomNumbers[i] = values[thisNumber];
            values.RemoveAt(thisNumber);
        }

        return randomNumbers;
    }
}
