using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public enum TileGraphicsType { WALKABLE, UN_WALKABLE,}

public class TileGraphics : MonoBehaviour
{
    [Header("Only one element type in list per type please!")]
    public List<TileType> tileTypes = new List<TileType>();
    public List<GameObject> holeObjects = new List<GameObject>();

    [System.Serializable]
    public class TileType
    {
        public string name;
        public NodeType type;
        public List<GameObject> tilePrefabs = new List<GameObject>();
        //public TileGraphicsType walkability = TileGraphicsType.WALKABLE;
        public bool walkable = true;
    }


    public Tuple<GameObject, bool> GetTileObject(string layer, int variation)
    {
        switch (layer)
        {
            case "Tile": return TryToFindObject(NodeType.NONE, variation);
            case "Swamp": return TryToFindObject(NodeType.SWAMP, variation);
            //case "Empty": return TryToFindObject(NodeType.HOLE, variation);
            case "Thorns": return TryToFindObject(NodeType.THORNS, variation);
            case "Road": return TryToFindObject(NodeType.ROAD, variation);
            case "Vines": return TryToFindObject(NodeType.VINES, variation);
            case "Wall": return TryToFindObject(NodeType.WALL, variation);
            case "Water": return TryToFindObject(NodeType.WATER, variation);
            case "Grass_purple": return TryToFindObject(NodeType.GRASS_PURPLE, variation);
            default: return null;
        }
    }
    
    public Tuple<GameObject, bool> TryToFindObject(NodeType tileType, int var)
    {
        foreach (TileType t in tileTypes)
        {
            if (t.type == tileType)
            {
                if (var < t.tilePrefabs.Count)
                {
                    return new Tuple<GameObject,bool>(t.tilePrefabs[var], t.walkable);
                }   
                else
                {
                    return new Tuple<GameObject, bool>(t.tilePrefabs[0], t.walkable);
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





    // H O L E   S T U F F

    // (true means no hole, false means hole...):
    bool[,] holes = null;
    bool left, topleft, up, topright, right, botright, down, botleft;

    public void ResetHoles()
    {
        holes = null;
    }

    public GameObject GetHoleObject(int variation, int rotation)
    {
        return InstantiateHoleObject(variation, rotation);
    }

    public void MakeHoleList(int currX, int currY, int xCount, int yCount, Node[,] nodes)
    {
        holes = new bool[xCount, yCount];
        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                if (currX == x && currY == y)
                {
                    holes[x, y] = false;
                }
                else
                {
                    holes[x, y] = nodes[x, y].tileTypeLayerName != "Empty";
                }
            }
        }
    }


    GameObject InstantiateHoleObject(int index, float rotation = 0)
    {
        var copy = Instantiate(holeObjects[index]);
        copy.transform.GetChild(0).Rotate(0, 0, rotation + 180);
        return copy;
    }



    // Here's a beautifully horrible "flowchard" for your reading pleasure!
    public HoleObjectStats GetCorrectHoleObjectStats(int currX, int currY)
    {
        // in holes -boolean list:
            // false = yes hole (OR out of bounds, counts as hole)
            // true = no hole

        // neighbours are checked in scenario builder before calling this

        if (left)
        {
            if (up)
            {
                if (right)
                {
                    if (down)
                    {
                        return new HoleObjectStats(0, 0);
                    }
                    else
                    {
                        return new HoleObjectStats(1, 0);
                    }
                }
                else
                {
                    if (down)
                    {
                        return new HoleObjectStats(1, 270); // rotate ccw
                    }
                    else
                    {
                        if (botright)
                        {
                            return new HoleObjectStats(5, 0);
                        }
                        else
                        {
                            return new HoleObjectStats(2, 0);
                        }
                    }
                }
            }
            else // left !up
            {
                if (down)
                {
                    if (right)
                    {
                        return new HoleObjectStats(1, 180); // rotate cw cw
                    }
                    else
                    {
                        if (topright)
                        {
                            return new HoleObjectStats(5, 270); //rotate ccw
                        }
                        else
                        {
                            return new HoleObjectStats(2, 270); //rotate ccw
                        }
                    }
                }
                else
                {
                    if (right)
                    {
                        return new HoleObjectStats(6, 0);
                    }
                    else
                    {
                        if (topright && botright)
                        {
                            return new HoleObjectStats(12, 0);
                        }
                        if (topright && !botright)
                        {
                            return new HoleObjectStats(8, 0);
                        }
                        if (!topright && botright)
                        {
                            return new HoleObjectStats(7, 0);
                        }
                        if (!topright && !botright)
                        {
                            return new HoleObjectStats(3, 0);
                        }
                    }
                }
            }
        }

        if (up) // !left
        {
            if (right)
            {
                if (down)
                {
                    return new HoleObjectStats(1, 90); //rotate cw
                }
                else
                {
                    if (botleft)
                    {
                        return new HoleObjectStats(5, 90); //rotate cw
                    }
                    else
                    {
                        return new HoleObjectStats(2, 90); //rotate cw
                    }
                }
            }
            else
            {
                if (down)
                {
                    return new HoleObjectStats(6, 90); //rotate cw
                }
                else
                {
                    if (botleft && botright)
                    {
                        return new HoleObjectStats(12, 90); // rotate cw
                    }
                    if (botleft && !botright)
                    {
                        return new HoleObjectStats(7, 90); //rotate cw
                    }
                    if (!botleft && botright)
                    {
                        return new HoleObjectStats(8, 90); //rotate cw
                    }
                    if (!botleft && !botright)
                    {
                        return new HoleObjectStats(3, 90); // rotate cw
                    }
                }
            }
        }

        if (right) // !left !up
        {
            if (down)
            {
                if (topleft)
                {
                    return new HoleObjectStats(5, 180); //rotate cw cw
                }
                else
                {
                    return new HoleObjectStats(2, 180); // rotate cw cw
                }
            }
            else
            {
                if (topleft && botleft)
                {
                    return new HoleObjectStats(12, 180); // rotate cw cw
                }
                if (topleft && !botleft)
                {
                    return new HoleObjectStats(7, 180); //rotate cw cw
                }
                if (!topleft && botleft)
                {   
                    return new HoleObjectStats(8, 180); // rotate cw cw
                }
                if (!topleft && !botleft)
                {
                    return new HoleObjectStats(3, 180); // rotate cw cw
                }
            }
        }

        if (down) //!left !up !right
        {
            if (topleft && topright)
            {
                return new HoleObjectStats(12, 270); // rotate ccw
            }
            if (topleft && !topright)
            {
                return new HoleObjectStats(8, 270); // rotate ccw
            }
            if (!topleft && topright)
            {
                return new HoleObjectStats(7, 270); // rotate ccw
            }
            if (!topleft && !topright)
            {
                return new HoleObjectStats(3, 270); // rotate ccw
            }
        }
        
        // no cardinal-dir neighbours
        if (topleft)
        {
            if (topright)
            {
                if (botright)
                {
                    if (botleft)
                    {
                        return new HoleObjectStats(14, 0);
                    }
                    else
                    {
                        return new HoleObjectStats(9, 90); // rotate cw
                    }
                }
                else
                {
                    if (botleft)
                    {
                        return new HoleObjectStats(9, 0);
                    }
                    else
                    {
                        return new HoleObjectStats(11, 180); // rotate cw cw
                    }
                }
            }
            else
            {
                if (botleft)
                {
                    if (botright)
                    {
                        return new HoleObjectStats(9, 270); // rotate ccw
                    }
                    else
                    {
                        return new HoleObjectStats(11, 90); // rotate cw
                    }
                }
                if (botright)
                {
                    return new HoleObjectStats(10, 90); // rotate cw
                }
                else
                {
                    return new HoleObjectStats(4, 90); // rotate cw
                }
            }
        }

        if (topright)
        {
            if (botright)
            {
                if (botleft)
                {
                    return new HoleObjectStats(9, 180); // rotate cw cw
                }
                else
                {
                    return new HoleObjectStats(11, 270); // rotate ccw
                }
            }
            else
            {
                if (botleft)
                {
                    return new HoleObjectStats(10, 0);
                }
                else
                {
                    return new HoleObjectStats(4, 180); // rotate cw cw
                }
            }
        }

        if (botright)
        {
            if (botleft)
            {
                return new HoleObjectStats(11, 0);
            }
            else
            {
                return new HoleObjectStats(4, 270); // rotate ccw
            }
        }

        if (botleft)
        {
            return new HoleObjectStats(4, 0);
        }


        return new HoleObjectStats(13, 0);
    }

    public Neighbors CheckNeighbours(int currX, int currY)
    {
        // left
        if (currX - 1 < 0)
        {
            left = false;
            botleft = false;
            topleft = false;
        }
        else
        {
            left = holes[currX - 1, currY];
            if (currY - 1 < 0)
            {
                botleft = false;
            }
            else
            {
                botleft = holes[currX - 1, currY - 1];
            }

            if (currY + 1 >= holes.GetLength(1))
            {
                topleft = false;
            }
            else
            {
                topleft = holes[currX - 1, currY + 1];
            }
        }

        // right
        if (currX + 1 >= holes.GetLength(0))
        {
            right = false;
            topright = false;
            botright = false;
        }
        else
        {
            right = holes[currX + 1, currY];
            if (currY - 1 < 0)
            {
                botright = false;
            }
            else
            {
                botright = holes[currX + 1, currY - 1];
            }

            if (currY + 1 >= holes.GetLength(1))
            {
                topright = false;
            }
            else
            {
                topright = holes[currX + 1, currY + 1];
            }
        }

        // down
        if (currY - 1 < 0)
        {
            down = false;
        }
        else
        {
            down = holes[currX, currY - 1];
        }

        // up
        if (currY + 1 >= holes.GetLength(1))
        {
            up = false;
        }
        else
        {
            up = holes[currX, currY + 1];
        }

        return new Neighbors(topleft, up, topright, right, botright, down, botleft, left);
    }
}

/// <summary>
/// Neighbors come in a list of bools going from topLeft to left clockwise.
/// </summary>
public class Neighbors {
    public List<Tuple<Vector2Int, bool>> neighbors;
    public Neighbors(bool topleft, bool top, bool topright, bool right, bool botright, bool bot, bool botleft, bool left)
    {
        neighbors = new List<Tuple<Vector2Int, bool>>
        {
            new(new(-1,1), topleft),
            new(new(0,1), top),
            new(new(1,1), topright),
            new(new(1,0), right),
            new(new(1,-1), botright),
            new(new(0,-1), bot),
            new(new(-1,-1), botleft),
            new(new(-1,0), left)
        };
    }
}

public class HoleObjectStats {
    public int variation;
    public int rotation;
    
    public HoleObjectStats(int variation, int rotation)
    {
        this.variation = variation;
        this.rotation = rotation;
    }
}
