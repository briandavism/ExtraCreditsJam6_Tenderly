using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTilePalette : MonoBehaviour
{
    // An array of the arrays of plant tiles that spawn in each of the 4 biomes.
    public Tile[][] plantTilePalette;
    public PlantsOnWater plantsOnWater;
    public PlantsOnMarsh plantsOnMarsh;
    public PlantsOnSoil plantsOnSoil;
    public PlantsOnBarren plantsOnBarren;


    private void Start()
    {

        plantTilePalette = new Tile[4][];
        plantTilePalette[0] = plantsOnWater.plantsOnWater;
        plantTilePalette[1] = plantsOnMarsh.plantsOnMarsh;
        plantTilePalette[2] = plantsOnSoil.plantsOnSoil;
        plantTilePalette[3] = plantsOnBarren.plantsOnBarren;
    }

}
