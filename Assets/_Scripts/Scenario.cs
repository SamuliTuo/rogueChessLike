using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewScenario", order = 3)]
public class Scenario : ScriptableObject
{
    [System.Serializable]
    public class Enemy
    {
        public GameObject unit;
        public Vector2Int spawnPos;
    }
    public List<Enemy> enemies = new List<Enemy>();
}
