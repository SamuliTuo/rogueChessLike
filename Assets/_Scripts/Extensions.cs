using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static List<T> GetKeysByValue<T, W>(this IDictionary<T, W> dict, W value)
    {
        List<T> keys = new List<T>();
        foreach (KeyValuePair<T, W> kvp in dict)
        {
            if (EqualityComparer<W>.Default.Equals(kvp.Value, value))
            {
                keys.Add(kvp.Key);
            }
        }
        return keys;
    }

    public static KeyValuePair<T1, T2> ToPair<T1, T2>(this Tuple<T1, T2> source)
    {
        return new KeyValuePair<T1, T2>(source.Item1, source.Item2);
    }

    public static Tuple<T1, T2> ToTuple<T1, T2>(this KeyValuePair<T1, T2> source)
    {
        return Tuple.Create(source.Key, source.Value);
    }

    public static int ReachToRange(int reach)
    {
        //So melee attacks with reach of 1 - node reach also diagonally
        if (reach == 1)
        {
            return 14;
        }
        //Arbirary value of 12 which is between 10 and 14 cause those are the cardinal and diagonal movement gCost for pathfinding when calculating
        else if (reach > 1)
        {
            return reach * 12;
        }
        return 0;
    }

    public static List<Vector2Int> GetFreeNodesAtAndAround(int x, int y)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        var units = Chessboard.Instance.GetUnits();
        Vector2Int tilecount = Chessboard.Instance.GetTilecount();

        // Current pos
        if (units[x,y] == null) 
            r.Add(new Vector2Int(x, y));

        // Right
        if (x + 1 < tilecount.x)
        {
            // Right
            if (units[x + 1, y] == null)
                r.Add(new Vector2Int(x + 1, y));
            //else if (units[x + 1, y].team != team)
                //r.Add(new Vector2Int(x + 1, y));

            // Top right
            if (y + 1 < tilecount.y)
                if (units[x + 1, y + 1] == null)
                    r.Add(new Vector2Int(x + 1, y + 1));
                //else if (units[x + 1, y + 1].team != team)
                    //r.Add(new Vector2Int(x + 1, y + 1));

            // Bottom right
            if (y - 1 >= 0)
                if (units[x + 1, y - 1] == null)
                    r.Add(new Vector2Int(x + 1, y - 1));
                //else if (units[x + 1, y - 1].team != team)
                    //r.Add(new Vector2Int(x + 1, y - 1));
        }
        // Left
        if (x - 1 >= 0)
        {
            // Left
            if (units[x - 1, y] == null)
                r.Add(new Vector2Int(x - 1, y));
            //else if (units[x - 1, y].team != team)
                //r.Add(new Vector2Int(x - 1, y));

            // Top left
            if (y + 1 < tilecount.y)
                if (units[x - 1, y + 1] == null)
                    r.Add(new Vector2Int(x - 1, y + 1));
                //else if (units[x - 1, y + 1].team != team)
                    //r.Add(new Vector2Int(x - 1, y + 1));

            // Bottom left
            if (y - 1 >= 0)
                if (units[x - 1, y - 1] == null)
                    r.Add(new Vector2Int(x - 1, y - 1));
                //else if (units[x - 1, y - 1].team != team)
                    //r.Add(new Vector2Int(x - 1, y - 1));
        }
        // Up
        if (y + 1 < tilecount.y)
            if (units[x, y + 1] == null) // || units[x, y + 1].team != team)
                r.Add(new Vector2Int(x, y + 1));
        // Down
        if (y - 1 >= 0)
            if (units[x, y - 1] == null) // || units[x, y - 1].team != team)
                r.Add(new Vector2Int(x, y - 1));

        return r;
    }
}