using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTileManager : MonoBehaviour
{
    public GameObject gameData;
    public static PlantTileManager instance;
    public Tilemap plantTilemap;
    private GridManager gridManager;
    private List<Vector3Int> allTilePositions;

    public Dictionary<Vector3Int, GroundTile> groundTileFromPosition = new Dictionary<Vector3Int, GroundTile>();
    public Dictionary<Vector3Int, PlantTile> plantTileFromPosition = new Dictionary<Vector3Int, PlantTile>();
    public Dictionary<string, Tile> groundTileFromName = new Dictionary<string, Tile>();
    public Dictionary<string, Tile> plantTileFromName = new Dictionary<string, Tile>();
    public Dictionary<string, Terrain> terrainFromName = new Dictionary<string, Terrain>();
    public Dictionary<string, Plant> plantFromName = new Dictionary<string, Plant>();
    public List<Plant> plantList = new List<Plant>();

    // For Plant Spawning Coroutine
    public float spawnTimer;
    public float spawnTimerFudged;
    public float spawnAnimationMinDelay;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        spawnTimerFudged = spawnTimer;
    }


    // Start is called before the first frame update
    void Start()
    {
        gridManager = gameObject.GetComponent<GridManager>();
        allTilePositions = gridManager.allTilePositions;

        LoadGameData loadGameData = gameData.GetComponent<LoadGameData>();
        groundTileFromPosition = gridManager.GetComponent<GroundTileManager>().groundTiles;
        groundTileFromName = loadGameData.groundTileFromName;
        plantTileFromName = loadGameData.plantTileFromName;
        terrainFromName = loadGameData.terrainFromName;
        plantFromName = loadGameData.plantFromName;
        plantList = loadGameData.plants;
        GetPlantTiles();

        // To start spawning things, call the SpawnPlants method, which will start a coroutine.
        StartCoroutine(SpawnPeriodicPlants());
    }


    // Use this for initialization: All tiles on the map should be empty to start.
    private void GetPlantTiles()
    {
        foreach (Vector3Int pos in allTilePositions)
        {
            if (!plantTilemap.HasTile(pos)) continue;

            Tile thisTile = plantTileFromName["Empty"];
            plantTilemap.SetTile(pos, thisTile);

            var tile = ScriptableObject.CreateInstance<PlantTile>();
            tile.GridVector = pos;
            tile.WorldVector = plantTilemap.CellToWorld(pos);
            tile.ThisTile = plantTilemap.GetTile<Tile>(pos);
            tile.TilemapMember = plantTilemap;
            tile.Name = pos.x + "," + pos.y;
            tile.Plant = plantFromName["Empty"];

            plantTileFromPosition.Add(tile.GridVector, tile);
        }
    }


    /* Clear Plants:
    * - Clear the tile under the cursor of plants, if any.
    */
    public void ClearPlants(Vector3Int tilePosition)
    {
        Tile groundUnderPlant = groundTileFromPosition[tilePosition].ThisTile;
        Tile plantToClear = plantTileFromPosition[tilePosition].ThisTile;
        if (plantToClear != null && groundUnderPlant != null)
        {
            // Update the tilemap to display the proper tile.
            plantTilemap.SetTile(tilePosition, plantTileFromName["Empty"]);

            // Now that there's no more plant here, update the plantTileFromPosition dictionary.
            plantTileFromPosition[tilePosition].ThisTile = plantTileFromName["Empty"];

            // Make sure to also update the plant
            plantTileFromPosition[tilePosition].Plant = plantFromName["Empty"];
        }
    }


    // Spawning:
    // Spawn New Plants:
    IEnumerator SpawnPeriodicPlants()
    {
        while (true)
        {
            // Spawn plants on some tile from the list of allTilePositions.
            foreach (Vector3Int tilePosition in allTilePositions)
            {
                PlantTile plantTile = plantTileFromPosition[tilePosition];
                Tile groundTile = gridManager.GetTiles(tilePosition)[0];

                // If there is a ground tile and the tile has no plants...
                if (plantTile is null || plantTile.ThisTile.name.Equals("Empty", System.StringComparison.Ordinal))
                {
                    switch (groundTile.name)
                    {
                        case "Water":
                            // Check if this tile will spawn by rolling under its associated spawnChance
                            if ((Random.Range(0f, 1f)) < terrainFromName["Water"].spawnRate)
                            {
                                // Which tile spawns? Get a random number from 1 to the totalSpawnChance
                                int randomDrop = Random.Range(1, terrainFromName["Water"].totalSpawnChance);

                                // Loop over each plant in the tarrain dropsDict and stop when randomDrop is <= 0
                                foreach (KeyValuePair<string, int> kvp in terrainFromName["Water"].dropsDict)
                                {
                                    randomDrop -= kvp.Value;

                                    if (randomDrop <= 0)
                                    {
                                        // Start a cooroutine to change the tile.
                                        StartCoroutine(DelayedPlantSpawn(tilePosition, plantTileFromName[kvp.Key]));
                                        CheckForMerge(tilePosition);
                                        break;
                                    }
                                }
                            }

                            break;
                        case "Marsh":
                            // Check if this tile will spawn by rolling under its associated spawnChance
                            if ((Random.Range(0f, 1f)) < terrainFromName["Marsh"].spawnRate)
                            {
                                // Which tile spawns? Get a random number from 1 to the totalSpawnChance
                                int randomDrop = Random.Range(1, terrainFromName["Marsh"].totalSpawnChance);

                                // Loop over each plant in the tarrain dropsDict and stop when randomDrop is <= 0
                                foreach (KeyValuePair<string, int> kvp in terrainFromName["Marsh"].dropsDict)
                                {
                                    randomDrop -= kvp.Value;

                                    if (randomDrop <= 0)
                                    {
                                        // Start a cooroutine to change the tile.
                                        StartCoroutine(DelayedPlantSpawn(tilePosition, plantTileFromName[kvp.Key]));
                                        break;
                                    }
                                }
                            }

                            break;
                        case "Soil":
                            // Check if this tile will spawn by rolling under its associated spawnChance
                            if ((Random.Range(0f, 1f)) < terrainFromName["Soil"].spawnRate)
                            {
                                // Which tile spawns? Get a random number from 1 to the totalSpawnChance
                                int randomDrop = Random.Range(1, terrainFromName["Soil"].totalSpawnChance);

                                // Loop over each plant in the tarrain dropsDict and stop when randomDrop is <= 0
                                foreach (KeyValuePair<string, int> kvp in terrainFromName["Soil"].dropsDict)
                                {
                                    randomDrop -= kvp.Value;

                                    if (randomDrop <= 0)
                                    {
                                        // Start a cooroutine to change the tile.
                                        StartCoroutine(DelayedPlantSpawn(tilePosition, plantTileFromName[kvp.Key]));
                                        break;
                                    }
                                }
                            }

                            break;
                        case "Barren":
                            // Check if this tile will spawn by rolling under its associated spawnChance
                            if ((Random.Range(0f, 1f)) < terrainFromName["Barren"].spawnRate)
                            {
                                // Which tile spawns? Get a random number from 1 to the totalSpawnChance
                                int randomDrop = Random.Range(1, terrainFromName["Barren"].totalSpawnChance);

                                // Loop over each plant in the tarrain dropsDict and stop when randomDrop is <= 0
                                foreach (KeyValuePair<string, int> kvp in terrainFromName["Barren"].dropsDict)
                                {
                                    randomDrop -= kvp.Value;

                                    if (randomDrop <= 0)
                                    {
                                        // Start a cooroutine to change the tile.
                                        StartCoroutine(DelayedPlantSpawn(tilePosition, plantTileFromName[kvp.Key]));
                                        break;
                                    }
                                }
                            }

                            break;
                    }
                }
            }

            // Fudge with the spawnTimer ? 
            /*
            int degrees = Mathf.RoundToInt(Time.time) % 360;
            spawnTimerFudged = 11 * Mathf.Cos(7 * degrees) + spawnTimer;
            yield return new WaitForSeconds(spawnTimerFudged);
            */

            // Otherwise, just wait the same time every time.
            yield return new WaitForSeconds(spawnTimer);

        }

    }


    // Delayed Plant Spawn: A plant will spawn on this tile but how long it takes is variable.
    IEnumerator DelayedPlantSpawn(Vector3Int tilePosition, Tile tile)
    {
        float randomSpawn = Random.Range(spawnAnimationMinDelay, spawnTimerFudged);

        yield return new WaitForSeconds(randomSpawn);

        // 50/50 flip the plant over the x axis.
        /*
        if (Random.Range(0, 1) == 0)
        {
            // TODO: Flip the tile's sprite.
        } 
        */

        // We have a winner! Using the plantTilePalette dictionary, lookup the correct tile and plant it.
        // TODO: Add a spawn delay!
        plantTilemap.SetTile(tilePosition, tile);

        // Now that a plant spawned there, update the plantTileFromPosition dictionary.
        plantTileFromPosition[tilePosition].ThisTile = tile;

        // Make sure to also update the plant
        plantTileFromPosition[tilePosition].Plant = plantFromName[tile.name];

        // Be sure to check if this tile can merge! 
        // ... but only if it isn't a deadplant
        if (!tile.name.Equals("DeadPlant", System.StringComparison.Ordinal))
        {
            // TODO: Add a post-spawn merge check-delay!
            yield return new WaitForSeconds(spawnTimer);
            CheckForMerge(tilePosition);
        }
    }


    /* Merge:
     *  - Given a tile position, attempt to use it to merge into a new plant nearby.
     */
    public bool CheckForMerge(Vector3Int tilePosition)
    {
        // At this location, what's the plant tile?
        PlantTile plantTile = plantTileFromPosition[tilePosition];

        // What type of plant am I?
        Plant plant = plantTile.Plant;

        // Create empty list of potentialMerges
        List<List<PlantTile>> potentialMerges = new List<List<PlantTile>>();

        // Who are my neighboring triples?
        List<List<Vector3Int>> tileTriples = GetClusters(tilePosition);
        tileTriples.AddRange(GetTendrils(tilePosition));

        // For every triple, check if it forms a valid recipe and add to completedRecipes list.
        foreach (List<Vector3Int> triple in tileTriples)
        {
            // ValidRecipe should return a Plant if it found a valid merge, else a null.
            List<PlantTile> potentialMerge = ValidRecipe(triple);

            // If ValidRecipe returned a name, ther was a valid recipe. If null, no recipe found for this triple.
            if (potentialMerge != null)
            {
                potentialMerges.Add(potentialMerge);
            }
        }

        // If there are no potentialMerges, we can terminate early.
        if (potentialMerges.Count <= 0 || potentialMerges == null)
        {
            return false;
        }



        // Now that we have every valid merge, we just need to pick one from among them and complete the merge.
        // First, pick one of the completedRecipes by some method. 
        // TODO: By some other method than just random?
        int randomInt = Random.Range(0, potentialMerges.Count);

        List<PlantTile> chosenMerge = potentialMerges[randomInt];
        PlantTile chosenPlant = chosenMerge[Random.Range(0, 3)];

        // TODO: Implement a merge delay.

        // Update the tilemap with the new plant.
        plantTilemap.SetTile(tilePosition, chosenPlant.ThisTile);

        // Now we need to set the old PlantTile at tilePosition to be the chosenMerge
        plantTileFromPosition[tilePosition] = chosenPlant;

        // Since this new tile might now be used for a different recipe, we need to check for another merge here.
        // TODO: Add a merge delay here too.
        // CheckForMerge(tilePosition);

        return true;
    }


    // Get Clusters: There are 15 unique clusters that form around a central tile.
    // This method makes a list of them and returns them.
    private static List<List<Vector3Int>> GetClusters(Vector3Int centerTilePosition)
    {
        List<List<Vector3Int>> tileClusters = new List<List<Vector3Int>>();

        // The outer loop represents the 6 possible tiles that surround the central tile.
        for (int i = 0; i < 6; i++)
        {
            // Add the center tile since it will be part of every cluster.
            List<Vector3Int> cluster = new List<Vector3Int>();
            cluster.Add(centerTilePosition);

            // Add the first outer loop tile of this cluster, based on its position relative to center.
            // That is, centerTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
            Vector3Int targetCubePosition = HexMath.cubeDirections[i] + HexMath.OddrToCube(centerTilePosition);
            Vector3Int targetOddrPosition = HexMath.CubeToOddr(targetCubePosition);
            cluster.Add(targetOddrPosition);

            // The inner loop goes through each unique third tile addition to the group.
            for (int j = i + 1; j < 6; j++)
            {
                List<Vector3Int> triple = new List<Vector3Int>();

                // Add the inner loop tile of this cluster, based on its position relative to center.
                // That is, centerTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
                targetCubePosition = HexMath.cubeDirections[j] + HexMath.OddrToCube(centerTilePosition);
                targetOddrPosition = HexMath.CubeToOddr(targetCubePosition);
                triple.Add(targetOddrPosition);
                triple.AddRange(cluster);

                // Now add this cluster of three tiles to the tileClusters list.
                tileClusters.Add(triple);
            }
        }

        return tileClusters;
    }
    

    // Get Tendrils: There are 18 unique tendrils that form using a given tile as a base, not including the
    // tendrils that are also considered clusters.
    private static List<List<Vector3Int>> GetTendrils(Vector3Int baseTilePosition)
    {
        List<List<Vector3Int>> tileTendrils = new List<List<Vector3Int>>();

        // The for loop represents the 6 possible tiles that surround the base tile.
        for (int i = 0; i < 6; i++)
        {
            // Add the center tile since it will be part of every tendril.
            List<Vector3Int> tendril = new List<Vector3Int>();
            tendril.Add(baseTilePosition);

            // Add this loop's tile to this tednril, based on its position relative to the base.
            // That is, baseTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
            Vector3Int targetCubePosition = HexMath.cubeDirections[i] + HexMath.OddrToCube(baseTilePosition);
            Vector3Int targetOddrPosition = HexMath.CubeToOddr(targetCubePosition);
            tendril.Add(targetOddrPosition);

            // The third tile slightly is more complicated. It can be in any three directions based on the initial
            // direction, deviating by -1, 0, and +1, and making sure to wrap from -1 to 5.
            for (int d = -1; d <= 1; d++)
            {
                List<Vector3Int> triple = new List<Vector3Int>();

                int newDirection = ((i + d) % 6 + 6) % 6;
                targetCubePosition = HexMath.cubeDirections[newDirection] + HexMath.OddrToCube(baseTilePosition);
                targetOddrPosition = HexMath.CubeToOddr(targetCubePosition);
                triple.Add(targetOddrPosition);
                triple.AddRange(tendril);

                // Now add this tendril of three tiles to the tileTendrils list.
                tileTendrils.Add(triple);
            }
        }

        return tileTendrils;
    }


    // Given a list of three tiles, return the product of the recipe as a PlantTile if valid, null otherwise.
    public List<PlantTile> ValidRecipe(List<Vector3Int> triple)
    {
        Dictionary<string, int> recipe = new Dictionary<string, int>();

        // For each location in the triple, add the name of the plant there to the recipe
        foreach (Vector3Int tile in triple)
        {
            // If there is a plant at the tile location, get it's name.
            if (plantTileFromPosition.ContainsKey(tile))
            {
                // TODO: REFACTOR this to remove the assumption that if there's an empty in the triple, it can't be valid.
                if (plantTileFromPosition[tile].ThisTile.name.Equals("Empty", System.StringComparison.Ordinal))
                {
                    return null;
                }


                string key = plantTileFromPosition[tile].ThisTile.name;

                // If this tile.name is new, add it to the dictionary, else increment by 1.
                if (recipe.ContainsKey(key))
                {
                    recipe[key] += 1;
                }
                else
                {
                    recipe.Add(key, 1);
                }
            }
            else
            {
                // If there wasn't a plant there, uh, well, just return?
                return null;
            }
        }

        // Now the recipe dictionary contains this triple recipe, but is it valid?
        foreach (Plant plant in plantList)
        {
            // Does the recipe dictionary contain this plant's ingredient 1?
            if (recipe.ContainsKey(plant.ingredient1))
            {
                // Does the recipe use the proper count of ingredient 2?
                if (recipe[plant.ingredient1] != plant.count1)
                {

                    if (plant.name.Equals("WaterLily", System.StringComparison.Ordinal))
                    {
                        //Debug.Log("recipe[plant.ingredient1]: " + recipe[plant.ingredient1]);
                    }


                    continue;
                }
            }
            else
            {
                continue;
            }

            // Does the recipe dictionary contain this plant's ingredient 2?
            if (recipe.ContainsKey(plant.ingredient2))
            {
                // Does the recipe use the proper count of ingredient 2?
                if (recipe[plant.ingredient2] != plant.count2)
                {
                    continue;
                }
            }
            else
            {
                continue;
            }
            
            // If we reach this far, then the recipe matches this plant's recipe!
            // Make a PlantTile for each tile in the triple.
            List<PlantTile> chosenPlantTiles = new List<PlantTile>();
            foreach (Vector3Int tile in triple)
            {
                // Make a new PlantTile with this plant.
                var chosenPlantTile = ScriptableObject.CreateInstance<PlantTile>();
                chosenPlantTile.GridVector = tile;
                chosenPlantTile.WorldVector = plantTilemap.CellToWorld(tile);
                chosenPlantTile.ThisTile = plantTileFromName[plant.name];
                chosenPlantTile.TilemapMember = plantTilemap;
                chosenPlantTile.Name = tile.x + "," + tile.y;
                chosenPlantTile.Plant = plant;

                chosenPlantTiles.Add(chosenPlantTile);
            }

            return chosenPlantTiles;
        }

        return null;
    }

}