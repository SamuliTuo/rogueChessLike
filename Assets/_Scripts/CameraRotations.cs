using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Windows;
using System;

public class CameraRotations : MonoBehaviour
{
    [SerializeField] private string axis = "";

    private TMP_InputField inputField;
    private ScenarioBuilderCameraSettings camSettings;


    public void SetAxis()
    {
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();

        if (camSettings == null) 
            camSettings = GetComponentInParent<ScenarioBuilderCameraSettings>();

        camSettings.ScenarioBuilderCameraRotationAxisChanged(axis, float.Parse(inputField.text));
    }
}
