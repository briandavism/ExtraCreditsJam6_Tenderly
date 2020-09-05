using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTilePalette : MonoBehaviour
{
    // An array of the arrays of plant tiles that spawn in each of the 4 biomes.
    public Dictionary<string, Tile> plantTilePalette;
    public PlantsOnWater plantsOnWater;
    public PlantsOnMarsh plantsOnMarsh;
    public PlantsOnSoil plantsOnSoil;
    public PlantsOnBarren plantsOnBarren;


    private void Start()
    {
        plantTilePalette = new Dictionary<string, Tile>();
        PopulatePallete(plantsOnWater.plantsOnWater);
        PopulatePallete(plantsOnMarsh.plantsOnMarsh);
        PopulatePallete(plantsOnSoil.plantsOnSoil);
        PopulatePallete(plantsOnBarren.plantsOnBarren);
    }

    // Adds all entries in a list of tiles keyed on their name
    private void PopulatePallete(List<Tile> listOfPlants)
    {
        foreach(Tile plantTile in plantsOnWater.plantsOnWater)
        {
            if (!plantTilePalette.ContainsKey(plantTile.name))
            {
                plantTilePalette.Add(plantTile.name, plantTile);
            }
            else
            {
                plantTilePalette[plantTile.name] = plantTile;
            }
        }
    }
}
