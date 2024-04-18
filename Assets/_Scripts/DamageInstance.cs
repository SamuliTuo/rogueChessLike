using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum DamageInstanceType 
{ 
    SINGLE_TARGET,
    AOE
}

public class DamageInstance : MonoBehaviour
{
    public void Activate(
        Node target,
        Vector3 forward,
        AOEShapes shape,
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
            ActivateSingleTarget(targetUnit, shooter, shooter.team, forward, targeting, damage, critChance, critDamage, missChance, isMagicDmg, statusMods);
        }
        else if (type == DamageInstanceType.AOE)
        {
            ActivateSquare(target, shooter, shooter.team, forward, shape, units, targeting, damage, critChance, critDamage, missChance, isMagicDmg, statusMods, particle);
        }
    }

    public void ActivateAreaDOT(
        Node targetNode,
        Vector3 forward,
        AOEShapes shape,
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
        StartCoroutine(AreaDOT(targetNode, forward, shape, tickDamage, tickIntervalSeconds, critChance, critDamage, missChance, intervals, shooter, shooter.team, targeting, scale, statusMods, isMagicDmg, particle));
    }


    // ================== PRIVATE ================== //
    private void ActivateSingleTarget(
        Unit targetUnit, 
        Unit shooter,
        int shooterTeam,
        Vector3 forward,
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
            // Shooter has died:
            if (shooter == null)
            {
                if (GameManager.Instance.IsValidTarget(shooterTeam, targetUnit, targeting))
                {
                    if (statusMods != null)
                    {
                        targetUnit.GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods, null);
                    }
                    targetUnit.GetComponent<UnitHealth>().RemoveHPAndCheckIfUnitDied(damage, false, critChance, critDamage, missChance, isMagicDmg);
                }
                return;
            }
            // Shooter is alive:
            if (GameManager.Instance.IsValidTarget(shooter, targetUnit, targeting))
            {
                if (statusMods != null)
                {
                    targetUnit.GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods, null);
                }
                if (targetUnit.GetComponent<UnitHealth>().RemoveHPAndCheckIfUnitDied(damage, false, critChance, critDamage, missChance, isMagicDmg) == true)
                {
                    shooter.UnitGotAKill();
                }
                // Lifesteal:
                if (shooter != null)
                {
                    if (shooter.lifeSteal_perc > 0)
                    {
                        shooter.GetComponent<UnitHealth>().RemoveHPAndCheckIfUnitDied(-Mathf.Abs(damage) * shooter.lifeSteal_perc, false, critChance, critDamage, missChance, isMagicDmg);
                    }
                    if (shooter.lifeSteal_flat > 0)
                    {
                        shooter.GetComponent<UnitHealth>().RemoveHPAndCheckIfUnitDied(-shooter.lifeSteal_flat, false, critChance, critDamage, missChance, isMagicDmg);
                    }
                }
            }
        }
    }

    private void ActivateSquare(
        Node target, 
        Unit shooter,
        int shooterTeam,
        Vector3 forward,
        AOEShapes shape,
        Unit[,] units, 
        UnitSearchType targeting, 
        float damage,
        float critChance, float critDamage, float missChance,
        bool isMagicDmg,
        UnitStatusModifier statusMods,
        ParticleType particle)
    {
        var squares = GetAOENodes(target.x, target.y, shape, HelperUtilities.DetermineCompassDir(forward).Item1);
        if (squares == null)
            return;

        if (shooter == null)
        {
            foreach (var node in squares)
            {
                if (units[node.Item1.x, node.Item1.y] != null)
                {
                    if (GameManager.Instance.IsValidTarget(shooterTeam, units[node.Item1.x, node.Item1.y], targeting))
                    {
                        if (statusMods != null)
                        {
                            units[node.Item1.x, node.Item1.y].GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods, null);
                        }
                        units[node.Item1.x, node.Item1.y].GetComponent<UnitHealth>().RemoveHPAndCheckIfUnitDied(damage, false, critChance, critDamage, missChance, isMagicDmg);

                        if (node.Item1.x != target.x && node.Item1.y != target.y)
                        {
                            GameManager.Instance.ParticleSpawner.SpawnParticles(particle, Chessboard.Instance.GetTileCenter(target.x, target.y), transform.forward);
                        }
                    }
                }
            }
            return;
        }
        // Shooter is still alive:
        foreach (var node in squares)
        {
            Instantiate(Resources.Load<GameObject>("DebuggerSphere"), Chessboard.Instance.GetTileCenter(node.Item1.x,node.Item1.y), Quaternion.identity);
            if (units[node.Item1.x,node.Item1.y] != null)
            {
                if (GameManager.Instance.IsValidTarget(shooter, units[node.Item1.x,node.Item1.y], targeting))
                {
                    if (statusMods != null)
                    {
                        units[node.Item1.x, node.Item1.y].GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods, shooter);
                    }
                    var hp = units[node.Item1.x, node.Item1.y].GetComponent<UnitHealth>();
                    if (hp != null)
                    {
                        if (hp.RemoveHPAndCheckIfUnitDied(damage, false, critChance, critDamage, missChance, isMagicDmg))
                        {
                            shooter.UnitGotAKill();
                        }
                    }
                    if (node.Item1.x != target.x && node.Item1.y != target.y)
                    {
                        GameManager.Instance.ParticleSpawner.SpawnParticles(particle, Chessboard.Instance.GetTileCenter(target.x,target.y), transform.forward);
                    }
                    // Lifesteal:
                    if (shooter != null)
                    {
                        if (shooter.lifeSteal_perc > 0)
                        {
                            shooter.GetComponent<UnitHealth>().RemoveHPAndCheckIfUnitDied(-Mathf.Abs(damage) * shooter.lifeSteal_perc, false, critChance, critDamage, missChance, isMagicDmg);
                        }
                        if (shooter.lifeSteal_flat > 0)
                        {
                            shooter.GetComponent<UnitHealth>().RemoveHPAndCheckIfUnitDied(-shooter.lifeSteal_flat, false, critChance, critDamage, missChance, isMagicDmg);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator AreaDOT(
        Node target,
        Vector3 forward,
        AOEShapes shape,
        float tickDamage, 
        float tickIntervalSeconds, 
        float critChance, float critDamage, float missChance,
        int intervals, 
        Unit shooter,
        int shooterTeam,
        UnitSearchType targeting, 
        DamageInstanceType scale,
        UnitStatusModifier statusMods,
        bool isMagicDmg,
        ParticleType particle = ParticleType.NONE)
    {
        int interval = 0;
        while (interval < intervals)
        {
            // Deal damage to units in effected area:
            Unit[,] units = Chessboard.Instance.GetUnits();
            if (scale == DamageInstanceType.SINGLE_TARGET)
            {
                var targetUnit = units[target.x, target.y];
                ActivateSingleTarget(targetUnit, shooter, shooterTeam, forward, targeting, tickDamage, critChance, critDamage, missChance, isMagicDmg, statusMods);
            }
            else if (scale == DamageInstanceType.AOE)
            {
                ActivateSquare(target, shooter, shooterTeam, forward, shape, units, targeting, tickDamage, critChance, critDamage, missChance, isMagicDmg, statusMods, particle);
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


    private List<Tuple<Vector2Int, AOEGridScriptable.NodeInfo>> GetAOENodes(int x, int y, AOEShapes shape, CompassDir orientation)
    {
        var aoe = GameManager.Instance.AOELibrary.GetAOEShape(shape, orientation);
        if (aoe == null)
        {
            return null;
        }
        var r = new List<Tuple<Vector2Int, AOEGridScriptable.NodeInfo>>();
        int tileCountX = Chessboard.Instance.GetTilecount().x;
        int tileCountY = Chessboard.Instance.GetTilecount().y;
        foreach (var ring in aoe)
        {
            for (int i = 0; i < ring.Count; i++)
            {
                if (ring[i] == null)
                    continue;

                if (x + ring[i].x < 0 || x + ring[i].x >= tileCountX || y + ring[i].y < 0 || y + ring[i].y >= tileCountY)
                    continue;

                r.Add(new(new Vector2Int(x + ring[i].x, y + ring[i].y), ring[i]));
            }
        }
        return r;
    }
}


