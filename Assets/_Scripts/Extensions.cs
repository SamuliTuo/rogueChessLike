using System;
using System.Collections.Generic;

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
}