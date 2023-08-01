using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaravanController : MonoBehaviour
{
    [SerializeField] GameObject caravanPanel;

    private List<UnitAndSavePath> units;
    private UnitAndSavePath caravanSlot1;
    private UnitAndSavePath caravanSlot2;
    private UnitAndSavePath caravanSlot3;
    private Image caravanSlot1Image;
    private Image caravanSlot2Image;
    private Image caravanSlot3Image;

    private void Start()
    {
        caravanSlot1Image = caravanPanel.transform.Find("Image1").GetComponent<Image>();
        caravanSlot2Image = caravanPanel.transform.Find("Image2").GetComponent<Image>();
        caravanSlot3Image = caravanPanel.transform.Find("Image3").GetComponent<Image>();
    }

    public void OpenCaravan()
    {
        var choices = GetUnitChoices();
        caravanSlot1 = choices[0];
        caravanSlot2 = choices[1];
        caravanSlot3 = choices[2];
        caravanSlot1Image.sprite = caravanSlot1.image;
        caravanSlot2Image.sprite = caravanSlot2.image;
        caravanSlot3Image.sprite = caravanSlot3.image;
        caravanPanel.SetActive(true);
    }
    public void ChooseUnit(int chosenSlot)
    {
        //print("huhuu");
        //choose a unit from presented selection
        var choices = GetUnitChoices();

        //add unit to party
        if (chosenSlot == 0)
        {
            GameManager.Instance.PlayerParty.AddUnit(caravanSlot1.unitPrefab.GetComponent<Unit>());
        }
        else if (chosenSlot == 1)
        {
            GameManager.Instance.PlayerParty.AddUnit(caravanSlot2.unitPrefab.GetComponent<Unit>());
        }
        else if (chosenSlot == 2)
        {
            GameManager.Instance.PlayerParty.AddUnit(caravanSlot3.unitPrefab.GetComponent<Unit>());
        }

        //close panel
        CloseCaravan();
    }


    List<UnitAndSavePath> GetUnitChoices()
    {
        var r = new List<UnitAndSavePath>();
        for (int i = 0; i < 3; i++)
        {
            r.Add(GameManager.Instance.UnitSavePaths.unitsDatas[Random.Range(0, GameManager.Instance.UnitSavePaths.unitsDatas.Count)]);
        }
        return r;
    }

    void CloseCaravan() 
    { 
        caravanPanel.SetActive(false);
        GameManager.Instance.MapController.SetCanMove(true);
    }
}