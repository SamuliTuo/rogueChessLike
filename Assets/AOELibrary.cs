using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public enum AOEShapes
{
    ROW_WIDE_1X3, ROW_WIDE_1X5, ROW_LONG_3X1, BOX_3X3, CONE_1_3_3
}

[Serializable]
public enum CompassDir { NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORTH_WEST, NONE }


public class AOELibrary : MonoBehaviour
{
    [Header(
    "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n" +
    "MAKE THE GRIDS ALWAYS SQUARES\n" +
    "WITH THE x or X IN THE MIDDLE!\n" +
    "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n\n\n" +
    "Separate elements with ,\n\n" +
    "-  [no effect] \n" +
    "1  [effect here] \n" +
    //"2  [aoe upgrade lvl 1]" +
    //"3  [aoe upgrade lvl 2]" +
    "X (BIG)  [middle square, effect]. \n" +
    "x (small) [middle square, no effect] \n\n" +

    "Always make push directions too, player might upgrade ability to push\n\n" +
    "- = no push \n" +
    "Big letters = push \n" +
    "Small letters = chip \n" +
    "x or X = chip up without moving\n" +
    "N = north \n" +
    "E = east \n" +
    "   (N, E, S, W, NE, SE, SW, NW)\n" +
    "number AFTER direction = extra square(s) of push/chip \n\n\n" +

    "Example: \n\n" +

    "1,1,1\n" +
    "1,x,1\n" +
    "-,-,-\n\n" +

    "NW,N1,NE\n" +
    "W,-,E\n" +
    "-,-,-\n")]

    public List<AOEGrid> AOEs = new List<AOEGrid>();
    [SerializeField]
    public AOEGridScriptable[] AOELibraryEntries;


    private void Start()
    {
        AOELibraryEntries = Resources.LoadAll<AOEGridScriptable>("AOEGrids");
    }


    // Called from inspectors up-right corner 3-dots: (aka. context menu (apparently lol))
    //[ContextMenu("Import grids")]
    public void RefreshAOELibrary()
    {
        for (int i = 0; i < AOEs.Count; i++)
        {
            ImportGrid(AOEs[i]);
        }
    }



    public void ImportGrid(AOEGrid grid)
    {
        Vector2Int middlePoint = Vector2Int.zero;

        var hitSquaresN = grid.EffectedSquaresNorth;
        var pushSquaresN = grid.PushDirectionsNorth;
        var hitSquaresNE = grid.EffectedSquaresNorthEast;
        var pushSquaresNE = grid.PushDirectionsNorthEast;

        string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

        TextAsset data = new TextAsset(hitSquaresN);
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        List<string[]> finalLines = new List<string[]>();
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            finalLines.Add(values);
            for (int j = 0; j < values.Length; j++)
            {
                if (values[j].Contains("x") || values[j].Contains("X"))
                {
                    middlePoint = new(j, i);
                }
            }
        }
        // Push data
        TextAsset data2 = new TextAsset(pushSquaresN);
        var lines2 = Regex.Split(data2.text, LINE_SPLIT_RE);
        List<string[]> finalLines2 = new List<string[]>();
        for (int i = lines2.Length - 1; i >= 0; i--)
        {
            var values = Regex.Split(lines2[i], SPLIT_RE);
            finalLines2.Add(values);
        }


        // Damage rings:
        //  - Rings are made from biggest to smalles.
        //  - A ring is made starting from north and going clockwise.
        //  - Last ring is just the middle-point alone.
        List<List<AOEGridScriptable.NodeInfo>> rings = new List<List<AOEGridScriptable.NodeInfo>>();
        int ringCount = middlePoint.x;
        for (int i = ringCount; i > 0; i--)
        {
            rings.Add(MakeRing(i, finalLines, finalLines2, middlePoint));
        }
        // middle
        if (finalLines[middlePoint.x][middlePoint.y] == "X")
        {
            char[] charArr = finalLines2[middlePoint.x][middlePoint.y].ToCharArray();
            rings.Add(
                new List<AOEGridScriptable.NodeInfo>() {
                    new AOEGridScriptable.NodeInfo(
                        0, 0,
                        char.IsUpper(charArr.First()),
                        !char.IsUpper(charArr.First()),
                        char.IsNumber(charArr.Last()) ? charArr.Last() : 0,
                        CompassDirectionFromString(finalLines2[middlePoint.x][middlePoint.y]))
                    });
        }


        //  D I A G O N A L S :

        // Hit data
        TextAsset data3 = new TextAsset(hitSquaresNE);
        var lines3 = Regex.Split(data3.text, LINE_SPLIT_RE);
        List<string[]> finalLines3 = new List<string[]>();
        for (int i = lines3.Length - 1; i >= 0; i--)
        {
            var values = Regex.Split(lines3[i], SPLIT_RE);
            finalLines3.Add(values);
            for (int j = 0; j < values.Length; j++)
            {
                if (values[j].Contains("x") || values[j].Contains("X"))
                {
                    middlePoint = new(j, i);
                }
            }
        }
        // Push data
        TextAsset data4 = new TextAsset(pushSquaresNE);
        var lines4 = Regex.Split(data4.text, LINE_SPLIT_RE);
        List<string[]> finalLines4 = new List<string[]>();
        for (int i = lines4.Length - 1; i >= 0; i--)
        {
            var values = Regex.Split(lines4[i], SPLIT_RE);
            finalLines4.Add(values);
        }


        // Damage rings:
        //  - Rings are made from biggest to smalles.
        //  - A ring is made starting from north and going clockwise.
        //  - Last ring is just the middle-point alone.
        List<List<AOEGridScriptable.NodeInfo>> rings2 = new List<List<AOEGridScriptable.NodeInfo>>();//new List<AOEGridScriptable.NodeInfo[]>();
        int ringCount2 = middlePoint.x;
        for (int i = ringCount2; i > 0; i--)
        {
            rings2.Add(MakeRing(i, finalLines3, finalLines4, middlePoint));
        }
        // middle
        if (finalLines3[middlePoint.x][middlePoint.y] == "X")
        {
            char[] charArr = finalLines4[middlePoint.x][middlePoint.y].ToCharArray();
            rings2.Add(
                new List<AOEGridScriptable.NodeInfo> {
                    new AOEGridScriptable.NodeInfo(
                        0, 0,
                        char.IsUpper(charArr.First()),
                        !char.IsUpper(charArr.First()),
                        char.IsNumber(charArr.Last()) ? charArr.Last() : 0,
                        CompassDirectionFromString(finalLines4[middlePoint.x][middlePoint.y]))
                    });
        }

        // Make the scriptable object to hold data:
        AOEGridScriptable newGrid = ScriptableObject.CreateInstance<AOEGridScriptable>();
        string path = "Assets/Resources/AOEGrids/" + grid.shapeName + ".asset";
        AssetDatabase.CreateAsset(newGrid, path);
        Undo.RecordObject(newGrid, "asd");

        newGrid.shapeName = grid.shapeName;

        if (rings.Count >= 1) newGrid.rings_east_0 = rings[0];
        if (rings.Count >= 2) newGrid.rings_east_1 = rings[1];
        if (rings.Count >= 3) newGrid.rings_east_2 = rings[2];
        if (rings.Count >= 4) newGrid.rings_east_3 = rings[3];
        if (rings.Count >= 5) newGrid.rings_east_4 = rings[4];
        if (rings.Count >= 6) newGrid.rings_east_5 = rings[5];
        if (rings.Count >= 7) newGrid.rings_east_6 = rings[6];
        if (rings.Count >= 8) newGrid.rings_east_7 = rings[7];

        var ringsEast = RotateRings(rings, 1);
        if (ringsEast.Count >= 1) newGrid.rings_south_0 = ringsEast[0];
        if (ringsEast.Count >= 2) newGrid.rings_south_1 = ringsEast[1];
        if (ringsEast.Count >= 3) newGrid.rings_south_2 = ringsEast[2];
        if (ringsEast.Count >= 4) newGrid.rings_south_3 = ringsEast[3];
        if (ringsEast.Count >= 5) newGrid.rings_south_4 = ringsEast[4];
        if (ringsEast.Count >= 6) newGrid.rings_south_5 = ringsEast[5];
        if (ringsEast.Count >= 7) newGrid.rings_south_6 = ringsEast[6];
        if (ringsEast.Count >= 8) newGrid.rings_south_7 = ringsEast[7];

        var ringsSouth = RotateRings(rings, 2);
        if (ringsSouth.Count >= 1) newGrid.rings_west_0 = ringsSouth[0];
        if (ringsSouth.Count >= 2) newGrid.rings_west_1 = ringsSouth[1];
        if (ringsSouth.Count >= 3) newGrid.rings_west_2 = ringsSouth[2];
        if (ringsSouth.Count >= 4) newGrid.rings_west_3 = ringsSouth[3];
        if (ringsSouth.Count >= 5) newGrid.rings_west_4 = ringsSouth[4];
        if (ringsSouth.Count >= 6) newGrid.rings_west_5 = ringsSouth[5];
        if (ringsSouth.Count >= 7) newGrid.rings_west_6 = ringsSouth[6];
        if (ringsSouth.Count >= 8) newGrid.rings_west_7 = ringsSouth[7];

        var ringsWest = RotateRings(rings, 3);
        if (ringsWest.Count >= 1) newGrid.rings_north_0 = ringsWest[0];
        if (ringsWest.Count >= 2) newGrid.rings_north_1 = ringsWest[1];
        if (ringsWest.Count >= 3) newGrid.rings_north_2 = ringsWest[2];
        if (ringsWest.Count >= 4) newGrid.rings_north_3 = ringsWest[3];
        if (ringsWest.Count >= 5) newGrid.rings_north_4 = ringsWest[4];
        if (ringsWest.Count >= 6) newGrid.rings_north_5 = ringsWest[5];
        if (ringsWest.Count >= 7) newGrid.rings_north_6 = ringsWest[6];
        if (ringsWest.Count >= 8) newGrid.rings_north_7 = ringsWest[7];

        if (rings2.Count >= 1) newGrid.rings_northEast_0 = rings2[0];
        if (rings2.Count >= 2) newGrid.rings_northEast_1 = rings2[1];
        if (rings2.Count >= 3) newGrid.rings_northEast_2 = rings2[2];
        if (rings2.Count >= 4) newGrid.rings_northEast_3 = rings2[3];
        if (rings2.Count >= 5) newGrid.rings_northEast_4 = rings2[4];
        if (rings2.Count >= 6) newGrid.rings_northEast_5 = rings2[5];
        if (rings2.Count >= 7) newGrid.rings_northEast_6 = rings2[6];
        if (rings2.Count >= 8) newGrid.rings_northEast_7 = rings2[7];

        var ringsSouthEast = RotateRings(rings2, 1);
        if (ringsSouthEast.Count >= 1) newGrid.rings_southEast_0 = ringsSouthEast[0];
        if (ringsSouthEast.Count >= 2) newGrid.rings_southEast_1 = ringsSouthEast[1];
        if (ringsSouthEast.Count >= 3) newGrid.rings_southEast_2 = ringsSouthEast[2];
        if (ringsSouthEast.Count >= 4) newGrid.rings_southEast_3 = ringsSouthEast[3];
        if (ringsSouthEast.Count >= 5) newGrid.rings_southEast_4 = ringsSouthEast[4];
        if (ringsSouthEast.Count >= 6) newGrid.rings_southEast_5 = ringsSouthEast[5];
        if (ringsSouthEast.Count >= 7) newGrid.rings_southEast_6 = ringsSouthEast[6];
        if (ringsSouthEast.Count >= 8) newGrid.rings_southEast_7 = ringsSouthEast[7];

        var ringsSouthWest = RotateRings(rings2, 2);
        if (ringsSouthWest.Count >= 1) newGrid.rings_southWest_0 = ringsSouthWest[0];
        if (ringsSouthWest.Count >= 2) newGrid.rings_southWest_1 = ringsSouthWest[1];
        if (ringsSouthWest.Count >= 3) newGrid.rings_southWest_2 = ringsSouthWest[2];
        if (ringsSouthWest.Count >= 4) newGrid.rings_southWest_3 = ringsSouthWest[3];
        if (ringsSouthWest.Count >= 5) newGrid.rings_southWest_4 = ringsSouthWest[4];
        if (ringsSouthWest.Count >= 6) newGrid.rings_southWest_5 = ringsSouthWest[5];
        if (ringsSouthWest.Count >= 7) newGrid.rings_southWest_6 = ringsSouthWest[6];
        if (ringsSouthWest.Count >= 8) newGrid.rings_southWest_7 = ringsSouthWest[7];

        var ringsNorthWest = RotateRings(rings2, 3);
        if (ringsNorthWest.Count >= 1) newGrid.rings_northWest_0 = ringsNorthWest[0];
        if (ringsNorthWest.Count >= 2) newGrid.rings_northWest_1 = ringsNorthWest[1];
        if (ringsNorthWest.Count >= 3) newGrid.rings_northWest_2 = ringsNorthWest[2];
        if (ringsNorthWest.Count >= 4) newGrid.rings_northWest_3 = ringsNorthWest[3];
        if (ringsNorthWest.Count >= 5) newGrid.rings_northWest_4 = ringsNorthWest[4];
        if (ringsNorthWest.Count >= 6) newGrid.rings_northWest_5 = ringsNorthWest[5];
        if (ringsNorthWest.Count >= 7) newGrid.rings_northWest_6 = ringsNorthWest[6];
        if (ringsNorthWest.Count >= 8) newGrid.rings_northWest_7 = ringsNorthWest[7];

        EditorUtility.SetDirty(newGrid);
        AssetDatabase.SaveAssetIfDirty(newGrid);
    }

    List<AOEGridScriptable.NodeInfo> MakeRing(int index, List<string[]> l1, List<string[]> l2, Vector2Int mid)
    {
        List<AOEGridScriptable.NodeInfo> r = new List<AOEGridScriptable.NodeInfo>();
        if (index == 1)
        {
            MakeNode(mid, 0, 1, l1, l2, r);
            MakeNode(mid, 1, 1, l1, l2, r);
            MakeNode(mid, 1, 0, l1, l2, r);
            MakeNode(mid, 1, -1, l1, l2, r);
            MakeNode(mid, 0, -1, l1, l2, r);
            MakeNode(mid, -1, -1, l1, l2, r);
            MakeNode(mid, -1, 0, l1, l2, r);
            MakeNode(mid, -1, 1, l1, l2, r);
        }
        else if (index == 2)
        {
            MakeNode(mid, 0, 2, l1, l2, r);
            MakeNode(mid, 1, 2, l1, l2, r);
            MakeNode(mid, 2, 2, l1, l2, r);
            MakeNode(mid, 2, 1, l1, l2, r);
            MakeNode(mid, 2, 0, l1, l2, r);
            MakeNode(mid, 2, -1, l1, l2, r);
            MakeNode(mid, 2, -2, l1, l2, r);
            MakeNode(mid, 1, -2, l1, l2, r);
            MakeNode(mid, 0, -2, l1, l2, r);
            MakeNode(mid, -1, -2, l1, l2, r);
            MakeNode(mid, -2, -2, l1, l2, r);
            MakeNode(mid, -2, -1, l1, l2, r);
            MakeNode(mid, -2, 0, l1, l2, r);
            MakeNode(mid, -2, 1, l1, l2, r);
            MakeNode(mid, -2, 2, l1, l2, r);
            MakeNode(mid, -1, 2, l1, l2, r);
        }
        else if (index == 3)
        {
            MakeNode(mid, 0, 3, l1, l2, r);
            MakeNode(mid, 1, 3, l1, l2, r);
            MakeNode(mid, 2, 3, l1, l2, r);
            MakeNode(mid, 3, 3, l1, l2, r);
            MakeNode(mid, 3, 2, l1, l2, r);
            MakeNode(mid, 3, 1, l1, l2, r);
            MakeNode(mid, 3, 0, l1, l2, r);
            MakeNode(mid, 3, -1, l1, l2, r);
            MakeNode(mid, 3, -2, l1, l2, r);
            MakeNode(mid, 3, -3, l1, l2, r);
            MakeNode(mid, 2, -3, l1, l2, r);
            MakeNode(mid, 1, -3, l1, l2, r);
            MakeNode(mid, 0, -3, l1, l2, r);
            MakeNode(mid, -1, -3, l1, l2, r);
            MakeNode(mid, -2, -3, l1, l2, r);
            MakeNode(mid, -3, -3, l1, l2, r);
            MakeNode(mid, -3, -2, l1, l2, r);
            MakeNode(mid, -3, -1, l1, l2, r);
            MakeNode(mid, -3, 0, l1, l2, r);
            MakeNode(mid, -3, 1, l1, l2, r);
            MakeNode(mid, -3, 2, l1, l2, r);
            MakeNode(mid, -3, 3, l1, l2, r);
            MakeNode(mid, -2, 3, l1, l2, r);
            MakeNode(mid, -1, 3, l1, l2, r);
        }

        else if (index == 4)
        {
            MakeNode(mid, 0, 4, l1, l2, r);
            MakeNode(mid, 1, 4, l1, l2, r);
            MakeNode(mid, 2, 4, l1, l2, r);
            MakeNode(mid, 3, 4, l1, l2, r);
            MakeNode(mid, 4, 4, l1, l2, r);
            MakeNode(mid, 4, 3, l1, l2, r);
            MakeNode(mid, 4, 2, l1, l2, r);
            MakeNode(mid, 4, 1, l1, l2, r);
            MakeNode(mid, 4, 0, l1, l2, r);
            MakeNode(mid, 4, -1, l1, l2, r);
            MakeNode(mid, 4, -2, l1, l2, r);
            MakeNode(mid, 4, -3, l1, l2, r);
            MakeNode(mid, 4, -4, l1, l2, r);
            MakeNode(mid, 3, -4, l1, l2, r);
            MakeNode(mid, 2, -4, l1, l2, r);
            MakeNode(mid, 1, -4, l1, l2, r);
            MakeNode(mid, 0, -4, l1, l2, r);
            MakeNode(mid, -1, -4, l1, l2, r);
            MakeNode(mid, -2, -4, l1, l2, r);
            MakeNode(mid, -3, -4, l1, l2, r);
            MakeNode(mid, -4, -4, l1, l2, r);
            MakeNode(mid, -4, -3, l1, l2, r);
            MakeNode(mid, -4, -2, l1, l2, r);
            MakeNode(mid, -4, -1, l1, l2, r);
            MakeNode(mid, -4, 0, l1, l2, r);
            MakeNode(mid, -4, 1, l1, l2, r);
            MakeNode(mid, -4, 2, l1, l2, r);
            MakeNode(mid, -4, 3, l1, l2, r);
            MakeNode(mid, -4, 4, l1, l2, r);
            MakeNode(mid, -3, 4, l1, l2, r);
            MakeNode(mid, -2, 4, l1, l2, r);
            MakeNode(mid, -1, 4, l1, l2, r);
        }
        else if (index == 5)
        {
            MakeNode(mid, 0, 5, l1, l2, r);
            MakeNode(mid, 1, 5, l1, l2, r);
            MakeNode(mid, 2, 5, l1, l2, r);
            MakeNode(mid, 3, 5, l1, l2, r);
            MakeNode(mid, 4, 5, l1, l2, r);
            MakeNode(mid, 5, 5, l1, l2, r);
            MakeNode(mid, 5, 4, l1, l2, r);
            MakeNode(mid, 5, 3, l1, l2, r);
            MakeNode(mid, 5, 2, l1, l2, r);
            MakeNode(mid, 5, 1, l1, l2, r);
            MakeNode(mid, 5, 0, l1, l2, r);
            MakeNode(mid, 5, -1, l1, l2, r);
            MakeNode(mid, 5, -2, l1, l2, r);
            MakeNode(mid, 5, -3, l1, l2, r);
            MakeNode(mid, 5, -4, l1, l2, r);
            MakeNode(mid, 5, -5, l1, l2, r);
            MakeNode(mid, 4, -5, l1, l2, r);
            MakeNode(mid, 3, -5, l1, l2, r);
            MakeNode(mid, 2, -5, l1, l2, r);
            MakeNode(mid, 1, -5, l1, l2, r);
            MakeNode(mid, 0, -5, l1, l2, r);
            MakeNode(mid, -1, -5, l1, l2, r);
            MakeNode(mid, -2, -5, l1, l2, r);
            MakeNode(mid, -3, -5, l1, l2, r);
            MakeNode(mid, -4, -5, l1, l2, r);
            MakeNode(mid, -5, -5, l1, l2, r);
            MakeNode(mid, -5, -4, l1, l2, r);
            MakeNode(mid, -5, -3, l1, l2, r);
            MakeNode(mid, -5, -2, l1, l2, r);
            MakeNode(mid, -5, -1, l1, l2, r);
            MakeNode(mid, -5, 0, l1, l2, r);
            MakeNode(mid, -5, 1, l1, l2, r);
            MakeNode(mid, -5, 2, l1, l2, r);
            MakeNode(mid, -5, 3, l1, l2, r);
            MakeNode(mid, -5, 4, l1, l2, r);
            MakeNode(mid, -5, 5, l1, l2, r);
            MakeNode(mid, -4, 5, l1, l2, r);
            MakeNode(mid, -3, 5, l1, l2, r);
            MakeNode(mid, -2, 5, l1, l2, r);
            MakeNode(mid, -1, 5, l1, l2, r);
        }
        else if (index == 6)
        {
            MakeNode(mid, 0, 6, l1, l2, r);
            MakeNode(mid, 1, 6, l1, l2, r);
            MakeNode(mid, 2, 6, l1, l2, r);
            MakeNode(mid, 3, 6, l1, l2, r);
            MakeNode(mid, 4, 6, l1, l2, r);
            MakeNode(mid, 5, 6, l1, l2, r);
            MakeNode(mid, 6, 6, l1, l2, r);
            MakeNode(mid, 6, 5, l1, l2, r);
            MakeNode(mid, 6, 4, l1, l2, r);
            MakeNode(mid, 6, 3, l1, l2, r);
            MakeNode(mid, 6, 2, l1, l2, r);
            MakeNode(mid, 6, 1, l1, l2, r);
            MakeNode(mid, 6, 0, l1, l2, r);
            MakeNode(mid, 6, -1, l1, l2, r);
            MakeNode(mid, 6, -2, l1, l2, r);
            MakeNode(mid, 6, -3, l1, l2, r);
            MakeNode(mid, 6, -4, l1, l2, r);
            MakeNode(mid, 6, -5, l1, l2, r);
            MakeNode(mid, 6, -6, l1, l2, r);
            MakeNode(mid, 5, -6, l1, l2, r);
            MakeNode(mid, 4, -6, l1, l2, r);
            MakeNode(mid, 3, -6, l1, l2, r);
            MakeNode(mid, 2, -6, l1, l2, r);
            MakeNode(mid, 1, -6, l1, l2, r);
            MakeNode(mid, 0, -6, l1, l2, r);
            MakeNode(mid, -1, -6, l1, l2, r);
            MakeNode(mid, -2, -6, l1, l2, r);
            MakeNode(mid, -3, -6, l1, l2, r);
            MakeNode(mid, -4, -6, l1, l2, r);
            MakeNode(mid, -5, -6, l1, l2, r);
            MakeNode(mid, -6, -6, l1, l2, r);
            MakeNode(mid, -6, -5, l1, l2, r);
            MakeNode(mid, -6, -4, l1, l2, r);
            MakeNode(mid, -6, -3, l1, l2, r);
            MakeNode(mid, -6, -2, l1, l2, r);
            MakeNode(mid, -6, -1, l1, l2, r);
            MakeNode(mid, -6, 0, l1, l2, r);
            MakeNode(mid, -6, 1, l1, l2, r);
            MakeNode(mid, -6, 2, l1, l2, r);
            MakeNode(mid, -6, 3, l1, l2, r);
            MakeNode(mid, -6, 4, l1, l2, r);
            MakeNode(mid, -6, 5, l1, l2, r);
            MakeNode(mid, -6, 6, l1, l2, r);
            MakeNode(mid, -5, 6, l1, l2, r);
            MakeNode(mid, -4, 6, l1, l2, r);
            MakeNode(mid, -3, 6, l1, l2, r);
            MakeNode(mid, -2, 6, l1, l2, r);
            MakeNode(mid, -1, 6, l1, l2, r);
        }
        else if (index == 7)
        {
            MakeNode(mid, 0, 7, l1, l2, r);
            MakeNode(mid, 1, 7, l1, l2, r);
            MakeNode(mid, 2, 7, l1, l2, r);
            MakeNode(mid, 3, 7, l1, l2, r);
            MakeNode(mid, 4, 7, l1, l2, r);
            MakeNode(mid, 5, 7, l1, l2, r);
            MakeNode(mid, 6, 7, l1, l2, r);
            MakeNode(mid, 7, 7, l1, l2, r);
            MakeNode(mid, 7, 6, l1, l2, r);
            MakeNode(mid, 7, 5, l1, l2, r);
            MakeNode(mid, 7, 4, l1, l2, r);
            MakeNode(mid, 7, 3, l1, l2, r);
            MakeNode(mid, 7, 2, l1, l2, r);
            MakeNode(mid, 7, 1, l1, l2, r);
            MakeNode(mid, 7, 0, l1, l2, r);
            MakeNode(mid, 7, -1, l1, l2, r);
            MakeNode(mid, 7, -2, l1, l2, r);
            MakeNode(mid, 7, -3, l1, l2, r);
            MakeNode(mid, 7, -4, l1, l2, r);
            MakeNode(mid, 7, -5, l1, l2, r);
            MakeNode(mid, 7, -6, l1, l2, r);
            MakeNode(mid, 7, -7, l1, l2, r);
            MakeNode(mid, 6, -7, l1, l2, r);
            MakeNode(mid, 5, -7, l1, l2, r);
            MakeNode(mid, 4, -7, l1, l2, r);
            MakeNode(mid, 3, -7, l1, l2, r);
            MakeNode(mid, 2, -7, l1, l2, r);
            MakeNode(mid, 1, -7, l1, l2, r);
            MakeNode(mid, 0, -7, l1, l2, r);
            MakeNode(mid, -1, -7, l1, l2, r);
            MakeNode(mid, -2, -7, l1, l2, r);
            MakeNode(mid, -3, -7, l1, l2, r);
            MakeNode(mid, -4, -7, l1, l2, r);
            MakeNode(mid, -5, -7, l1, l2, r);
            MakeNode(mid, -6, -7, l1, l2, r);
            MakeNode(mid, -7, -7, l1, l2, r);
            MakeNode(mid, -7, -6, l1, l2, r);
            MakeNode(mid, -7, -5, l1, l2, r);
            MakeNode(mid, -7, -4, l1, l2, r);
            MakeNode(mid, -7, -3, l1, l2, r);
            MakeNode(mid, -7, -2, l1, l2, r);
            MakeNode(mid, -7, -1, l1, l2, r);
            MakeNode(mid, -7, 0, l1, l2, r);
            MakeNode(mid, -7, 1, l1, l2, r);
            MakeNode(mid, -7, 2, l1, l2, r);
            MakeNode(mid, -7, 3, l1, l2, r);
            MakeNode(mid, -7, 4, l1, l2, r);
            MakeNode(mid, -7, 5, l1, l2, r);
            MakeNode(mid, -7, 6, l1, l2, r);
            MakeNode(mid, -7, 7, l1, l2, r);
            MakeNode(mid, -6, 7, l1, l2, r);
            MakeNode(mid, -5, 7, l1, l2, r);
            MakeNode(mid, -4, 7, l1, l2, r);
            MakeNode(mid, -3, 7, l1, l2, r);
            MakeNode(mid, -2, 7, l1, l2, r);
            MakeNode(mid, -1, 7, l1, l2, r);
        }
        return r;
    }
    void MakeNode(Vector2Int mid, int x, int y, List<string[]> l1, List<string[]> l2, List<AOEGridScriptable.NodeInfo> list)
    {
        if (l1[mid.x + x][mid.y + y] == "-")
        {
            return;
        }
        char[] charArr = l2[mid.x + x][mid.y + y].ToCharArray();
        var node = new AOEGridScriptable.NodeInfo(
            x, y,
            char.IsUpper(charArr.First()),
            !char.IsUpper(charArr.First()),
            char.IsNumber(charArr.Last()) ? charArr.Last() : 0,
            CompassDirectionFromString(l2[mid.x + x][mid.y + y])
            );
        list.Add(node);
    }
    List<List<AOEGridScriptable.NodeInfo>> RotateRings(List<List<AOEGridScriptable.NodeInfo>> rings, int rotations)
    {
        // Make a copy of the list
        List<List<AOEGridScriptable.NodeInfo>> r = new List<List<AOEGridScriptable.NodeInfo>>();
        for (int i = 0; i < rings.Count; i++)
        {
            r.Add(new List<AOEGridScriptable.NodeInfo>());
            for (int j = 0; j < rings[i].Count; j++)
            {
                r[i].Add(HelperUtilities.Clone(rings[i][j]));
            }
        }
        // Rotate the new list
        for (int i = 0; i < rings.Count; i++)
        {
            for (int j = 0; j < rings[i].Count; j++)
            {
                Shift(r[i][j], rotations);
            }
        }
        return r;
    }

    void Shift(AOEGridScriptable.NodeInfo node, int rotations)
    {
        for (int r = 0; r < rotations; r++)
        {
            Vector2Int oldCoord = new(node.x, node.y);
            node.x = oldCoord.y;
            node.y = -oldCoord.x;
            // Turn the compass directions
            node.pushDirection = ShiftCompassDir(node.pushDirection);
        }
    }
    string CompassDirectionFromString(string dir)
    {
        if (dir == "-")
        {
            return "-"; // CompassDir.NONE;
        }
        else if (dir.ToLower().Contains("ne"))
        {
            return "ne";// CompassDir.NORTH_EAST;
        }
        else if (dir.ToLower().Contains("nw"))
        {
            return "nw";// CompassDir.NORT_WEST;
        }
        else if (dir.ToLower().Contains("sw"))
        {
            return "sw";// CompassDir.SOUTH_WEST;
        }
        else if (dir.ToLower().Contains("se"))
        {
            return "se";// CompassDir.SOUTH_EAST;
        }
        else if (dir.ToLower().Contains("n"))
        {
            return "n";// CompassDir.NORTH;
        }
        else if (dir.ToLower().Contains("s"))
        {
            return "s";// CompassDir.SOUTH;
        }
        else if (dir.ToLower().Contains("e"))
        {
            return "e";// CompassDir.EAST;
        }
        else if (dir.ToLower().Contains("w"))
        {
            return "w";// CompassDir.WEST;
        }
        return "-";// CompassDir.NONE;
    }
    string ShiftCompassDir(string dir)
    {
        switch (dir)
        {
            case "-": return "-";
            case "n": return "ne";
            case "ne": return "e";
            case "e": return "se";
            case "se": return "s";
            case "s": return "sw";
            case "sw": return "w";
            case "w": return "nw";
            case "nw": return "n";
            default: return "-";
        }
    }
    public CompassDir GetCompassDirFromString(string dir)
    {
        switch (dir)
        {
            case "n": return CompassDir.NORTH;
            case "ne": return CompassDir.NORTH_EAST;
            case "e": return CompassDir.EAST;
            case "se": return CompassDir.SOUTH_EAST;
            case "s": return CompassDir.SOUTH;
            case "sw": return CompassDir.SOUTH_WEST;
            case "w": return CompassDir.WEST;
            case "nw": return CompassDir.NORTH_WEST;
            default: return CompassDir.NONE;
        }
    }


    public List<List<AOEGridScriptable.NodeInfo>> GetAOEShape(AOEShapes shapeName, CompassDir orientation)
    {
        var r = new List<List<AOEGridScriptable.NodeInfo>>();
        foreach (var entry in AOELibraryEntries)
        {
            if (entry.shapeName == shapeName)
            {
                switch (orientation)
                {
                    case CompassDir.NORTH:
                        if (entry.rings_north_0.Count > 0) r.Add(entry.rings_north_0);
                        if (entry.rings_north_1.Count > 0) r.Add(entry.rings_north_1);
                        if (entry.rings_north_2.Count > 0) r.Add(entry.rings_north_2);
                        if (entry.rings_north_3.Count > 0) r.Add(entry.rings_north_3);
                        if (entry.rings_north_4.Count > 0) r.Add(entry.rings_north_4);
                        if (entry.rings_north_5.Count > 0) r.Add(entry.rings_north_5);
                        if (entry.rings_north_6.Count > 0) r.Add(entry.rings_north_6);
                        if (entry.rings_north_7.Count > 0) r.Add(entry.rings_north_7);
                        break;
                    case CompassDir.NORTH_EAST:
                        if (entry.rings_northEast_0.Count > 0) r.Add(entry.rings_northEast_0);
                        if (entry.rings_northEast_1.Count > 0) r.Add(entry.rings_northEast_1);
                        if (entry.rings_northEast_2.Count > 0) r.Add(entry.rings_northEast_2);
                        if (entry.rings_northEast_3.Count > 0) r.Add(entry.rings_northEast_3);
                        if (entry.rings_northEast_4.Count > 0) r.Add(entry.rings_northEast_4);
                        if (entry.rings_northEast_5.Count > 0) r.Add(entry.rings_northEast_5);
                        if (entry.rings_northEast_6.Count > 0) r.Add(entry.rings_northEast_6);
                        if (entry.rings_northEast_7.Count > 0) r.Add(entry.rings_northEast_7);
                        break;
                    case CompassDir.EAST:
                        if (entry.rings_east_0.Count > 0) r.Add(entry.rings_east_0);
                        if (entry.rings_east_1.Count > 0) r.Add(entry.rings_east_1);
                        if (entry.rings_east_2.Count > 0) r.Add(entry.rings_east_2);
                        if (entry.rings_east_3.Count > 0) r.Add(entry.rings_east_3);
                        if (entry.rings_east_4.Count > 0) r.Add(entry.rings_east_4);
                        if (entry.rings_east_5.Count > 0) r.Add(entry.rings_east_5);
                        if (entry.rings_east_6.Count > 0) r.Add(entry.rings_east_6);
                        if (entry.rings_east_7.Count > 0) r.Add(entry.rings_east_7);
                        break;
                    case CompassDir.SOUTH_EAST:
                        if (entry.rings_southEast_0.Count > 0) r.Add(entry.rings_southEast_0);
                        if (entry.rings_southEast_1.Count > 0) r.Add(entry.rings_southEast_1);
                        if (entry.rings_southEast_2.Count > 0) r.Add(entry.rings_southEast_2);
                        if (entry.rings_southEast_3.Count > 0) r.Add(entry.rings_southEast_3);
                        if (entry.rings_southEast_4.Count > 0) r.Add(entry.rings_southEast_4);
                        if (entry.rings_southEast_5.Count > 0) r.Add(entry.rings_southEast_5);
                        if (entry.rings_southEast_6.Count > 0) r.Add(entry.rings_southEast_6);
                        if (entry.rings_southEast_7.Count > 0) r.Add(entry.rings_southEast_7);
                        break;
                    case CompassDir.SOUTH:
                        if (entry.rings_south_0.Count > 0) r.Add(entry.rings_south_0);
                        if (entry.rings_south_1.Count > 0) r.Add(entry.rings_south_1);
                        if (entry.rings_south_2.Count > 0) r.Add(entry.rings_south_2);
                        if (entry.rings_south_3.Count > 0) r.Add(entry.rings_south_3);
                        if (entry.rings_south_4.Count > 0) r.Add(entry.rings_south_4);
                        if (entry.rings_south_5.Count > 0) r.Add(entry.rings_south_5);
                        if (entry.rings_south_6.Count > 0) r.Add(entry.rings_south_6);
                        if (entry.rings_south_7.Count > 0) r.Add(entry.rings_south_7);
                        break;
                    case CompassDir.SOUTH_WEST:
                        if (entry.rings_southWest_0.Count > 0) r.Add(entry.rings_southWest_0);
                        if (entry.rings_southWest_1.Count > 0) r.Add(entry.rings_southWest_1);
                        if (entry.rings_southWest_2.Count > 0) r.Add(entry.rings_southWest_2);
                        if (entry.rings_southWest_3.Count > 0) r.Add(entry.rings_southWest_3);
                        if (entry.rings_southWest_4.Count > 0) r.Add(entry.rings_southWest_4);
                        if (entry.rings_southWest_5.Count > 0) r.Add(entry.rings_southWest_5);
                        if (entry.rings_southWest_6.Count > 0) r.Add(entry.rings_southWest_6);
                        if (entry.rings_southWest_7.Count > 0) r.Add(entry.rings_southWest_7);
                        break;
                    case CompassDir.WEST:
                        if (entry.rings_west_0.Count > 0) r.Add(entry.rings_west_0);
                        if (entry.rings_west_1.Count > 0) r.Add(entry.rings_west_1);
                        if (entry.rings_west_2.Count > 0) r.Add(entry.rings_west_2);
                        if (entry.rings_west_3.Count > 0) r.Add(entry.rings_west_3);
                        if (entry.rings_west_4.Count > 0) r.Add(entry.rings_west_4);
                        if (entry.rings_west_5.Count > 0) r.Add(entry.rings_west_5);
                        if (entry.rings_west_6.Count > 0) r.Add(entry.rings_west_6);
                        if (entry.rings_west_7.Count > 0) r.Add(entry.rings_west_7);
                        break;
                    case CompassDir.NORTH_WEST:
                        if (entry.rings_northWest_0.Count > 0) r.Add(entry.rings_northWest_0);
                        if (entry.rings_northWest_1.Count > 0) r.Add(entry.rings_northWest_1);
                        if (entry.rings_northWest_2.Count > 0) r.Add(entry.rings_northWest_2);
                        if (entry.rings_northWest_3.Count > 0) r.Add(entry.rings_northWest_3);
                        if (entry.rings_northWest_4.Count > 0) r.Add(entry.rings_northWest_4);
                        if (entry.rings_northWest_5.Count > 0) r.Add(entry.rings_northWest_5);
                        if (entry.rings_northWest_6.Count > 0) r.Add(entry.rings_northWest_6);
                        if (entry.rings_northWest_7.Count > 0) r.Add(entry.rings_northWest_7);
                        break;
                    default: break;
                }
            }
        }
        return r;
    }
}