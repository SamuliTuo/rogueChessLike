using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ScenarioBuilderCameraSettings : MonoBehaviour
{
    CameraManager camManager;
    float x = 45;
    float y = -45;
    float z = 0;

    public Vector3 GetScenarioCameraRotation()
    {
        return new Vector3(x,y,z);
    }

    public void ScenarioBuilderCameraRotationAxisChanged(string axis, float value)
    {
        switch (axis)
        {
            case "x": x = value; break;
            case "y": y = value; break; 
            case "z": z = value; break;
            default: break;
        }

        if (camManager == null)
        {
            camManager = Camera.main.transform.GetComponent<CameraManager>();
        }
        camManager.SetCamera(new Vector3(x,y,z));
    }
}
