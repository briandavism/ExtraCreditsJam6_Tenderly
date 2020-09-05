using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsOnMarsh : MonoBehaviour
{

    public List<Tile> plantsOnMarsh;
    public Dictionary<string, int> plantsOnMarshSpawnChances;
    public int totalSpawnChance;

    private void Awake()
    {
        // Calculate totalSpawnCost
        totalSpawnChance = 0;
        foreach (KeyValuePair<string, int> kvp in plantsOnMarshSpawnChances)
        {
            totalSpawnChance += kvp.Value;
        }
    }
}
