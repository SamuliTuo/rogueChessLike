using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NudgeArms : MonoBehaviour
{
    [SerializeField] private Transform nudger = null;
    [SerializeField] private Transform chipper = null;
    [SerializeField] private Animator nudgerAnimator = null;
    [SerializeField] private Animator chipperAnimator = null;

    
    public void StartNudger(Vector3 position)
    {
        nudger.gameObject.SetActive(true);
        nudger.position = position;
        //nudger.rotation = Quaternion.LookRotation(direction, Vector3.up);
        nudgerAnimator.Play("nudgeArm_animation_lower");
    }
    public void StartChipper(Vector3 position)
    {
        chipper.gameObject.SetActive(true);
        chipper.position = position;
        //chipper.rotation = Quaternion.LookRotation(direction, Vector3.up);
        chipperAnimator.Play("chipArm_animation_lower");
    }



    public void UpdateNudgerAimPosition(float aimPerc, Vector3 position, Vector3 direction)
    {
        print("charging nudge, perc: " + aimPerc);
        nudger.position = position;
        nudger.rotation = Quaternion.LookRotation(direction, Vector3.up);
        nudgerAnimator.SetFloat("nudgerAimPerc", aimPerc);
    }
    public void UpdateChipperAimPosition(float aimPerc, Vector3 position, Vector3 direction)
    {
        print("charging chip, perc: " + aimPerc);
        chipper.position = position;
        chipper.rotation = Quaternion.LookRotation(direction, Vector3.up);
        chipperAnimator.SetFloat("chipperAimPerc", aimPerc);
    }



    public void NudgerNudge()
    {
        nudgerAnimator.Play("nudgeArm_animation_slap");
    }
    public void ChipperChip()
    {
        chipperAnimator.Play("chipArm_animation_chip");
    }



    public void StopChipper()
    {
        chipper.gameObject.SetActive(false);
    }
    public void StopNudger()
    {
        nudger.gameObject.SetActive(false);
    }
}
