using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsOnWater : MonoBehaviour
{

    public List<Tile> plantsOnWater;
    public Dictionary<string, int> plantsOnWaterSpawnChances;
    public int totalSpawnChance;

    private void Awake()
    {
        // Calculate totalSpawnCost
        totalSpawnChance = 0;
        foreach (KeyValuePair<string, int> kvp in plantsOnWaterSpawnChances)
        {
            totalSpawnChance += kvp.Value;
        }
    }

}
