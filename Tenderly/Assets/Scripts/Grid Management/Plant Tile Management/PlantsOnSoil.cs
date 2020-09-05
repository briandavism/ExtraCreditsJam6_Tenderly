using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsOnSoil : MonoBehaviour
{

    public List<Tile> plantsOnSoil;
    public Dictionary<string, int> plantsOnSoilSpawnChances;
    public int totalSpawnChance;

    private void Awake()
    {
        // Calculate totalSpawnCost
        totalSpawnChance = 0;
        foreach (KeyValuePair<string, int> kvp in plantsOnSoilSpawnChances)
        {
            totalSpawnChance += kvp.Value;
        }
    }
}
