using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes
{
    BASIC, ARTIFACT, BARRIER,
}
public enum ItemUniqueEffects
{
    NONE, FLYING, EXPLODING
}

public class ItemLibrary : MonoBehaviour
{
    [Header("Color codes to use for stats:\n" +
        "(These are examples)\n" +
        "(Use: <color=#123456>text</color>\n\n" +
        "Unique = ff0fe6 \n" +
        "HP = b70202 \n" +
        "DMG = dfa343 \n" +
        "Magic = 43d7df \n" +
        "crit = d1c748 \n" +
        "AttSpd = 48d15b \n" +
        "Armor = b0ab5c \n" +
        "M.Res = 6667a8 \n" +
        "MoveSpd = 91290e")]


    public List<Item> items_basic;
    public List<Item> items_artifacts;
    public List<Item> items_barriers;

    /// <summary>
    /// Item ID:s are:
    /// Basic:  0 - 499
    /// Artifact: 500 - 999
    /// Barrier: 1000 -
    /// </summary>
    /// <returns></returns>
    public Item GetItemWithID(int id)
    {
        if (id < 500)
        {
            foreach (Item item in items_basic)
            {
                if (item.itemID == id)
                {
                    return item;
                }
            }
        }
        else if (id < 1000)
        {
            foreach (Item item in items_artifacts)
            {
                if (item.itemID == id)
                {
                    return item;
                }
            }
        }
        else
        {
            foreach (Item item in items_barriers)
            {
                if (item.itemID == id)
                {
                    return item;
                }
            }
        }
        return null;
    }


    public List<Item> GetRandomItems(int count, ItemTypes type, bool noDuplicates)
    {
        List<Item> r = new List<Item>();

        if (noDuplicates)
        {
            switch (type)
            {
                case ItemTypes.BASIC: r = GetUniques(items_basic, count); break;
                case ItemTypes.ARTIFACT: r = GetUniques(items_artifacts, count); break;
                case ItemTypes.BARRIER: r = GetUniques(items_barriers, count); break;
                default: break;
            }
        }
        else
        {
            switch (type)
            {
                case ItemTypes.BASIC: r = GetRandoms(items_basic, count); break;
                case ItemTypes.ARTIFACT: r = GetRandoms(items_artifacts, count); break;
                case ItemTypes.BARRIER: r = GetRandoms(items_barriers, count); break;
                default: break;
            }
        }
        return r;
    }

    private List<Item> GetUniques(List<Item> itemList, int count)
    {
        List<Item> r = new List<Item>();
        var indexes = GameManager.Instance.GenerateRandomUniqueIntegers(new Vector2Int(count, count), new Vector2Int(0, itemList.Count));
        foreach (var index in indexes)
        {
            r.Add(itemList[index]);
        }
        return r;
    }

    private List<Item> GetRandoms(List<Item> itemList, int count)
    {
        var r = new List<Item>();
        for (var i = 0; i < count; i++)
        {
            r.Add(itemList[Random.Range(0, itemList.Count)]);
        }
        return r;
    }
}


[System.Serializable]
public class Item {
    public string name;
    public ItemTypes type;
    public ItemUniqueEffects effects;
    [TextArea(5, 50)]
    public string description;
    public int price;
    public Sprite icon;
    public StartingStats itemStats;
    public int itemID;
    public Item(string name, ItemTypes type, ItemUniqueEffects effects, string description, int price, Sprite icon, StartingStats itemStats, int itemID)
    {
        this.name = name;
        this.type = type;
        this.effects = effects;
        this.description = description;
        this.price = price;
        this.icon = icon;
        this.itemStats = itemStats;
        this.itemID = itemID;
    }
}