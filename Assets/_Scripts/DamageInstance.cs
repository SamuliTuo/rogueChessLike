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
            ActivateSingleTarget(targetUnit, shooter, forward, targeting, damage, critChance, critDamage, missChance, isMagicDmg, statusMods);
        }
        else if (type == DamageInstanceType.AOE)
        {
            ActivateSquare(target, shooter, forward, shape, units, targeting, damage, critChance, critDamage, missChance, isMagicDmg, statusMods, particle);
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
        StartCoroutine(AreaDOT(targetNode, forward, shape, tickDamage, tickIntervalSeconds, critChance, critDamage, missChance, intervals, shooter, targeting, scale, statusMods, isMagicDmg, particle));
    }


    // ================== PRIVATE ================== //
    private void ActivateSingleTarget(
        Unit targetUnit, 
        Unit shooter,
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
        Vector3 forward,
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

    public GameObject debugger; //
    private void ActivateSquare(
        Node target, 
        Unit shooter,
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

        foreach (var node in squares)
        {
            Instantiate(Resources.Load<GameObject>("DebuggerSphere"), Chessboard.Instance.GetTileCenter(node.Item1.x,node.Item1.y), Quaternion.identity);
            if (units[node.Item1.x,node.Item1.y] != null)
            {
                if (GameManager.Instance.IsValidTarget(shooter, units[node.Item1.x,node.Item1.y], targeting))
                {
                    if (statusMods != null)
                    {
                        units[node.Item1.x, node.Item1.y].GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods);
                    }
                    var hp = units[node.Item1.x, node.Item1.y].GetComponent<UnitHealth>();
                    if (hp != null)
                        hp.RemoveHP(damage, false, critChance, critDamage, missChance, isMagicDmg);

                    if (node.Item1.x != target.x && node.Item1.y != target.y)
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

        foreach (var node in squares)
        {
            if (units[node.Item1.x, node.Item1.y] != null)
            {
                if (GameManager.Instance.IsValidTarget(shooterTeam, units[node.Item1.x, node.Item1.y], targeting))
                {
                    if (statusMods != null)
                    {
                        units[node.Item1.x, node.Item1.y].GetComponent<UnitStatusModifiersHandler>().AddNewStatusModifiers(statusMods);
                    }
                    units[node.Item1.x, node.Item1.y].GetComponent<UnitHealth>().RemoveHP(damage, false, critChance, critDamage, missChance, isMagicDmg);

                    if (node.Item1.x != target.x && node.Item1.y != target.y)
                    {
                        GameManager.Instance.ParticleSpawner.SpawnParticles(particle, Chessboard.Instance.GetTileCenter(target.x, target.y), transform.forward);
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
                ActivateSingleTarget(targetUnit, shooterTeam, forward, targeting, tickDamage, critChance, critDamage, missChance, isMagicDmg, statusMods);
            }
            else if (scale == DamageInstanceType.AOE)
            {
                ActivateSquare(target, shooterTeam, forward, shape, units, targeting, tickDamage, critChance, critDamage, missChance, isMagicDmg, statusMods, particle);
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
        print("Activating AOE with direction: " + orientation);
        var aoe = GameManager.Instance.AOELibrary.GetAOEShape(shape, orientation);
        if (aoe == null)
        {
            return null;
        }
        var r = new List<Tuple<Vector2Int, AOEGridScriptable.NodeInfo>>();
        int tileCountX = Chessboard.Instance.GetTilecount().x;
        int tileCountY = Chessboard.Instance.GetTilecount().y;
        //List<Vector2Int> r = new List<Vector2Int>();


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
        //var grid = GameManager.Instance.AOELibrary.GetAEO(aoe, orientation);

        // Loop through grid
        // Make nodedamageinfo array for each node that will get damaged by the attack.
        // Order them from outside in so that pushes happen properly. (I hope :P )

        return r;
    }
}


