using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEncounterResponse : MonoBehaviour
{
    public TextMeshProUGUI textField;
    ResponseRequirementsAndReward response;

    

    public void SetupResponse(ResponseRequirementsAndReward response)
    {
        if (textField == null)
        {
            textField = GetComponent<TextMeshProUGUI>();
        }
        textField.text = response.response;
        this.response = response;

    }
}
