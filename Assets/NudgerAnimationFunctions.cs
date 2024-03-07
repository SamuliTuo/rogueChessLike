using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NudgerAnimationFunctions : MonoBehaviour
{
    NudgeArms controller;

    private void Start()
    {
        controller = GetComponentInParent<NudgeArms>();
    }

    public void StopNudger()
    {
        controller.StopNudger();
    }

    public void StopChipper()
    {
        controller.StopChipper();
    }
}