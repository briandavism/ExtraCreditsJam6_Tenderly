using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsOnBarren : MonoBehaviour
{

    public Tile[] plantsOnBarren;
    public int[] plantsOnBarrenSpawnChances;
    public int totalSpawnChance;

    private void Awake()
    {
        // Calculate totalSpawnCost
        totalSpawnChance = 0;
        foreach (int spawnChance in plantsOnBarrenSpawnChances)
        {
            totalSpawnChance += spawnChance;
        }
    }
}
