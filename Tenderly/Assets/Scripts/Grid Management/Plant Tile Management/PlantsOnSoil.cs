using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsOnSoil : MonoBehaviour
{

    public Tile[] plantsOnSoil;
    public int[] plantsOnSoilSpawnChances;
    public int totalSpawnChance;

    private void Awake()
    {
        // Calculate totalSpawnCost
        totalSpawnChance = 0;
        foreach (int spawnChance in plantsOnSoilSpawnChances)
        {
            totalSpawnChance += spawnChance;
        }
    }
}
