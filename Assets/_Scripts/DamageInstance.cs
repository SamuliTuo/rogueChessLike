using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum DamageInstanceType 
{ 
    SINGLE_TARGET,
    SQUARE,
    DOT,
    AOE
}

public class DamageInstance : MonoBehaviour
{
    public void Activate(
        Node target,
        float damage,
        float critChance,
        float critDamage,
        float missChance,
        Unit shooter, 
        UnitSearchType targeting, 
        DamageInstanceType type, 
        UnitStatusModifier statusMods,
        bool isMagicDmg,
        ParticleType particle = ParticleType.NONE)
    {   
        Unit[,] units = Chessboard.Instance.GetUnits();
        var targetUnit = units[target.x, target.y];
        if (type == DamageInstanceType.SINGLE_TARGET)
        {
            ActivateSingleTarget(targetUnit, shooter, targeting, damage, critChance, critDamage, missChance, isMagicDmg, statusMods);
        }
        else if (type == DamageInstanceType.SQUARE)
        {
            ActivateSquare(target, shooter, units, targeting, damage, critChance, critDamage, missChance, isMagicDmg, statusMods, particle);
        }
    }

    public void ActivateAreaDOT(
        Node targetNode, 
        float tickDamage,
        float tickIntervalSeconds,
        float critChance, float critDamage, float missChance,
        int intervals, 
        Unit shooter, 
        UnitSearchType targeting, 
        DamageInstanceType scale, 
        UnitStatusModifier statusMods, 
        bool isMagicDmg,
        ParticleType particle = ParticleType.NONE)
    {
        StartCoroutine(AreaDOT(targetNode, tickDamage, tickIntervalSeconds, critChance, critDamage, missChance, intervals, shooter, targeting, scale, statusMods, isMagicDmg, particle));
    }


    // ================== PRIVATE ================== //
    private void ActivateSingleTarget(
        Unit targetUnit, 
        Unit shooter, 
        UnitSearchType targeting, 
        float damage,
        float critChance,
        float critDamage,
        float missChance,
        bool isMagicDmg,
        UnitStatusModifier statusMods)
    {
        //print("target unit: "+ targetUnit);
        if (targetUnit != null)
        {
            //print("valid target: " + GameManager.Instance.IsValidTarget(shooter, targetUnit, targeting));
            //print("============ shooter: " + shooter + ", targetUnit: " + targetUnit + ", targeting: " + targeting);
            if (GameManager.Instance.IsValidTarget(shooter, targetUnit, targeting))
            {
                //print("stat mods: " + statusMods);
                if (statusMods != null)
                {
                    targetUnit.GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods);
                }
                //print("damage: "+damage+"target unit hp: "+targetUnit.GetComponent<UnitHealth>());
                targetUnit.GetComponent<UnitHealth>().RemoveHP(damage, false, critChance, critDamage, missChance, isMagicDmg);
                // Lifesteal:
                if (shooter != null)
                {
                    if (shooter.lifeSteal_perc > 0)
                    {
                        shooter.GetComponent<UnitHealth>().RemoveHP(-Mathf.Abs(damage) * shooter.lifeSteal_perc, false, critChance, critDamage, missChance, isMagicDmg);
                    }
                    if (shooter.lifeSteal_flat > 0)
                    {
                        shooter.GetComponent<UnitHealth>().RemoveHP(-shooter.lifeSteal_flat, false, critChance, critDamage, missChance, isMagicDmg);
                    }
                }
            }
        }
    }
    private void ActivateSingleTarget(
        Unit targetUnit, 
        int shooterTeam, 
        UnitSearchType targeting, 
        float damage,
        float critChance, float critDamage, float missChance,
        bool isMagicDmg,
        UnitStatusModifier statusMods)
    {
        if (targetUnit != null)
        {
            if (GameManager.Instance.IsValidTarget(shooterTeam, targetUnit, targeting))
            {
                if (statusMods != null)
                {
                    targetUnit.GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods);
                }
                targetUnit.GetComponent<UnitHealth>().RemoveHP(damage, false, critChance, critDamage, missChance, isMagicDmg);
            }
        }
    }

    private void ActivateSquare(
        Node target, 
        Unit shooter, 
        Unit[,] units, 
        UnitSearchType targeting, 
        float damage,
        float critChance, float critDamage, float missChance,
        bool isMagicDmg,
        UnitStatusModifier statusMods,
        ParticleType particle)
    {
        foreach (Vector2Int node in GetSquare(target.x,target.y))
        {
            if (units[node.x,node.y] != null)
            {
                if (GameManager.Instance.IsValidTarget(shooter, units[node.x,node.y], targeting))
                {
                    if (statusMods != null)
                    {
                        units[node.x, node.y].GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods);
                    }
                    var hp = units[node.x, node.y].GetComponent<UnitHealth>();
                    if (hp != null)
                        hp.RemoveHP(damage, false, critChance, critDamage, missChance, isMagicDmg);

                    if (node.x != target.x && node.y != target.y)
                    {
                        GameManager.Instance.ParticleSpawner.SpawnParticles(particle, Chessboard.Instance.GetTileCenter(target.x,target.y), transform.forward);
                    }
                    // Lifesteal:
                    if (shooter != null)
                    {
                        if (shooter.lifeSteal_perc > 0)
                        {
                            shooter.GetComponent<UnitHealth>().RemoveHP(-Mathf.Abs(damage) * shooter.lifeSteal_perc, false, critChance, critDamage, missChance, isMagicDmg);
                        }
                        if (shooter.lifeSteal_flat > 0)
                        {
                            shooter.GetComponent<UnitHealth>().RemoveHP(-shooter.lifeSteal_flat, false, critChance, critDamage, missChance, isMagicDmg);
                        }
                    }
                }
            }
        }
    }
    private void ActivateSquare(
        Node target, 
        int shooterTeam, 
        Unit[,] units, 
        UnitSearchType targeting, 
        float damage, 
        float critChance, float critDamage, float missChance,
        bool isMagicDmg,
        UnitStatusModifier statusMods,
        ParticleType particle)
    {
        foreach (Vector2Int node in GetSquare(target.x, target.y))
        {
            if (units[node.x, node.y] != null)
            {
                if (GameManager.Instance.IsValidTarget(shooterTeam, units[node.x, node.y], targeting))
                {
                    if (statusMods != null)
                    {
                        units[node.x, node.y].GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods);
                    }
                    units[node.x, node.y].GetComponent<UnitHealth>().RemoveHP(damage, false, critChance, critDamage, missChance, isMagicDmg);

                    if (node.x != target.x && node.y != target.y)
                    {
                        GameManager.Instance.ParticleSpawner.SpawnParticles(particle, Chessboard.Instance.GetTileCenter(target.x, target.y), transform.forward);
                    }
                }
            }
        }
    }

    private IEnumerator AreaDOT(
        Node target, 
        float tickDamage, 
        float tickIntervalSeconds, 
        float critChance, float critDamage, float missChance,
        int intervals, 
        Unit shooter, 
        UnitSearchType targeting, 
        DamageInstanceType scale,
        UnitStatusModifier statusMods,
        bool isMagicDmg,
        ParticleType particle = ParticleType.NONE)
    {
        int shooterTeam = shooter.team;
        int interval = 0;
        while (interval < intervals)
        {
            // Deal damage to units in effected area:
            Unit[,] units = Chessboard.Instance.GetUnits();
            if (scale == DamageInstanceType.SINGLE_TARGET)
            {
                var targetUnit = units[target.x, target.y];
                ActivateSingleTarget(targetUnit, shooterTeam, targeting, tickDamage, critChance, critDamage, missChance, isMagicDmg, statusMods);
            }
            else if (scale == DamageInstanceType.SQUARE)
            {
                ActivateSquare(target, shooterTeam, units, targeting, tickDamage, critChance, critDamage, missChance, isMagicDmg, statusMods, particle);
            }

            // Wait for the next interval:
            float t = 0;
            while (t < tickIntervalSeconds)
            {
                t += Time.deltaTime;
                yield return null;
            }
            interval++;
            yield return null;
        }
    }

    private List<Vector2Int> GetSquare(
        int x, int y)
    {
        int tileCountX = Chessboard.Instance.GetTilecount().x;
        int tileCountY = Chessboard.Instance.GetTilecount().y;
        List<Vector2Int> r = new List<Vector2Int>();

        // Current pos
        r.Add(new Vector2Int(x, y));

        // Right
        if (x + 1 < tileCountX)
        {
            // Right
            r.Add(new Vector2Int(x + 1, y));
            // Top right
            if (y + 1 < tileCountY)
                r.Add(new Vector2Int(x + 1, y + 1));
            // Bottom right
            if (y - 1 >= 0)
                r.Add(new Vector2Int(x + 1, y - 1));
        }
        // Left
        if (x - 1 >= 0)
        {
            // Left
            r.Add(new Vector2Int(x - 1, y));
            // Top left
            if (y + 1 < tileCountY)
                r.Add(new Vector2Int(x - 1, y + 1));
            // Bottom left
            if (y - 1 >= 0)
                r.Add(new Vector2Int(x - 1, y - 1));

        }
        // Up
        if (y + 1 < tileCountY)
            r.Add(new Vector2Int(x, y + 1));
        // Down
        if (y - 1 >= 0)
            r.Add(new Vector2Int(x, y - 1));


        return r;
    }
}
