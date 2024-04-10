using System;
using UnityEngine;

[Serializable]
public class AOEGrid
{
    [Header("Only effects the name in this list:")]
    public string name;
    [Space(5)]
    [Header("Make the shapeName -enum inside AOELibrary.cs, at the top:")]
    public AOEShapes shapeName;

    [Space(10)]
    [TextArea(5, 50)]
    public string EffectedSquaresNorth;
    [TextArea(5, 50)]
    public string PushDirectionsNorth;
    [Space(50)]
    [TextArea(5, 50)]
    public string EffectedSquaresNorthEast;
    [TextArea(5, 50)]
    public string PushDirectionsNorthEast;
}