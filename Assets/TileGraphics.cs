using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TileGraphicsType 
{ 
    NONE, GRASS_BLUE, 
}

public class TileGraphics : MonoBehaviour
{
    public List<TileType> tileTypes = new List<TileType>();

    [System.Serializable]
    public class TileType
    {
        public TileGraphicsType type;
        public List<GameObject> tilePrefabs = new List<GameObject>();
    }
}
