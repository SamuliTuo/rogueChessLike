using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    VictoryScreenUnitSlot slot;
    UnitData unitThatsLevelingUp = null;

    Sprite upgrades1_slot1;
    Sprite upgrades1_slot2;
    Sprite upgrades1_slot3;

    Sprite upgrades2_slot1;
    Sprite upgrades2_slot2;
    Sprite upgrades2_slot3;

    public void InitLevelUpPanel(VictoryScreenUnitSlot slot)
    {
        this.slot = slot;
        this.unitThatsLevelingUp = slot.slottedUnit;
    }


}
