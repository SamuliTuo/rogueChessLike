using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAugmentsManager : MonoBehaviour
{
    private Unit unit;
    private UnitHealth hp;
    private UnitAbilityManager abilities;
    private Chessboard board;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        hp = GetComponent<UnitHealth>();
        abilities = GetComponent<UnitAbilityManager>();
    }

    public void ActivateAugmentEffects()
    {
        if(unit.team == 1)
        {
            return;
        }
        print("unitdata: " + unit.unitData + ", augments: " + unit.unitData.augments.Count);
        foreach (var aug in unit.unitData.augments)
        {
            print("activating " + aug);
            switch (aug)
            {
                case UnitAugment.BEST_FRIENDS: BestFriends(); break;
                case UnitAugment.BURNING_ATTACKS: BurningAttacks(); break;
                case UnitAugment.ICY_ATTACKS: IcyAttacks(); break;
                case UnitAugment.ACCELERATING_ATTACKS: AcceleratingAttacks(); break;
                case UnitAugment.BLOOD_MONEY: BloodMoney(); break;
                case UnitAugment.MONEY_GRUBBER: MoneyGrubber(); break;
                case UnitAugment.RANDOM_ITEM: RandomItem(); break;
                case UnitAugment.RANDOM_ARTIFACT: RandomArtifact(); break;
                case UnitAugment.FAST_LEARNER: FastLearner(); break;
                case UnitAugment.ITEM_HEALTH: ItemHealth(); break;
                case UnitAugment.COOLDOWNER: Cooldowner(); break;
                case UnitAugment.EMPTY_HANDS: EmptyHands(); break;
                case UnitAugment.LIFE_STEALER: LifeStealer(); break;
                case UnitAugment.KILL_HEALS: KillHeals(); break;
                default:
                    break;
            }
        }
    }


    void BestFriends()
    {
        if (board == null)
            board = Chessboard.Instance;

        if (board == null)
            return;

        var units = board.GetUnits();
        int nearbyUnits = 0;
        var neighbors = board.GetNeighbourNodes(board.nodes[unit.x, unit.y]);

        foreach (var neighbor in neighbors)
        {
            if (units[neighbor.x, neighbor.y] != null)
            {
                nearbyUnits++;
            }
        }
        if (nearbyUnits == 1)
        {
            print("Best friends ACTIVATE! (Give attack speed or something to the unit.)");
            unit.attackSpeed += GameManager.Instance.UnitAugments.bestFriendsAttackSPD;
            hp.armor += GameManager.Instance.UnitAugments.bestFriendsArmor;
        }
        else
        {
            print("Best friends, more like NU-UH!");
        }
    }

    void BurningAttacks()
    {
        print("trying to add burn now, attacks: " + unit.normalAttacks);
        foreach (var attack in unit.normalAttacks)
        {
            print("adding burn to attacks");
            if (attack.Item1.statusModifiers == null)
            {
                print("creating new statusmod");
                attack.Item1.statusModifiers = ScriptableObject.CreateInstance<UnitStatusModifier>();
            }
            attack.Item1.statusModifiers.burningAttacks = true;
            attack.Item1.statusModifiers.burningAttacks_tickDamage = GameManager.Instance.UnitAugments.burningAttacksTickDamage;
            attack.Item1.statusModifiers.burningAttacks_intervalCount = GameManager.Instance.UnitAugments.burningAttacksTickCount;
            attack.Item1.statusModifiers.burningAttacks_tickInterval = GameManager.Instance.UnitAugments.burningAttacksTickInterval;
        }
    }

    void IcyAttacks()
    {
        print("ice this, bitch!");
    }

    void AcceleratingAttacks()
    {
        print("accium!");
    }

    void BloodMoney()
    {
        hp.bloodMoneyAmount = GameManager.Instance.UnitAugments.bloodMoneyAmount;
    }

    void MoneyGrubber()
    {
        unit.moneyOnKill = GameManager.Instance.UnitAugments.moneyGrubberAmount;
    }

    void RandomItem()
    {
        print("Offer the unit 3 random COMPLETED ITEMS to choose from.");
    }

    void RandomArtifact()
    {
        print("Offer the unit 3 random ARTIFACTS to choose from.");
    }

    void FastLearner()
    {
        unit.unitData.experienceGainMultiplier = GameManager.Instance.UnitAugments.fastLearnerMult;
    }

    void ItemHealth()
    {
        print("Count the items and add HP accordingly. For now just using a random int 0 - 3.");
        int itemCount = Random.Range(0, 4);
        hp.AddMaxHealth(itemCount * GameManager.Instance.UnitAugments.itemHealthAmount);
    }

    void Cooldowner()
    {
        abilities.cooldownReduction = GameManager.Instance.UnitAugments.cooldownerReduction;
    }

    void EmptyHands()
    {
        print("If unit has no items, activate this effect");
    }

    void LifeStealer()
    {
        unit.lifeSteal_perc += GameManager.Instance.UnitAugments.lifeStealerAmount;
        print("gave " + GameManager.Instance.UnitAugments.lifeStealerAmount + " lifesteal to " + unit.name + ", new lifesteal = " + unit.lifeSteal_perc);
    }

    void KillHeals()
    {
        unit.healingOnKill = GameManager.Instance.UnitAugments.killHealsAmount;
    }
}
