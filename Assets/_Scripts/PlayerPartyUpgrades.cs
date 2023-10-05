using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPartyUpgrades : MonoBehaviour
{
    private PlayerParty playerParty;

    private void Start()
    {
        playerParty = GetComponent<PlayerParty>();
    }

    public void GiveUnitNewSpell(Unit unit, UnitAbility ability, int spellSlot)
    {
        var unitAbilities = unit.GetComponent<UnitAbilityManager>();
        switch (spellSlot)
        {
            case 0:
                unitAbilities.ability_1 = ability;
                break;
            case 1:
                unitAbilities.ability_2 = ability;
                break;
            case 2:
                unitAbilities.ability_3 = ability;
                break;
            default:
                unitAbilities.ability_4 = ability;
                break;
        }
    }

    public void GiveSpellAttributes(UnitAbility ability, float damageIncrease, int bouncesIncrease)
    {
        ability.damage += damageIncrease;
        ability.bounceSpawnCount_ability += bouncesIncrease;
    }

}
