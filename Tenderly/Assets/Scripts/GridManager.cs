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

    // Private variables
    private Vector3Int[] cubeDirections = {new Vector3Int(0, +1, -1),  // NW
                                            new Vector3Int(+1, 0, -1),  // NE
                                            new Vector3Int(+1, -1, 0),  // E
                                            new Vector3Int(0, -1, +1),  // SE
                                            new Vector3Int(-1, 0, +1),  // SW
                                            new Vector3Int(-1, +1, 0)}; // W
                                            
                                            
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


    /* Grid:
     *  - Fundamentally, there is a hexagonal grid of tiles.
     *  - Each tile can be some kind of ground type.
     *      - Current assumption is 4 types: Water, Marsh, Soil, Barren.
     *  - Unity uses Odd Offset coordinates, while easy algorithms use cubic.
     */
    /*************************************: CUBE AND ODDR FUNCTIONS :********************************************/
    // Cube coords to odd pointy top offset coords.
    // Taken from https://www.redblobgames.com/grids/hexagons/#conversions-offset
    public Vector3Int CubeToOddr(Vector3Int cube)
    {
        int col = cube.x + (cube.y - (cube.y & 1)) / 2;
        int row = cube.y;
        return new Vector3Int(col, row, 0);
    }

    // Odd pointy top offset coords to cube coords.
    // Taken from https://www.redblobgames.com/grids/hexagons/#conversions-offset
    public Vector3Int OddrToCube(Vector3Int oddr)
    {
        int x = oddr.x - (oddr.y - (oddr.y & 1)) / 2;
        int y = oddr.y;
        int z = -x - y;
        return new Vector3Int(x, y, z);
    }
    
    // Cube directions from a simple int representation of the 6 directions.
    // NW is 0, NE is 1, so on clockwise until W is 5.
    public Vector3Int CubeDirection(int direction)
    {
        return cubeDirections[direction];
    }

    // Cube neighbors finds the neighbor in a direction from a given Vector3int cube coord.
    // NW is 0, NE is 1, so on clockwise until W is 5.
    public Vector3Int CubeNeighbor(Vector3Int cube, int direction)
    {
        return cube + CubeDirection(direction);
    }

    // To find the oddr offset neighbor, just covert to cube and use cubic function.
    public Vector3Int OddrNeighbor(Vector3Int oddr, int direction)
    {
        return CubeToOddr(CubeNeighbor(OddrToCube(oddr), direction));
    }

    // Cube distance: Given two cubes, a and b,find distance, return int.
    public int CubeDistance(Vector3Int cubeA, Vector3Int cubeB)
    {
        return Mathf.Max(Mathf.Abs(cubeA.x - cubeB.x), Mathf.Abs(cubeA.y - cubeB.y), Mathf.Abs(cubeA.z - cubeB.z));
    }

    // Oddr distance: Given two hexes, a and b, convert from oddr to cube, find distance, return int.
    public int OddrDistance(Vector3Int oddrA, Vector3Int oddrB)
    {
        Vector3Int cubeA = OddrToCube(oddrA);
        Vector3Int cubeB = OddrToCube(oddrB);

        return CubeDistance(cubeA, cubeB);
    }

    // Cuve Range: Given a Cube coord and a distance, return a list of tiles that fall within the range.
    public List<Vector3Int> CubeRange(Vector3Int cube, int distance)
    {
        // List of tiles in Cube coords, not oddr coords.
        List<Vector3Int> results = new List<Vector3Int>();

        // From the center x, search from d distance in the negative and positive directions from x.
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = Mathf.Max(-distance, -x - distance); y <= Mathf.Min(distance, -x + distance); y++)
            {
                int z = -x - y;
                results.Add(cube + new Vector3Int(x, y, z));
            }
        }

        return results;
    }

    // Oddr Range: Given an Oddr coord and a distance, return a list of tiles that fall within the range.
    public List<Vector3Int> OddrRange(Vector3Int oddr, int distance)
    {
        // List of tiles in Oddr coords.
        List<Vector3Int> results = new List<Vector3Int>();
        Vector3Int cube = OddrToCube(oddr);

        // From the center x, search from d distance in the negative and positive directions from x.
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = Mathf.Max(-distance, -x - distance); y <= Mathf.Min(distance, -x + distance); y++)
            {
                int z = -x - y;
                results.Add(CubeToOddr(cube + new Vector3Int(x, y, z)));
            }
        }

        return results;

        // DEBUG MATH
        // Distance = 5, x = -4 for this part of the loop.
        // What is the max of (-5 and --4 - 5)? -1.
        // What is the min of (5 and --4 +5) 5?
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
        List<Vector3Int> tileVectors = OddrRange(waterSource, 5);
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
        tileVectors = OddrRange(waterSource, 2);
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
            List<Vector3Int> tileVectors = OddrRange(spawnOrigin, spawnRadius);
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
                                if ((Random.Range(0f, 1f)) < 0.05f)
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
                                // dbug
                                Debug.Log("Soil options are: " + tile.name);
                                if ((Random.Range(0f, 1f)) < 0.05f)
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
                                if ((Random.Range(0f, 1f)) < 0.05f)
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
                                if ((Random.Range(0f, 1f)) < 0.05f)
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
     *  - For each tile, add a random 4 digit decimal from 0 to 9999 to the Tier number of the object.
     *  - Sort the objects from lowest to highest.
     *  - For each object in this list, check to see if something happens.
     *      - Tier 0: Empty tile spawn new objects
     *          - Water: Lilypad
     *          - Marsh: Soft Rush
     *          - Soil: Fiddlehead
     *      - Tier 1:
     *          - Water: Lilypad * 3 = Water Lily
     *          - Marsh: Soft Rush * 3 = Bullrush
     *          - Soil: Fiddlehead * 3 = Bracken
     *  - When something happens, 
     *  
     *  
     *  - For each tier, keep a queue of tiles belonging to that tier.
     *  - For each tier, randomly iterate over each tile in that tier queue to check for an event. 
     *      - 
     *  - When a tile leaves a tier due to an event, add it to the appropriate tier's list and remove
     *      it from its previous tier.
     *      
     *  - Am I part of a recipie?
     *      - Create an empty list of completed recipies.
     *      - Create an empty list of partial recipies.
     *      - Populate a list of initial recipies that require me.
     *      - Enque my neighbors randomly.
     *      - Look at my next neighbor. 
     *          - For every partial recipie, does the neighbor complete it?
     *              - If it does, make a new completed recipe out of the partial recipie's tiles and
     *                  this new neighbor tile. Don't remove the partial recipie though.
     *          - For every initial recipe, does the neighbor also help fulfill it?
     *              - If it does, make a new partial recipe out of the neighbor and my tiles.
     *                  Don't remove the initial recipe though.
     *              - If it doesn't, this neighbor can't be used to make anything with me. Move on.
     *      - When no neighbors remain, were there completed recipies?
     *          - If yes, pick one of the completed recipies, at random or by some other method.
     *              Then pick one of the tiles at random to become the new tile, based on the recipe.
     *          - If not, this tile isn't the center of a recipe.
     */
    public void CheckForMerge(Vector3Int tilePosition)
    {
        // What tile am I?
        Tile tile = plantTiles.GetTile<Tile>(tilePosition);

        // Create empty list of triples that we can add all valid recipes into.
        // Also create a list of the desired upgrade that cooresponds 1 to 1 indexwise with completed recipes.
        List<List<Tile>> completedRecipes = new List<List<Tile>>();
        List<Tile> potentialMerges = new List<Tile>();

        // Who are my neighboring triples?
        List<List<Tile>> tileTriples = GetClusters(plantTiles, tilePosition);
        tileTriples.AddRange(GetTendrils(plantTiles, tilePosition));

        // For every triple, check if it forms a valid recipe and add to compleedRecipes list.
        foreach (List<Tile> triple in tileTriples)
        {
            // ValidRecipe should return a Tile if this triple is a valid recipe, or a null otherwise.
            Tile mergeIntoTile = MergeRecipies.ValidRecipe(triple);

            if (mergeIntoTile != null)
            {
                completedRecipes.Add(triple);
                potentialMerges.Add(mergeIntoTile);
            }
        }

        // Now that we have every valid recipe, we just need to pick one from among them and complete the merge.
        // TODO: Choose a List<Tile> of three tiles from among completedRecipes by some method.

        for (int i = 0; i < completedRecipes.Count; i++)

        {

        }
        // TODO: Choose one of the tiles in the triple to upgrade. ClearPlants() on the other two and
        //  plantTiles.SetTile() this tile into the desired upgrade.
    }


    // Get Clusters: There are 15 unique clusters that form around a central tile.
    // This method makes a list of them and returns them.
    private List<List<Tile>> GetClusters(Tilemap tilemap, Vector3Int centerTilePosition)
    {
        List<List<Tile>> tileClusters = new List<List<Tile>>();

        // The outer loop represents the 6 possible tiles that surround the central tile.
        for (int i = 0; i < 6; i++)
        {
            // Add the center tile since it will be part of every cluster.
            Tile centerTile = tilemap.GetTile<Tile>(centerTilePosition);
            List<Tile> cluster = new List<Tile>();
            cluster.Add(centerTile);

            // Add the first outer loop tile of this cluster, based on its position relative to center.
            // That is, centerTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
            Vector3Int targetCubePosition = cubeDirections[i] + OddrToCube(centerTilePosition);
            Vector3Int targetOddrPosition = CubeToOddr(targetCubePosition);
            Tile targetTile = tilemap.GetTile<Tile>(targetOddrPosition);
            cluster.Add(targetTile);

            // The inner loop goes through each unique third tile addition to the group.
            for (int j = i+1; j < 6; j++)
            {
                // Add the inner loop tile of this cluster, based on its position relative to center.
                // That is, centerTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
                targetCubePosition = cubeDirections[j] + OddrToCube(centerTilePosition);
                targetOddrPosition = CubeToOddr(targetCubePosition);
                targetTile = tilemap.GetTile<Tile>(targetOddrPosition);
                cluster.Add(targetTile);

                // Now add this cluster of three tiles to the tileClusters list.
                tileClusters.Add(cluster);
            }
        }

        return tileClusters;
    }

    // Get Tendrils: There are 18 unique tendrils that form using a given tile as a base, not including the
    // tendrils that are also considered clusters.
    private List<List<Tile>> GetTendrils(Tilemap tilemap, Vector3Int baseTilePosition)
    {
        List<List<Tile>> tileTendrils = new List<List<Tile>>();

        // The for loop represents the 6 possible tiles that surround the base tile.
        for (int i = 0; i < 6; i++)
        {
            // Add the center tile since it will be part of every tendril.
            Tile baseTile = tilemap.GetTile<Tile>(baseTilePosition);
            List<Tile> tendril = new List<Tile>();
            tendril.Add(baseTile);

            // Add this loop's tile to this tednril, based on its position relative to the base.
            // That is, baseTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
            Vector3Int targetCubePosition = cubeDirections[i] + OddrToCube(baseTilePosition);
            Vector3Int targetOddrPosition = CubeToOddr(targetCubePosition);
            Tile targetTile = tilemap.GetTile<Tile>(targetOddrPosition);
            tendril.Add(targetTile);

            // The third tile slightly is more complicated. It can be in any three directions based on the initial
            // direction, deviating by -1, 0, and +1, and making sure to wrap from -1 to 5.
            for (int d = -1; d <=1; d++)
            {
                int newDirection = ((i + d) % 6 + 6) % 6;
                targetCubePosition = cubeDirections[newDirection] + OddrToCube(baseTilePosition);
                targetOddrPosition = CubeToOddr(targetCubePosition);
                targetTile = tilemap.GetTile<Tile>(targetOddrPosition);
                tendril.Add(targetTile);

                // Now add this tendril of three tiles to the tileTendrils list.
                tileTendrils.Add(tendril);
            }
        }

        return tileTendrils;
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
