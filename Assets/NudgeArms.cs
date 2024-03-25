using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NudgeArms : MonoBehaviour
{
    [SerializeField] private Transform nudger = null;
    [SerializeField] private Transform chipper = null;
    [SerializeField] private Transform placer = null;
    [SerializeField] private Animator nudgerAnimator = null;
    [SerializeField] private Animator chipperAnimator = null;
    [SerializeField] private Animator placerAnimator = null;

    private bool placing = false;

    private void Start()
    {
        nudger.gameObject.SetActive(false);
        chipper.gameObject.SetActive(false);
        placer.gameObject.SetActive(false);
    }

    public void StartNudger(Vector3 position)
    {
        nudger.gameObject.SetActive(true);
        nudger.position = position;
        //nudger.rotation = Quaternion.LookRotation(direction, Vector3.up);
        nudgerAnimator.Play("nudgeArm_animation_lower", 0, 0);
    }
    public void StartChipper(Vector3 position)
    {
        chipper.gameObject.SetActive(true);
        chipper.position = position;
        //chipper.rotation = Quaternion.LookRotation(direction, Vector3.up);
        chipperAnimator.Play("chipArm_animation_lower", 0, 0);
    }
    public void StartPlacer(Vector3 position)
    {
        placing = true;
        placer.gameObject.SetActive(true);
        placer.position = position;
        placerAnimator.Play("placer_animation_start", 0, 0);
    }
    


    public void UpdateNudgerAimPosition(float aimPerc, Vector3 position, Vector3 direction)
    {
        nudger.position = position;
        nudger.rotation = Quaternion.LookRotation(direction, Vector3.up);
        nudgerAnimator.SetFloat("nudgerAimPerc", aimPerc);
    }
    public void UpdateChipperAimPosition(float aimPerc, Vector3 position, Vector3 direction)
    {
        chipper.position = position;
        chipper.rotation = Quaternion.LookRotation(direction, Vector3.up);
        chipperAnimator.SetFloat("chipperAimPerc", aimPerc);
    }
    public void UpdatePlacerPosition(Vector3 position)
    {
        placer.position = position;
    }



    public void NudgerNudge()
    {
        nudgerAnimator.Play("nudgeArm_animation_slap");
    }
    public void ChipperChip()
    {
        chipperAnimator.Play("chipArm_animation_chip");
    }
    public void EndPlacer()
    {
        placing = false;
        placerAnimator.Play("placer_animation_end");
    }



    public void StopChipper()
    {
        chipper.gameObject.SetActive(false);
    }
    public void StopNudger()
    {
        nudger.gameObject.SetActive(false);
    }
    public void StopPlacer()
    {
        if (!placing)
        {
            placer.gameObject.SetActive(false);
        }
    }
}
