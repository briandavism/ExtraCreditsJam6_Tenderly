using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsOnWater : MonoBehaviour
{

    public Tile[] plantsOnWater;
    public int[] plantsOnWaterSpawnChances;
    public int totalSpawnChance;

    private void Awake()
    {
        // Calculate totalSpawnCost
        totalSpawnChance = 0;
        foreach (int spawnChance in plantsOnWaterSpawnChances)
        {
            totalSpawnChance += spawnChance;
        }
    }

}
