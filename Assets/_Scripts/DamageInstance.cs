using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public enum DamageInstanceType 
{ 
    SINGLE_TARGET,
    SQUARE,
}

public class DamageInstance : MonoBehaviour
{
    public void Activate(Node target, float damage, Unit shooter, UnitSearchType targeting, DamageInstanceType type, ParticleType particle = ParticleType.NONE)
    {
        Unit[,] units = Chessboard.Instance.GetUnits();
        if (type == DamageInstanceType.SINGLE_TARGET)
        {
            if (units[target.x, target.y] != null)
                if (GameManager.Instance.IsValidTarget(shooter, units[target.x, target.y], targeting))
                    units[target.x, target.y].GetComponent<UnitHealth>().GetDamaged(damage);
                    
        }
        else
        {
            foreach (Vector2Int node in GetSquare(target.x, target.y))
                if (units[node.x, node.y] != null)
                    if (GameManager.Instance.IsValidTarget(shooter, units[node.x, node.y], targeting))
                    {
                        units[node.x, node.y].GetComponent<UnitHealth>().GetDamaged(damage);
                        if (node.x != target.x && node.y != target.y)
                        {
                            GameManager.Instance.ParticleSpawner.SpawnParticles(particle, Chessboard.Instance.GetTileCenter(target.x, target.y));
                        }
                    }
                        
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
