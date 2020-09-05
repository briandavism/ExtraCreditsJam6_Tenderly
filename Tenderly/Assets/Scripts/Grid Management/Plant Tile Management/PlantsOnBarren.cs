using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsOnBarren : MonoBehaviour
{

    public List<Tile> plantsOnBarren;
    public Dictionary<string, int> plantsOnBarrenSpawnChances;
    public int totalSpawnChance;

    private void Awake()
    {
        // Calculate totalSpawnCost
        totalSpawnChance = 0;
        foreach (KeyValuePair<string, int> kvp in plantsOnBarrenSpawnChances)
        {
            totalSpawnChance += kvp.Value;
        }
    }
}
