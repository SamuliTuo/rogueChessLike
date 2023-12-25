using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScenarioEditorPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI variationText_m1 = null;
    [SerializeField] private TextMeshProUGUI variationText_m2 = null;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI rotationText_m1 = null;
    [SerializeField] private TextMeshProUGUI rotationText_m2 = null;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI unitRotation = null;
    [SerializeField] private TextMeshProUGUI objectRotation = null;

    public void SetVariationText(int mouseButton, string text)
    {
        switch (mouseButton)
        {
            case 1: variationText_m1.text = text; break;
            case 2: variationText_m2.text = text; break;
            default: break;
        }
    }

    public void SetRotationText(int mouseButton, int rot)
    {
        string text;
        switch (rot)
        {
            case 0: text = 0.ToString(); break;
            case 1: text = 90.ToString(); break;
            case 2: text = 180.ToString(); break;
            case 3: text = 270.ToString(); break;
            default: text = 0.ToString(); break;
        }
        switch (mouseButton)
        {
            case 1: rotationText_m1.text = text; break;
            case 2: rotationText_m2.text = text; break;
            default: break;
        }
    }
    public void SetObjectRotationText(int rot)
    {
        string text;
        switch (rot)
        {
            case 0: text = 0.ToString(); break;
            case 1: text = 90.ToString(); break;
            case 2: text = 180.ToString(); break;
            case 3: text = 270.ToString(); break;
            default: text = 0.ToString(); break;
        }
        unitRotation.text = text;
        objectRotation.text = text;
    }
}
