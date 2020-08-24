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
    public List<Tile> groundTilePalette;
    public List<Tile> barrenPlants;
    public List<Tile> soilPlants;
    public List<Tile> marshPlants;
    public List<Tile> waterPlants;
    public Dictionary<Tile, List<Tile>> plantTilePalette;
    // For Plant Merging
    public float mergeDelay = 5.0f;
    public float spawnChance = 0.005f;

    // Private variables
    private Vector3Int[] cubeDirections = HexMath.cubeDirections;
                                            
                                            
    // Start is called before the first frame update
    void Start()
    {
        // Call dbug.
        Dbug();

        // To start spawning things, call the SpawnPlants method, which will start a coroutine.
        plantTilePalette = new Dictionary<Tile, List<Tile>>();
        plantTilePalette.Add(groundTilePalette[0], barrenPlants);
        plantTilePalette.Add(groundTilePalette[1], soilPlants);
        plantTilePalette.Add(groundTilePalette[2], marshPlants);
        plantTilePalette.Add(groundTilePalette[3], waterPlants);
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
    public void PlaceWater(Vector3Int waterSource)
    {
        // First, get a list of tiles within 5 tiles of the water source. Increase their wetness by 1.
        // OR, for now, just make them instantly dirt.
        List<Vector3Int> tileVectors = HexMath.OddrRange(waterSource, 5);
        List<Tile> tiles = new List<Tile>();
        foreach (Vector3Int tilePosition in tileVectors)
        {
            Tile tile = groundTiles.GetTile<Tile>(tilePosition);
            tiles.Add(tile);

            // Upgrade any barren tiles to dirt. TODO: Implement slow dampening.
            if (groundTiles.GetTile<Tile>(tilePosition) == groundTilePalette[0])
            {
                groundTiles.SetTile(tilePosition, groundTilePalette[1]);
            }
        }

        // Now increase the wetness of tiles within 2.
        tileVectors = HexMath.OddrRange(waterSource, 2);
        tiles = new List<Tile>();
        foreach (Vector3Int tilePosition in tileVectors)
        {
            Tile tile = groundTiles.GetTile<Tile>(tilePosition);
            tiles.Add(tile);

            // Upgrade any barren tiles to dirt. TODO: Implement slow dampening.
            if (groundTiles.GetTile<Tile>(tilePosition) == groundTilePalette[0] ||
                groundTiles.GetTile<Tile>(tilePosition) == groundTilePalette[1])
            {
                groundTiles.SetTile(tilePosition, groundTilePalette[2]);
            }
        }

        // Now increase the wetness of the central target tile.
        groundTiles.SetTile(waterSource, groundTilePalette[3]);
    }

    // Remove Water: For now, not easily implementable. 
    // TODO: For every tile, track every water tile within 5 tiles. We can then track how wet a tile should be
    // and if it should become dry or not when water is removed.


    /* Spawning:
     *  - What can spawn?
     *      - Tier 1 (most often) and Tier 2 (less often).
     * Tier 0: 
     *  - Barren:
     *      - Dead Plants
     *  - Soil:
     *      - Dead Plants
     *  - Marsh:
     *      - Dead Plants
     *  - Water:
     *      - Dead Plants
     * Tier 1:
     *  - Barren:
     *      - (Nothing)
     *  - Soil:
     *      - Grass
     *  - Marsh:
     *      - Soft Rush
     *  - Water:
     *      - Lilypad
     */
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
                    switch (groundTile.name)
                    {
                        case "Barren":
                            // Spawn something from the barren plant list.
                            foreach (Tile tile in spawnOptions)
                            {
                                if ((Random.Range(0f, 1f)) < spawnChance)
                                {
                                    plantTiles.SetTile(tilePosition, tile);
                                    // Be sure to check if this tile can merge!
                                    CheckForMerge(tilePosition);
                                    break;
                                }
                            }
                            break;
                        case "Soil":
                            // Spawn something from the soil plant list.
                            foreach (Tile tile in spawnOptions)
                            {
                                if ((Random.Range(0f, 1f)) < spawnChance)
                                {
                                    plantTiles.SetTile(tilePosition, tile);
                                    // Be sure to check if this tile can merge!
                                    CheckForMerge(tilePosition);
                                    break;
                                }
                            }
                            break;
                        case "Marsh":
                            // Spawn something from the marsh plant list.
                            foreach (Tile tile in spawnOptions)
                            {
                                if ((Random.Range(0f, 1f)) < spawnChance)
                                {
                                    plantTiles.SetTile(tilePosition, tile);
                                    // Be sure to check if this tile can merge!
                                    CheckForMerge(tilePosition);
                                    break;
                                }
                            }
                            break;
                        case "Water":
                            // Spawn something from the water plant list.
                            foreach (Tile tile in spawnOptions)
                            {
                                if ((Random.Range(0f, 1f)) < spawnChance)
                                {
                                    plantTiles.SetTile(tilePosition, tile);
                                    // Be sure to check if this tile can merge!
                                    CheckForMerge(tilePosition);
                                    break;
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
        Tile tileToClear = plantTiles.GetTile<Tile>(tilePosition);
        if (tileToClear != null && grountUnderPlant != null)
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
        Vector3Int origin = new Vector3Int(0, 0, 0);
        Vector3Int target = new Vector3Int(2, 2, 0);
        Vector3Int targetCube = new Vector3Int(2, 2, -4);


        // Place Water testing:
        /* 
        Vector3Int tileToMakeWet = new Vector3Int(9, -3, 0);
        PlaceWater(tileToMakeWet);
        * Good, works okay */


        // Get tile testing:
        /*
        target = new Vector3Int(-1, 0, 0);
        Debug.Log("Is there a lily and water tile at (-1,0,0)? ");
        List<Tile> tilesTarget = GetTiles(target);
        for (int i = 0; i < tilesTarget.Count; i++)
        {
            Debug.Log(tilesTarget[i]);
        }

        Debug.Log("What's at origin?");
        List<Tile> tilesOrigin = GetTiles(origin);
        for (int i = 0; i < tilesOrigin.Count; i++)
        {
            Debug.Log(tilesOrigin[i]);
        }
        * Works alright */

        // Neighbor testing: Keep in mind, the range function also captures yourself!
        /*
        List<Vector3Int> cubeNeighbors = CubeRange(origin, 0);
        List<Vector3Int> oddrNeighbors = OddrRange(origin, 0);
        Assert.AreEqual(cubeNeighbors.Count, 1);
        Assert.AreEqual(oddrNeighbors.Count, 1);

        List<Vector3Int> cubeNeighbors2 = CubeRange(targetCube, 1);
        List<Vector3Int> oddrNeighbors2 = OddrRange(target, 2);
        List<Vector3Int> oddrNeighbors3 = OddrRange(target, 3);
        Assert.AreEqual(cubeNeighbors2.Count, 7);
        Assert.AreEqual(oddrNeighbors2.Count, 19);
        Assert.AreEqual(oddrNeighbors3.Count, 37);
        * Done great work */

        // For loop
        /* 
        Debug.Log("Center at: " + origin);
        Debug.Log("Neighbor count: " + cubeNeighbors2.Count);
        foreach (Vector3Int v in cubeNeighbors2)
        {
            Debug.Log("Neighbor at: " + v);
        } 
        * /


        // Distance testing:
        /* 
        Assert.AreEqual(OddrDistance(origin, target), 3);
        Assert.AreEqual(CubeDistance(origin, targetCube), 4);
        * Done, works great.*/

        // Neighbors debug.
        /*
        for (int d = 0; d <= 5; d++)
        {
            target = oddr_offset_neighbor(origin, d);
            Debug.Log("Ground neighbor of target at " + origin + " in " + cube_to_oddr(cube_direction(d)) + ": " 
                + groundTiles.GetTile(target));
            Debug.Log("Plant neighbor of target at " + origin + " in " + cube_to_oddr(cube_direction(d)) + ": " 
                + plantTiles.GetTile(target));

        } 
        * Done, works good. */

        // Cube - Hex conversion testing.
        /*
        Vector3Int Hex = new Vector3Int(2, -2, 0);
        Vector3Int Cube = new Vector3Int(3, -2, -1);
        Debug.Log("Cube to Hex. Does " + Hex + " equal " + cube_to_oddr(Cube) + " ?");
        Debug.Log("Hex to Cube. Does " + Cube + " equal " + oddr_to_cube(Hex) + " ?");
        * Done, works good. */
    }
}
