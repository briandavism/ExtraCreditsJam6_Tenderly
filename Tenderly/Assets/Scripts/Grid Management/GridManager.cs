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
    // For Plant Spawning Coroutine
    public float spawnTimer;
    public int spawnRadius;
    public GroundTilePallete groundTilePalette;
    private Tile[] groundTileArray;
    public List<Tile> barrenPlants;
    public List<Tile> soilPlants;
    public List<Tile> marshPlants;
    public List<Tile> waterPlants;
    public Dictionary<Tile, List<Tile>> plantTilePalette;
    // For Plant Merging
    public float mergeDelay = 5.0f;
    public float spawnChance = 0.005f;
    // For Water Placement and Removal
    public GroundTileManager groundTileManager;

    private void Awake()
    {
        groundTileArray = groundTilePalette.groundTilePallete;
    }

    // Start is called before the first frame update
    void Start()
    {

        // Call dbug.
        Dbug();

        // To start spawning things, call the SpawnPlants method, which will start a coroutine.
        plantTilePalette = new Dictionary<Tile, List<Tile>>();
        plantTilePalette.Add(groundTileArray[3], barrenPlants);
        plantTilePalette.Add(groundTileArray[2], soilPlants);
        plantTilePalette.Add(groundTileArray[1], marshPlants);
        plantTilePalette.Add(groundTileArray[0], waterPlants);
        StartCoroutine(SpawnPlants());
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
        if (groundTiles.GetTile<Tile>(tileLocation).sprite != groundTileArray[0].sprite)
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
        if (groundTiles.GetTile<Tile>(tileLocation).sprite == groundTileArray[0].sprite)
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

    // Spawning:
    // Spawn New Plants:
    IEnumerator SpawnPlants()
    {
        while (true)
        {
            // Spawn plants on some tiles within spawnRadius originating from the mouse.
            Vector3Int spawnOrigin = GetComponentInParent<Grid>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            List<Vector3Int> tileVectors = HexMath.OddrRange(spawnOrigin, spawnRadius);
            List<Tile> tiles = new List<Tile>();
            foreach (Vector3Int tilePosition in tileVectors)
            {
                Tile groundTile = groundTiles.GetTile<Tile>(tilePosition);

                Tile plantTile = plantTiles.GetTile<Tile>(tilePosition);
                tiles.Add(plantTile);

                // If there is a ground tile and the tile has no plants...
                if (groundTile != null && (plantTile is null || plantTile.name == "Empty"))
                {
                    List<Tile> spawnOptions = plantTilePalette[groundTile];
                    float tempSpawnChance = spawnChance;
                    switch (groundTile.name)
                    {
                        case "Barren":
                            // Spawn something from the barren plant list.
                            foreach (Tile tile in spawnOptions)
                            {
                                if ((Random.Range(0f, 1f)) < tempSpawnChance)
                                {
                                    plantTiles.SetTile(tilePosition, tile);
                                    // Be sure to check if this tile can merge!
                                    CheckForMerge(tilePosition);
                                    break;
                                } else
                                {
                                    tempSpawnChance = tempSpawnChance * 0.75f;
                                }
                            }
                            break;
                        case "Soil":
                            // Spawn something from the soil plant list.
                            foreach (Tile tile in spawnOptions)
                            {
                                if ((Random.Range(0f, 1f)) < tempSpawnChance)
                                {
                                    plantTiles.SetTile(tilePosition, tile);
                                    // Be sure to check if this tile can merge!
                                    CheckForMerge(tilePosition);
                                    break;
                                }
                                else
                                {
                                    tempSpawnChance = tempSpawnChance * 0.75f;
                                }
                            }
                            break;
                        case "Marsh":
                            // Spawn something from the marsh plant list.
                            foreach (Tile tile in spawnOptions)
                            {
                                if ((Random.Range(0f, 1f)) < tempSpawnChance)
                                {
                                    plantTiles.SetTile(tilePosition, tile);
                                    // Be sure to check if this tile can merge!
                                    CheckForMerge(tilePosition);
                                    break;
                                }
                                else
                                {
                                    tempSpawnChance = tempSpawnChance * 0.75f;
                                }
                            }
                            break;
                        case "Water":
                            // Spawn something from the water plant list.
                            foreach (Tile tile in spawnOptions)
                            {
                                if ((Random.Range(0f, 1f)) < tempSpawnChance)
                                {
                                    plantTiles.SetTile(tilePosition, tile);
                                    // Be sure to check if this tile can merge!
                                    CheckForMerge(tilePosition);
                                    break;
                                }
                                else
                                {
                                    tempSpawnChance = tempSpawnChance * 0.75f;
                                }
                            }
                            break;
                    }
                } 
            }

            yield return new WaitForSeconds(spawnTimer);
        }
        
    }


    /* Merge:
     *  - Given a tile position, attempt to use it to merge into a new plant nearby.
     */
    public void CheckForMerge(Vector3Int tilePosition)
    {
        return;
        // Degbug
        Debug.Log("Tile at: " + tilePosition + " will merge after " + mergeDelay + "seconds.");

        // Hey MergeManager, given this tile's Vector3Int position, go try to merge it into a valid tile.
        Vector3Int newPlantLocation = MergeManager.AttemptMerge(plantTiles, tilePosition);

        Debug.Log("newplant at: " + newPlantLocation);


        // 
        if (newPlantLocation == null)
        {
            // Since there is a new tile, we want to check it for merges as well, but maybe after a little longer.
            //CheckForMerge(newPlantLocation);
        }

        // If the MergeManager AttemptMerge() was successful, it should return a Vector3Int for the tile it
        //  became (not necessarily the same as this tile's tilePosition. Since we want to wait a little bit 
        //  before checking for merge again, we can start a coroutine to CheckForMerge of the new tile after 
        //  some time has passed.
    }


    /* Clear Plants:
     * - Clear the tile under the cursor of plants, if any.
     */
    public void ClearPlants(Vector3Int tilePosition)
    {
        Tile grountUnderPlant = groundTiles.GetTile<Tile>(tilePosition);
        Tile plantToClear = plantTiles.GetTile<Tile>(tilePosition);
        if (plantToClear != null && grountUnderPlant != null)
        {
            plantTiles.SetTile(tilePosition, plantTilePalette[grountUnderPlant][0]);
        }
    }


    // Is this tile the center of a recipie? If it is, grow it into something!
    void GrowTile()//??? tile)
    {
        
    }


    // Debug and testing misc function.
    private void Dbug()
    {

    }
}
