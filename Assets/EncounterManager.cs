using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapSettings;

public class EncounterManager : MonoBehaviour
{
    public List<MapFloor> encounters;
    public int currentNodeIndex;

    private void Start()
    {
        // Load the initial scene and set the current node to the starting node
    }

    private void Update()
    {
        // Check if the player has reached a new node and trigger the encounter
    }

    public void TriggerEncounter(MapNodeType encounter)
    {
        
    }

    private void EndEncounter(MapFloor encounter)
    {
        // Handle the logic for ending the encounter, including rewards and penalties
        // Unload the encounter scene and return to the map
    }
}