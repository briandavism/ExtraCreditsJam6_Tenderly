using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsOnMarsh : MonoBehaviour
{

    public Tile[] plantsOnMarsh;
    public int[] plantsOnMarshSpawnChances;
    public int totalSpawnChance;

    private void Awake()
    {
        // Calculate totalSpawnCost
        totalSpawnChance = 0;
        foreach (int spawnChance in plantsOnMarshSpawnChances)
        {
            totalSpawnChance += spawnChance;
        }
    }
}
