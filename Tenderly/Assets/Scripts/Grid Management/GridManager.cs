using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;


public class GridManager : MonoBehaviour
{
    // Public variables
    public Tilemap groundTiles;
    public Tilemap plantTiles;
    // Loaded Game Data
    public GameObject dataLoader;
    public Dictionary<string, Tile> groundTileFromName = new Dictionary<string, Tile>();
    public Dictionary<string, Tile> plantTileFromName = new Dictionary<string, Tile>();
    public List<Terrain> terrains;
    public List<Plant> plants;
    public List<Vector3Int> allTilePositions;

    // For Plant Merging
    public float mergeDelay = 5.0f;
    public float spawnChance = 0.005f;
    // For Water Placement and Removal
    public GroundTileManager groundTileManager;
    public PlantTileManager  plantTileManager;


    private void Awake()
    {
        allTilePositions = new List<Vector3Int>();
        foreach (var pos in groundTiles.cellBounds.allPositionsWithin)
        {
            if (groundTiles.HasTile(pos))
            {
                allTilePositions.Add(pos);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        groundTileFromName = dataLoader.GetComponent<LoadGameData>().groundTileFromName;
        plantTileFromName = dataLoader.GetComponent<LoadGameData>().plantTileFromName;
        terrains = dataLoader.GetComponent<LoadGameData>().terrains;
        plants = dataLoader.GetComponent<LoadGameData>().plants;
    }

    
    // List shuffler: Given a list, shuffle it.
    public List<T> ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;

        for (int i = list.Count - 1; i > 1; i--)
        {
            int rnd = random.Next(i + 1);
            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }

        return list;
    }


    // Get Tile: Givent a Vector3Int, return the tile or tiles that are beneath it.
    public List<Tile> GetTiles(Vector3Int mousePos)
    {
        List<Tile> results = new List<Tile>();

        // There should always be a ground tile. TODO: Error checking. Possibly use bounds to validate?
        if (groundTiles.HasTile(mousePos))
        {
            Tile tile = groundTiles.GetTile<Tile>(mousePos);
            results.Add(tile);
        }

        if (plantTiles.HasTile(mousePos))
        {
            Tile tile = plantTiles.GetTile<Tile>(mousePos);
            results.Add(tile);
        }

        return results;
    }


    /*************************************: GAME MECHANICS :********************************************/
    /* Water:
     *  - In the beginning, all is barren.
     *  - When water is placed, find all tiles within a certain radius and enque them.
     *      - Potentially, we can queue the nearest neighbors and only enque more neighbors when a 
     *          tile becomes marsh.
     *  - 
     */
    // Place Water: Call to send moisture out from a given Vector3Int location. Input assumes Oddr coords.
    public bool PlaceWater(Vector3Int tileLocation)
    {
        // Only bother placing water if there isn't water already there.
        if (!groundTiles.GetTile<Tile>(tileLocation).name.Equals("Water", System.StringComparison.Ordinal))
        {
            groundTileManager.PlaceWater(tileLocation);

            // TODO: Also, double check that we didn't destroy plants on the tile we just watered.
            //  if (...) {ClearPlants(thisLocation)}
            return true;
        }
        else
        {
            // DO SOMETHING: When you place water into water, maybe an over flowing sound?
            //  Maybe a nearby tile floods instead?
            return false;
        }
    }
    

    // Remove Water: Should remove the tile and auto-adjust wetness level of surrounding ground tiles.
    public bool RemoveWater(Vector3Int tileLocation)
    {
        // Don't bother removing water if there isn't water already there.
        if (groundTiles.GetTile<Tile>(tileLocation).name.Equals("Water", System.StringComparison.Ordinal))
        {
            groundTileManager.RemoveWater(tileLocation);
            return true;
        }
        else
        {
            // DO SOMETHING: When you try to remove water where there is none, maybe a dusty breeze?
            return false;
        }
    }


    /* Merge:
     *  - Given a tile position, attempt to use it to merge into a new plant nearby.
     */
    public void CheckForMerge(Vector3Int tilePosition)
    {
        plantTileManager.CheckForMerge(tilePosition);
    }


    /* Clear Plants:
     * - Clear the tile under the cursor of plants, if any.
     */
    public void ClearPlants(Vector3Int tilePosition)
    {
        plantTileManager.ClearPlants(tilePosition);
    }

}
