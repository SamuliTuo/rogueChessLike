using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TileGraphics : MonoBehaviour
{
    [Header("Only one element type in list per type please! :)")]
    public List<TileType> tileTypes = new List<TileType>();

    [System.Serializable]
    public class TileType
    {
        public NodeType type;
        public List<GameObject> tilePrefabs = new List<GameObject>();
    }


    public GameObject GetTileObject(string layer, int variation)
    {
        switch (layer)
        {
            case "Tile": return TryToFindObject(NodeType.NONE, variation);
            case "Swamp": return TryToFindObject(NodeType.SWAMP, variation);
            case "Empty": return TryToFindObject(NodeType.HOLE, variation);
            case "Thorns": return TryToFindObject(NodeType.THORNS, variation);
            case "Road": return TryToFindObject(NodeType.ROAD, variation);
            case "Vines": return TryToFindObject(NodeType.VINES, variation);
            case "Wall": return TryToFindObject(NodeType.WALL, variation);
            case "Water": return TryToFindObject(NodeType.WATER, variation);
            case "Grass_purple": return TryToFindObject(NodeType.GRASS_PURPLE, variation);
            default: return null;
        }
    }
    
    public GameObject TryToFindObject(NodeType tileType, int var)
    {
        foreach (TileType t in tileTypes)
        {
            if (t.type == tileType)
            {
                if (var < t.tilePrefabs.Count)
                {
                    return t.tilePrefabs[var];
                }   
                else
                {
                    return t.tilePrefabs[0];
                }
            }
        }
        print("Didn't find tile object, returning null");
        return null;
    }

    public int GetTiletypeVariationsCount(NodeType type)
    {
        foreach (TileType t in tileTypes)
        {
            if (t.type == type)
            {
                return (int)t.tilePrefabs.Count;
            }
        }
        print("Didn't find tiles of type: " + type.ToString());
        return 0;
    }
}
